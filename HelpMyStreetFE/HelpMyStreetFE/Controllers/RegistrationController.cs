using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Services;
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
        private readonly IAuthService _authService;

        public RegistrationController(ILogger<RegistrationController> logger, IUserService userService, IAuthService authService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService;
            _authService = authService;
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

        [AllowAnonymous]
        [HttpPost("[controller]/stepone")]
        public async Task<ActionResult> StepOnePost([FromBody] NewUserModel userData)
        {
            _logger.LogInformation("Posting new user");
            var uid = await _authService.VerifyIdTokenAsync(userData.Token);
            await _userService.CreateUser(userData.Email, uid);

            await _authService.LoginWithTokenAsync(uid, HttpContext);

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

        [HttpPost("[controller]/steptwo")]
        public async Task<ActionResult> StepTwoPost([FromForm] StepTwoFormModel form)
        {
            var personalDetails = FormToUserDetails(form);
            personalDetails.EmailAddress = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = new User
            {
                ID = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                FirebaseUID = HttpContext.User.FindFirstValue(ClaimTypes.Name),
                UserPersonalDetails = personalDetails
            };

            try
            {
                var resp = await _userService.UpdateUser(user);
                return Redirect("/registration/steptwo?failure=error");
            }
            catch
            {
                return Redirect("/registration/stepthree");
            }
        }

        [HttpGet("[controller]/stepthree")]
        public ActionResult StepThree()
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = 3
            });
        }

        [HttpGet("[controller]/stepfour")]
        public ActionResult StepFour()
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = 4
            });
        }

        private UserPersonalDetails FormToUserDetails(StepTwoFormModel form)
        {
            return new UserPersonalDetails
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Address = new Address
                {
                    AddressLine1 = form.AddressLine1,
                    AddressLine2 = form.AddressLine2,
                    Locality = form.City,
                    Postcode = form.Postcode,
                },
                MobilePhone = form.MobilePhone,
                OtherPhone = form.OtherPhone
            };
        }
    }
}
