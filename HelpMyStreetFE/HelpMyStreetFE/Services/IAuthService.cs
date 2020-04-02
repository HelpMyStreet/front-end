using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HelpMyStreetFE.Services
{
    public interface IAuthService
    {
        Task LoginViaToken(string token, HttpContext httpContext);
    }
}