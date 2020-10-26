using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services.Users
{
    public interface IAuthService
    {
        Task Logout(HttpContext httpContext);
        Task LoginWithTokenAsync(string token, HttpContext httpContext);
        Task LoginWithUserId(int userId, HttpContext httpContext, CancellationToken cancellationToken);
        Task<string> VerifyIdTokenAsync(string token);
        Task<User> GetCurrentUser(HttpContext httpContext, CancellationToken cancellationToken);
    }
}