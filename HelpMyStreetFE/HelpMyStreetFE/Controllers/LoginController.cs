using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HelpMyStreetFE.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        private static readonly string PROFILE_URL = "/account/open-requests";

        public LoginController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        private Dictionary<string, string> Errors = new Dictionary<string, string>()
        {
            { "login", "Sorry, we couldn't find an account with that email address and password. Please check and try again" },
            { "server", "Uh-oh, something has gone wrong at our end. Please try again" },
            { "email", "Please enter a valid email address" },
            { "password", "Please enter a valid password" }
        };


        [Route("login")]
        [Route("login/{referringGroup}")]
        [Route("login/{referringGroup}/{source}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string er, string referringGroup, string source, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (user != null)
            {
                return Redirect(PROFILE_URL);
            }

            var errorMessage = String.IsNullOrEmpty(er) ? "" : Errors[er];

            LoginViewModel model = new LoginViewModel
            {
                Email = email,
                EmailError = er == "email" ? errorMessage : "",
                LoginError = er != "email" ? errorMessage : "",
                FirebaseConfiguration = _configuration["Firebase:Configuration"],
                SignUpURL = BuildSignUpUrl(referringGroup, source),
            };
            return View(model);
        }

        private string BuildSignUpUrl(string referringGroup, string source)
        {
            if (string.IsNullOrEmpty(referringGroup))
            {
                return "/registration";
            }
            else if (string.IsNullOrEmpty(source))
            {
                return $"/registration/{referringGroup}";
            }
            else
            {
                return $"/registration/{referringGroup}/{source}";
            }
        }

    }
}
