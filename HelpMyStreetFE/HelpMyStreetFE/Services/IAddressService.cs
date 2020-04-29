using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IAddressService
    {
        Task<GetPostCodeResponse> CheckPostCode(string postCode);
        Task<List<PostCodeDetail>> GetPostcodeDetailsNearUser(User user);
        Task<int> GetTotalStreets();
        Task<GetPostCodeCoverageResponse> GetPostcodeCoverage(string postcode);
        Task<GetPostcodesResponse> GetFriendlyNames(List<string> postcodes);
    }
}
