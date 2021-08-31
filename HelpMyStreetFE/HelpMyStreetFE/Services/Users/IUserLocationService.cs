using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services.Users
{
    public interface IUserLocationService
    {
        Task<double> GetDistanceFromPostcodeForCurrentUser(string postCode, CancellationToken cancellationToken);
        Task<LocationWithDistance> GetLocationWithDistanceForCurrentUser(IContainsLocation locationItem, CancellationToken cancellationToken);
        Task<IEnumerable<LocationWithDistance>> GetLocationDetailsForUser(User user, CancellationToken cancellationToken);
    }
}