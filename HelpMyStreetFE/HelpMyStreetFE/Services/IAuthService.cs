using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services
{
    public interface IAuthService
    {
        Task LoginWithTokenAsync(string token, HttpContext httpContext);
        Task<string> VerifyIdTokenAsync(string token);
    }
}