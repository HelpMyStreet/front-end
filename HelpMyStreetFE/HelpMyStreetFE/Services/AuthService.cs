using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HelpMyStreetFE.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseAuth _firebase;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;

            var firebaseCredentials = _configuration["Firebase:Credentials"];

            if (firebaseCredentials == string.Empty)
            {
                throw new Exception("Firebase cedentials missing");
            }

            var fb = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(firebaseCredentials)
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
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.UserPersonalDetails.FirstName + " " + user.UserPersonalDetails.LastName));

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        public async Task LoginWithUserId(int userId, HttpContext httpContext)
        {
            var user = await _userRepository.GetUser(userId);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.UserPersonalDetails.EmailAddress));

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        public async Task Logout(HttpContext httpContext)
        {
            httpContext.Session.Clear();
            await httpContext.SignOutAsync();            
        }
    }
}
