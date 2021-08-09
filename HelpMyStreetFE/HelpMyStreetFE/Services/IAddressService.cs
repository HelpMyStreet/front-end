using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using HelpMyStreetFE.Models.Account;

namespace HelpMyStreetFE.Services
{
    public interface IAddressService
    {
        Task<GetPostCodeResponse> CheckPostCode(string postCode);
        Task<GetPostCodeCoverageResponse> GetPostcodeCoverage(string postcode);
        Task<IEnumerable<LocationWithDistance>> GetLocationDetailsForUser(User user, CancellationToken cancellationToken);
        Task<IEnumerable<LocationDetails>> GetLocationDetailsForGroup(int groupId, bool includeChildGroups, CancellationToken cancellationToken);
        Task<LocationDetails> GetLocationDetails(Location location, CancellationToken cancellationToken);
        Task<double> GetDistanceBetweenPostcodes(string postCode1, string postCode2, CancellationToken cancellationToken);
        Task<List<PostcodeCoordinate>> GetPostcodeCoordinates(string postcode);
        Task<double> GetDistanceFromPostcodeForCurrentUser(string postCode, CancellationToken cancellationToken);
        Task<LocationWithDistance> GetLocationWithDistance(IContainsLocation locationItem, CancellationToken cancellationToken);
        Task<LocationWithDistance> GetLocationWithDistance(IEnumerable<IContainsLocation> locationItem, CancellationToken cancellationToken);
    }
}
