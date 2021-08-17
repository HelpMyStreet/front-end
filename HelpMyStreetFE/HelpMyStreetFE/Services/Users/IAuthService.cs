using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services.Users
{
    public interface IAuthService
    {
        Task Logout();
        Task LoginWithTokenAsync(string token);
        Task LoginWithUserId(int userId, CancellationToken cancellationToken);
        Task<string> VerifyIdTokenAsync(string token);
        Task<User> GetCurrentUser(CancellationToken cancellationToken);
        void PutSessionAuthorisedUrl(string authorisedURL);
        bool GetUrlIsSessionAuthorised(string url);
        bool GetUrlIsSessionAuthorised();
    }
}