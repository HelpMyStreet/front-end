using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class AuthService : IAuthService
    {
        private FirebaseAuth _firebase { get; set; }

        public AuthService()
        {
            var fb = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("C:\\projects\\factor50-test-firebase-adminsdk-54f9y-450bc8e44a.json")
            });

            _firebase = FirebaseAuth.GetAuth(fb);
        }

        public async Task LoginViaToken(string token, HttpContext httpContext)
        {
            var decoded = await _firebase.VerifyIdTokenAsync(token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, decoded.Uid));

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}
