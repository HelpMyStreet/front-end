using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HelpMyStreetFE.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseAuth _firebase;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;

            var credFileLocation = _configuration["Firebase:CredentialKeyFile"];

            if (credFileLocation == string.Empty)
            {
                throw new Exception("Credential file missing");
            }

            var fb = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credFileLocation)
            });

            _firebase = FirebaseAuth.GetAuth(fb);
        }

        public async Task<string> VerifyIdTokenAsync(string token)
        {
            var decoded = await _firebase.VerifyIdTokenAsync(token);

            return decoded.Uid;
        }

        public async Task LoginWithTokenAsync(string token, HttpContext httpContext)
        {
            var uid = await VerifyIdTokenAsync(token);
            var user = await _userRepository.GetUserByAuthId(uid);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.UserPersonalDetails.EmailAddress));

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}
