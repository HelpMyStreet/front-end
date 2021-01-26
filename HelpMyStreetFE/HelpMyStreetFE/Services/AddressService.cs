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

namespace HelpMyStreetFE.Services
{
    public class AddressService : BaseHttpService, IAddressService
    {
        private readonly IOptions<RequestSettings> _requestSettings;
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IMemDistCache<LocationDetails> _memDistCache;
        private readonly IUserRepository _userRepository;

        private const string CACHE_KEY_PREFIX = "address-service-";

        public AddressService(
            ILogger<AddressService> logger,
            IConfiguration configuration,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            IOptions<RequestSettings> requestSettings,
            IMemDistCache<LocationDetails> memDistCache,
            HttpClient client) : base(client, configuration, "Services:Address")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _requestSettings = requestSettings;
            _memDistCache = memDistCache;
        }

        public Task<int> GetTotalStreets()
        {
            return Task.Factory.StartNew(() => 1765422);  // TODO: Implement in Address Service
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

        public async Task<GetPostcodesResponse> GetFriendlyNames(List<string> postcodes)
        {
            GetPostcodesRequest request = new GetPostcodesRequest
            {
                PostcodeList = new PostcodeList
                {
                    Postcodes = postcodes.Select(x => x).ToList()
                },
                IncludeAddressDetails = false
            };
            return await _addressRepository.GetPostcodes(request);

        }

        public async Task<List<Location>> GetLocationsByDistance(string postcode)
        {
            var locationsResponse = await _addressRepository.GetLocationsByDistance(_requestSettings.Value.ShiftRadius, postcode);

            if (locationsResponse.IsSuccessful && locationsResponse.HasContent)
            {
                var content = locationsResponse.Content;
                return content.LocationDistances.Select(ld => ld.Location).ToList();
            }
            else
            {
                throw new HttpRequestException("Unable to fetch locations by distance");
            }
        }

        public async Task<LocationDetails> GetLocationDetails(Location location, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                var response = await _addressRepository.GetLocationDetails(location);
                if (response.HasContent && response.IsSuccessful)
                {
                    return response.Content.LocationDetails;
                }
                else
                {
                    throw new HttpRequestException("Unable to fetch location details");
                }
            }, $"{CACHE_KEY_PREFIX}-location-{(int)location}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<List<LocationDetails>> GetLocationDetails(IEnumerable<Location> locations)
        {

            var responses = locations.Select(location => GetLocationDetails(location, new CancellationToken()));
            var awaitedResponses = await Task.WhenAll(responses);

            return awaitedResponses.ToList();
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
