using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Reponses;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface IAddressRepository
    {
        Task<NearbyPostcodeResponse> GetNearbyPostcodes(string postcode);
        Task<GetPostcodesResponse> GetPostcodes(GetPostcodesRequest request);
        Task<ResponseWrapper<GetLocationResponse, AddressServiceErrorCode>> GetLocationDetails(Location location);
        Task<ResponseWrapper<GetLocationsByDistanceResponse, AddressServiceErrorCode>> GetLocationsByDistance(int distance, string postcode);
    }
}