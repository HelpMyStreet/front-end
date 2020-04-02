using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IUserService _userService;

        public RegistrationController(ILogger<RegistrationController> logger, IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService;
        }

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
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // await _userService.CreateUser(userData.Email, HttpContext.User.Identity.);

            return Ok();
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
