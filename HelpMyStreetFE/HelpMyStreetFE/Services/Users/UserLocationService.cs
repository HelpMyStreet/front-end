using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Services.Users
{
    public class UserLocationService : IUserLocationService
    {
        private readonly IAddressService _addressService;
        private readonly IAuthService _authService;
        private readonly IGroupService _groupService;
        private readonly IMemDistCache<IEnumerable<LocationWithDistance>> _memDistCache_LocationDistanceList;


        private const string CACHE_KEY_PREFIX = "user-location-service-";

        public UserLocationService(IGroupService groupService, IAddressService addressService, IAuthService authService, IMemDistCache<IEnumerable<LocationWithDistance>> memDistCache_LocationDistanceList)
        {
            _authService = authService;
            _addressService = addressService;
            _groupService = groupService;
            _memDistCache_LocationDistanceList = memDistCache_LocationDistanceList;
        }

        public async Task<IEnumerable<LocationWithDistance>> GetLocationDetailsForUser(User user, CancellationToken cancellationToken)
        {
            if (user.PostalCode != null)
            {
                return await _memDistCache_LocationDistanceList.GetCachedDataAsync(async (cancellationToken) => {

                    var userLocations = await _groupService.GetUserLocations(user.ID);
                    var locationsWithDistance = await _addressService.GetLocationsByDistance(2000, user.PostalCode);
                    if (locationsWithDistance.Count() == 0)
                    {
                        return new List<LocationWithDistance>();
                    }
                    if (userLocations != null)
                    {
                        locationsWithDistance = locationsWithDistance.Where(x => userLocations.Contains(x.Location)).ToList();
                        if (locationsWithDistance.Count() == 0)
                        {
                            return new List<LocationWithDistance>();
                        }
                    }

                    return await Task.WhenAll(locationsWithDistance.Select(async l => {
                        var lDetails = await _addressService.GetLocationDetails(l.Location, cancellationToken);
                        return new LocationWithDistance()
                        {
                            Distance = l.DistanceFromPostCode,
                            Location = l.Location,
                            LocationDetails = lDetails
                        };
                    }));

                }, $"{CACHE_KEY_PREFIX}-user-{user.ID}-locations", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
            }
            else
            {
                return new List<LocationWithDistance>();
            }
        }


        public async Task<double> GetDistanceFromPostcodeForCurrentUser(string postCode, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);

            if (user != null && !String.IsNullOrEmpty("postCode"))
            {
                return await _addressService.GetDistanceBetweenPostcodes(user.PostalCode, postCode, cancellationToken);
            }
            

            return 0.0;
        }

        public async Task<LocationWithDistance> GetLocationWithDistanceForCurrentUser(IContainsLocation locationItem, CancellationToken cancellationToken)
        {
            var locationDetails = locationItem.GetLocationDetails();

            if (locationDetails.Location != 0)
            {
                locationDetails = await _addressService.GetLocationDetails(locationDetails.Location, cancellationToken);
            }

            double distance = 0;
            if (locationDetails.Address != null)
            {
                distance = await GetDistanceFromPostcodeForCurrentUser(locationDetails.Address.Postcode, cancellationToken);
            }

            var lwd = new LocationWithDistance()
            {
                Distance = distance,
                Location = locationDetails.Location,
                LocationDetails = locationDetails
            };

            return lwd;
        }
    }
}
