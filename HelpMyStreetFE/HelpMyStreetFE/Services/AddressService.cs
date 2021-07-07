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

namespace HelpMyStreetFE.Services
{
    public class AddressService : BaseHttpService, IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IMemDistCache<LocationDetails> _memDistCache;
        private readonly IMemDistCache<IEnumerable<LocationDetails>> _memDistCache_LocationDetailsList;
        private readonly IMemDistCache<IEnumerable<LocationWithDistance>> _memDistCache_LocationDistanceList;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberService _groupMemberService;

        private const string CACHE_KEY_PREFIX = "address-service-";

        public AddressService(
            ILogger<AddressService> logger,
            IConfiguration configuration,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            IMemDistCache<LocationDetails> memDistCache,
            IMemDistCache<IEnumerable<LocationDetails>> memDistCache_LocationDetailsList,
            IMemDistCache<IEnumerable<LocationWithDistance>> memDistCache_LocationDistanceList,
            IGroupRepository groupRepository,
            IGroupMemberService groupMemberService,
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
                //check if user is member of groupid =-32 if yes set to 2000d else 20d 
                //this problem will go away when combining requests and shifts
                int defaultShiftRadius = 20;
                var userIsMemberOfApexBankStaff = await _groupMemberService.GetUserHasRole_Any(
                    user.ID,
                    (int)HelpMyStreet.Utils.Enums.Groups.ApexBankStaff,
                    new List<GroupRoles>() { GroupRoles.Member },
                    true,
                    cancellationToken);

                if(userIsMemberOfApexBankStaff)
                {
                    //user is member of apex bank staff
                    defaultShiftRadius = 2000;
                }
    
                var locationsWithDistance = await _addressRepository.GetLocationsByDistance(defaultShiftRadius, user.PostalCode);
                if (locationsWithDistance.Count() == 0)
                {
                    return new List<LocationWithDistance>();
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

        public async Task<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>> GetPostcodeCoordinates(GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest)
        {
            string json = JsonConvert.SerializeObject(getPostcodeCoordinatesRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetPostcodeCoordinates", data);
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>>(str);
        }

        public async Task<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>> GetPostcodeCoordinate(string postcode)
        {
            postcode = PostcodeFormatter.FormatPostcode(postcode);
            GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest = new GetPostcodeCoordinatesRequest()
            {
                Postcodes = new List<string>() { postcode }
            };
            ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode> getPostcodeCoordinatesResponse = await GetPostcodeCoordinates(getPostcodeCoordinatesRequest);

            return getPostcodeCoordinatesResponse;
        }
    }
}
