using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
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
    }
}
