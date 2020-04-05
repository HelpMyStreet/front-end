using HelpMyStreetFE.Models.Reponses;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IAddressService
    {
        Task<GetPostCodeResponse> CheckPostCode(string postCode);
        Task<int> GetPostCodesCovered();
        Task<int> GetStreetChampions();
        Task<int> GetStreetsCovered();
        Task<int> GetStreetsRemaining();
    }
}
