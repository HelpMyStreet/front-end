using HelpMyStreet.Contracts.AddressService.Request;

using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Email;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Services.Groups;
using Microsoft.AspNetCore.Http;
using HelpMyStreetFE.Services.Users;
using System.Security.Claims;

namespace HelpMyStreetFE.Services
{
    public class AddressService : BaseHttpService, IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IMemDistCache<LocationDetails> _memDistCache;
        private readonly IMemDistCache<IEnumerable<LocationDetails>> _memDistCache_LocationDetailsList;
        private readonly IMemDistCache<IEnumerable<LocationWithDistance>> _memDistCache_LocationDistanceList;
        private readonly IMemDistCache<double> _memDistCache_PostcodeDistances;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string CACHE_KEY_PREFIX = "address-service-";

        public AddressService(
            ILogger<AddressService> logger,
            IConfiguration configuration,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            IMemDistCache<LocationDetails> memDistCache,
            IMemDistCache<IEnumerable<LocationDetails>> memDistCache_LocationDetailsList,
            IMemDistCache<IEnumerable<LocationWithDistance>> memDistCache_LocationDistanceList,
            IMemDistCache<double> memDistCache_PostcodeDistances,
            IGroupRepository groupRepository,
            IGroupMemberService groupMemberService,
            IHttpContextAccessor httpContextAccessor,
            HttpClient client) : base(client, configuration, "Services:Address")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _groupMemberService = groupMemberService;
            _memDistCache = memDistCache;
            _memDistCache_LocationDetailsList = memDistCache_LocationDetailsList;
            _memDistCache_LocationDistanceList = memDistCache_LocationDistanceList;
            _memDistCache_PostcodeDistances = memDistCache_PostcodeDistances;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetPostCodeResponse> CheckPostCode(string postcode)
        {
            postcode = PostcodeFormatter.FormatPostcode(postcode);
            var response = await Client.GetAsync($"/api/getpostcode?postcode={postcode}");
            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetPostCodeResponse>(str);
        }

        public async Task<GetPostCodeCoverageResponse> GetPostcodeCoverage(string postcode)
        {
            postcode = PostcodeFormatter.FormatPostcode(postcode);
            GetPostCodeCoverageResponse response = new GetPostCodeCoverageResponse();
            response.PostCodeResponse = await CheckPostCode(postcode);
            if (response.PostCodeResponse.HasContent && response.PostCodeResponse.IsSuccessful)
            {
                response.VolunteerCount = await _userRepository.GetVolunteerCountByPostcode(postcode);
            }

            return response;
        }

        public async Task<IEnumerable<LocationDetails>> GetLocationDetailsForGroup(int groupId, bool includeChildGroups, CancellationToken cancellationToken)
        {
            return await _memDistCache_LocationDetailsList.GetCachedDataAsync(async (cancellationToken) =>
            {
                var locations = await _groupRepository.GetGroupLocations(groupId, includeChildGroups);

                if (locations.Count() == 0)
                {
                    return new List<LocationDetails>();
                }

                return await _addressRepository.GetLocationDetails(locations);
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}-{includeChildGroups}-locations", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<IEnumerable<LocationWithDistance>> GetLocationDetailsForUser(User user, CancellationToken cancellationToken)
        {
            if (user.PostalCode != null)
            {
                return await _memDistCache_LocationDistanceList.GetCachedDataAsync(async (cancellationToken) => {

                    var userLocations = await _groupRepository.GetUserLocations(user.ID);    
                    var locationsWithDistance = await _addressRepository.GetLocationsByDistance(2000, user.PostalCode);
                    if (locationsWithDistance.Count() == 0)
                    {
                        return new List<LocationWithDistance>();
                    }
                    if(userLocations!=null)
                    {
                        locationsWithDistance = locationsWithDistance.Where(x => userLocations.Contains(x.Location)).ToList();
                        if (locationsWithDistance.Count() == 0)
                        {
                            return new List<LocationWithDistance>();
                        }
                    }

                    var locationDetails = await _addressRepository.GetLocationDetails(locationsWithDistance.Select(l => l.Location));

                    return locationsWithDistance.Select(l => new LocationWithDistance
                    {
                        Location = l.Location,
                        Distance = l.DistanceFromPostCode,
                        LocationDetails = locationDetails.FirstOrDefault(d => d.Location.Equals(l.Location))
                    });
                    }, $"{CACHE_KEY_PREFIX}-user-{user.ID}-locations", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
            }
            else
            {
                return new List<LocationWithDistance>();
            }
        }

        public async Task<LocationDetails> GetLocationDetails(Location location, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                var response = await _addressRepository.GetLocationDetails(location);
                if (response != null)
                {
                    return response;
                }
                else
                {
                    throw new HttpRequestException("Unable to fetch location details");
                }
            }, $"{CACHE_KEY_PREFIX}-location-{(int)location}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }



        public async Task<List<PostcodeCoordinate>> GetPostcodeCoordinates(string postcode)
        {
            postcode = PostcodeFormatter.FormatPostcode(postcode);

            GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { postcode }
            };

            var response = await _addressRepository.GetPostcodeCoordinates(getPostcodeCoordinatesRequest);
            return response.PostcodeCoordinates.ToList();
        }

        public async Task<double> GetDistanceBetweenPostcodes(string postCode1, string postCode2, CancellationToken cancellationToken)
        {

            return await _memDistCache_PostcodeDistances.GetCachedDataAsync(async (cancellationToken) =>
            {
                var response = await _addressRepository.GetDistanceBetweenPostcodes(postCode1, postCode2);
                if (response != null)
                {
                    return response.DistanceInMiles;
                }
                else
                {
                    throw new HttpRequestException("Unable to fetch location details");
                }
            }, $"{CACHE_KEY_PREFIX}-postcode-distances-{postCode1}-{postCode2}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<double> GetDistanceFromPostcodeForCurrentUser(string postCode, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                var id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user =  await _userRepository.GetUser(id);
                if (user != null && !String.IsNullOrEmpty("postCode"))
                {
                    return await GetDistanceBetweenPostcodes(user.PostalCode, postCode, cancellationToken);
                }
            }

            return 0.0;
        }

        public async Task<LocationWithDistance> GetLocationWithDistanceForCurrentUser(IContainsLocation locationItem, CancellationToken cancellationToken)
        {
            var locationDetails = locationItem.GetLocationDetails();

            if (locationDetails.Location != 0)
            {
                locationDetails = await GetLocationDetails(locationDetails.Location, cancellationToken);
            }

            var lwd = new LocationWithDistance()
            {
                Distance = await GetDistanceFromPostcodeForCurrentUser(locationDetails.Address.Postcode, cancellationToken),
                Location = locationDetails.Location,
                LocationDetails = locationDetails
            };

            return lwd;
        }
    }
}
