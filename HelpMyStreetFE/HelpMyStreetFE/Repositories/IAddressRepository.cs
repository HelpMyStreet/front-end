using HelpMyStreetFE.Models.Reponses;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface IAddressRepository
    {
        Task<NearbyPostcodeResponse> GetNearbyPostcodes(string postcode);
    }
}