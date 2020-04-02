using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IUserService _userService;

        public RegistrationController(ILogger<RegistrationController> logger, IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("[controller]/stepone")]
        public ActionResult StepOne()
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = 1
            });
        }

        [HttpPost("[controller]/stepone")]
        public async Task<ActionResult> StepOnePost([FromBody] NewUserModel userData)
        {
            _logger.LogInformation("Posting new user");
            var uid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.CreateUser(userData.Email, uid);

            return Redirect("/registration/steptwo");
        }

        [HttpGet("[controller]/steptwo")]
        public ActionResult StepTwo()
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = 2
            });
        }
    }
}
