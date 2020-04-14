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
using System.Threading.Tasks;

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
            HttpClient client) : base(client,configuration, "Services:Address")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressRepository = addressRepository;
            _userRepository = userRepository;
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
            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetPostCodeResponse>(str);
        }

        public async Task<List<PostCodeDetail>> GetPostcodeDetailsNearUser(User user)
        {
            var postCodes = await _addressRepository.GetNearbyPostcodes(user.PostalCode);

            var nearby = new List<PostCodeDetail>();

            foreach (var postcode in postCodes.Content.Postcodes)
            {
                var street = string.Concat(postcode.AddressDetails[0].AddressLine1.Where(c => !char.IsNumber(c)));
                var champs = await _userRepository.GetChampionCountByPostcode(postcode.Postcode);
                nearby.Add(new PostCodeDetail { StreetName = street, ChampionCount = champs, Postcode = postcode.Postcode });
            }

            return nearby;
        }
    }
}
