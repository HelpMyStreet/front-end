using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class AddressRepository : BaseHttpRepository, IAddressRepository
    {
        public AddressRepository(HttpClient client,IConfiguration config, ILogger<AddressRepository> logger) : base(client,config, logger, "Services:Address")
        {
        }

        public async Task<NearbyPostcodeResponse> GetNearbyPostcodes(string postcode)
        {
            return await GetAsync<NearbyPostcodeResponse>($"/api/getnearbypostcodes?postcode={postcode}");
        }

        public async Task<GetPostcodesResponse> GetPostcodes(GetPostcodesRequest request)
        {
            return await PostAsync<GetPostcodesResponse>($"/api/GetPostCodes", request);
        }

        public async Task<List<LocationDistance>> GetLocationsByDistance(int distance, string postcode)
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
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.LocationDistances;
            }
            throw new System.Exception($"Bad response from GetLocationsByDistance");
        }

        public async Task<LocationDetails> GetLocationDetails(Location location)
        {
            var getLocationRequest = new GetLocationRequest
            {
                LocationRequest = new LocationRequest
                {
                    Location = location
                }
            };

            string json = JsonConvert.SerializeObject(getLocationRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetLocation", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetLocationResponse, AddressServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.LocationDetails;
            }
            throw new System.Exception($"Bad response from GetLocation");
        }

        public async Task<List<LocationDetails>> GetLocationDetails(IEnumerable<Location> locations)
        {
            var getLocationRequest = new GetLocationsRequest
            {
                LocationsRequests = new LocationsRequest
                {
                    Locations = locations.ToList()
                }
            };

            string json = JsonConvert.SerializeObject(getLocationRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetLocations", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetLocationsResponse, AddressServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.LocationDetails;
            }
            throw new System.Exception($"Bad response from GetLocations");
        }
    }
}
