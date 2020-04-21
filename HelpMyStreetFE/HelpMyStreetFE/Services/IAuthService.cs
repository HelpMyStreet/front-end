using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services
{
    public interface IAuthService
    {
        Task Logout(HttpContext httpContext);
        Task LoginWithTokenAsync(string token, HttpContext httpContext);
        Task LoginWithUserId(int userId, HttpContext httpContext);
        Task<string> VerifyIdTokenAsync(string token);

    }
}