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
using GetPostcodesResponse = HelpMyStreetFE.Models.Reponses.GetPostcodesResponse;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Services
{
    public class AddressService : BaseHttpService, IAddressService
    {
        private readonly ILogger<AddressService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;

        public AddressService(
            ILogger<AddressService> logger,
            IConfiguration configuration,
            IAddressRepository addressRepository,
            IUserRepository userRepository,
            HttpClient client) : base(client, configuration, "Services:Address")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressRepository = addressRepository;
            _userRepository = userRepository;
        }

        public Task<int> GetTotalStreets()
        {
            return Task.Factory.StartNew(() => 1765422);  // TODO: Implement in Address Service
        }

        public async Task<GetPostCodeResponse> CheckPostCode(string postcode)
        {
            postcode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode);
            var response = await Client.GetAsync($"/api/getpostcode?postcode={postcode}");
            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetPostCodeResponse>(str);
        }

        public async Task<GetPostCodeCoverageResponse> GetPostcodeCoverage(string postcode)
        {

            postcode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode);
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

        public async Task<ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>> GetLocationsByDistance(int distance, string postcode)
        {
            GetLocationsByDistanceRequest getLocationsByDistanceRequest = new GetLocationsByDistanceRequest()
            {
                MaxDistance = distance,
                Postcode = postcode
            };
            string json = JsonConvert.SerializeObject(getLocationsByDistanceRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetLocationsByDistance", data);
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>>(str);
        }

        public async Task<ResponseWrapper<GetLocationResponse, AddressServiceErrorCode>> GetLocationDetails(Location location)
        {
            var locationRequest = new LocationRequest();
            locationRequest.Location = location;

            var getLocationRequest = new GetLocationRequest();
            getLocationRequest.LocationRequest = locationRequest;

            string json = JsonConvert.SerializeObject(getLocationRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetLocationsByDistance", data);
            string str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResponseWrapper<GetLocationResponse, AddressServiceErrorCode>>(str);
        }

        public async Task<List<LocationDetails>> GetLocationDetails(IEnumerable<Location> locations)
        {

            var locationRequests = locations.Select(location => {
                var lr = new LocationRequest();
                lr.Location = location;
                var glr = new GetLocationRequest();
                glr.LocationRequest = lr;
                return glr;
            });

            var responses = new List<LocationDetails>();

            foreach (GetLocationRequest glr in locationRequests) {
                string json = JsonConvert.SerializeObject(glr);
                StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await Client.PostAsync("/api/GetLocationsByDistance", data);
                string str = await response.Content.ReadAsStringAsync();
                var outcome = JsonConvert.DeserializeObject<ResponseWrapper<GetLocationResponse, AddressServiceErrorCode>>(str);
                if (outcome.IsSuccessful && outcome.HasContent) {
                    responses.Add(outcome.Content.LocationDetails);
                }
            }
            return responses;

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
