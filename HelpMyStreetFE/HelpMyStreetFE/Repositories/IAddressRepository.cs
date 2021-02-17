using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface IAddressRepository
    {
        Task<NearbyPostcodeResponse> GetNearbyPostcodes(string postcode);
        Task<GetPostcodesResponse> GetPostcodes(GetPostcodesRequest request);
        Task<LocationDetails> GetLocationDetails(Location location);
        Task<List<LocationDetails>> GetLocationDetails(IEnumerable<Location> location);
        Task<List<LocationDistance>> GetLocationsByDistance(int distance, string postcode);
    }
}