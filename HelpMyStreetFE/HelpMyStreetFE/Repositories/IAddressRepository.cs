using HelpMyStreet.Contracts.AddressService.Request;

using HelpMyStreetFE.Models.Reponses;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface IAddressRepository
    {
        Task<NearbyPostcodeResponse> GetNearbyPostcodes(string postcode);
        Task<GetPostcodesResponse> GetPostcodes(GetPostcodesRequest request);
    }
}