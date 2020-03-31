using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HelpMyStreetFE.Services
{
    public class Address
    {
        public string houseName { get; set; }
        public int? houseNumber { get; set; }
    }

    public class PostCodeResponse
    {
        public string postCode { get; set; }
        public List<Address> addresses { get; set; }
    }

    public class AddressService : IAddressService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly ILogger<AddressService> _logger;

        public AddressService(ILogger<AddressService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            client.DefaultRequestHeaders.Add("x-functions-key", "xjof6oogVYjn2MHYTWHUDXhvm4NGa1Vk8NhbJ3pouiehPAiqTM6MlA==");
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

        public async Task<PostCodeResponse> CheckPostCode(string postCode)
        {
            _logger.LogInformation($"Checking postcode {postCode}");
            var reqJson = JsonSerializer.Serialize(new { postCode });
            _logger.LogInformation($"Sending {reqJson}");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://helpmystreet-address-service-dev.azurewebsites.net/api/GetPostCode"),
                Content = new StringContent(reqJson, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(request);
            _logger.LogInformation($"Status Code: {response.StatusCode}");
            var str = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Received: {str}");
            return JsonSerializer.Deserialize<PostCodeResponse>(str); ;
        }
    }
}
