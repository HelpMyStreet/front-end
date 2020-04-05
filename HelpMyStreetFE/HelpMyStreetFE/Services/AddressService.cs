using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class AddressService : BaseHttpService, IAddressService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly ILogger<AddressService> _logger;

        public AddressService(ILogger<AddressService> logger, IConfiguration configuration) : base(configuration, "Services:Address")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int GetVolunteerCount()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetStreetsCovered()
        {
            return Task.Factory.StartNew(() => 1734);
        }

        public Task<int> GetStreetChampions()
        {
            return Task.Factory.StartNew(() => 2834);
        }

        public Task<int> GetStreetsRemaining()
        {
            return Task.Factory.StartNew(() => 8182);
        }

        public Task<int> GetPostCodesCovered()
        {
            return Task.Factory.StartNew(() => 15);
        }

        public async Task<GetPostCodeResponse> CheckPostCode(string postcode)
        {
            var response = await Client.GetAsync($"/api/getpostcode?postcode={postcode}");
            return JsonConvert.DeserializeObject<GetPostCodeResponse>(await response.Content.ReadAsStringAsync());
        }
    }
}
