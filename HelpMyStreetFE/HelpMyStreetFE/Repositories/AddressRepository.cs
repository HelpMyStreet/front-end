using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    }
}
