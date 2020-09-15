using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
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
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseAuth _firebase;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IUserService userService, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userService = userService;
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
            FirebaseToken decoded;

            try
            {
                decoded = await _firebase.VerifyIdTokenAsync(token);
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception($"ID {token} not authorized at Firebase", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception calling _firebase.VerifyIdTokenAsync", ex);
            }

            return decoded.Uid;
        }

        public async Task LoginWithTokenAsync(string token, HttpContext httpContext)
        {
            var uid = await VerifyIdTokenAsync(token);

            User user;

            try
            {
                user = await _userService.GetUserByAuthId(uid);
            }
            catch (Exception ex)
            {
                throw new Exception($"User {uid} not found in User Service", ex);
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                });
        }

        public async Task LoginWithUserId(int userId, HttpContext httpContext, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserAsync(userId, cancellationToken);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                });
        }

        public async Task<User> GetCurrentUser(HttpContext httpContext, CancellationToken cancellationToken)
        {
            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                var id = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return await _userService.GetUserAsync(id, cancellationToken);
            }
            else
            {
                return null;
            }
        }


        public async Task Logout(HttpContext httpContext)
        {
            httpContext.Session.Clear();
            await httpContext.SignOutAsync();            
        }
    }
}
