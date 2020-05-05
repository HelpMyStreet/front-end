using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        private readonly IConfiguration _configuration;
        public RegistrationController(
            ILogger<RegistrationController> logger,
            IUserService userService,
            IAuthService authService,
            IAddressService addressService,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService;
            _authService = authService;
            _addressService = addressService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("[controller]/stepone")]
        public ActionResult StepOne()
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = 1,
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            });
        }

        [AllowAnonymous]
        [HttpPost("[controller]/stepone")]
        public async Task<ActionResult> StepOnePost([FromBody] NewUserModel userData)
        {
            try
            {
                _logger.LogInformation("Posting new user");
                var uid = await _authService.VerifyIdTokenAsync(userData.Token);
                await _userService.CreateUserAsync(userData.Email, uid);
                await _authService.LoginWithTokenAsync(userData.Token, HttpContext);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 1");
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
        }

        [HttpGet("[controller]/steptwo")]
        public async Task<ActionResult> StepTwo()
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string correctPage = await GetCorrectPage(userId);
            if (correctPage != "/registration/steptwo")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            return View(new RegistrationViewModel
            {
                ActiveStep = 2
            });
        }

        [HttpPost("[controller]/steptwo")]
        public async Task<ActionResult> StepTwoPost([FromForm] StepTwoFormModel form)
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // Remove any references to User in session so on next Load it fetches the updated values;
            HttpContext.Session.Remove("User");
            string correctPage = await GetCorrectPage(userId);
            if (correctPage != "/registration/steptwo")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            try
            {
                await _userService.CreateUserStepTwoAsync(userId, form.Postcode, form.FirstName, form.LastName, form.AddressLine1, form.AddressLine2, form.County, form.City, form.MobilePhone, form.OtherPhone, form.DateOfBirth);
                return Redirect("/registration/stepthree");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 2");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/steptwo?failure=error");
            }
        }

        [HttpGet("[controller]/stepthree")]
        public async Task<ActionResult> StepThree()
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string correctPage = await GetCorrectPage(userId);
            if (correctPage != "/registration/stepthree")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            return View(new RegistrationViewModel
            {
                ActiveStep = 3
            });
        }

        [HttpPost("[controller]/stepthree")]
        public async Task<ActionResult> StepThreePost([FromForm] StepThreeFormModel form)
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // Remove any references to User in session so on next Load it fetches the updated values;
            HttpContext.Session.Remove("User");
            string correctPage = await GetCorrectPage(userId);
            if (correctPage != "/registration/stepthree")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            try
            {
                _logger.LogInformation($"Step 3 submission for {userId}");

                await _userService.CreateUserStepThreeAsync(
                    userId,
                    form.VolunteerOptions,
                    form.VolunteerDistance,
                    form.VolunteerPhoneContact,
                    form.VolunteerMedicalCondition);

                return Redirect("/registration/stepfour");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 3");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/stepthree?failure=error");
            }
        }

        [HttpGet("[controller]/stepfour")]
        public async Task<ActionResult> StepFour()
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            string correctPage = await GetCorrectPage(userId);
            if (correctPage != "/registration/stepfour")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            var user = await _userService.GetUserAsync(userId);
            var nearby = await _addressService.GetPostcodeDetailsNearUser(user);

            var userPostcode = nearby.Where(p => p.Postcode == user.PostalCode).FirstOrDefault();
            var nearbyToDisplay = nearby.Where(p => p.Postcode != user.PostalCode).OrderBy(p => p.ChampionCount).ThenBy(p => p.DistanceInMetres).Take(4).ToList();

            // Insert user postcode at the top of the list...
            nearbyToDisplay.Insert(0, userPostcode);

            // ...but re-sort if there are already 2 champions there
            if (userPostcode.ChampionCount >= 2)
            {
                nearbyToDisplay = nearbyToDisplay.OrderBy(p => p.ChampionCount).ThenBy(p => p.DistanceInMetres).ToList();
            }

            var championsNeeded = nearbyToDisplay.Aggregate(false, (acc, next) => acc || next.ChampionCount < 2);

            return View(new StepFourRegistrationViewModel
            {
                ActiveStep = 4,
                NearbyPostCodes = nearbyToDisplay,
                UsersPostCode = userPostcode,
                LocalAvailability = championsNeeded
            });
        }

        [HttpPost("[controller]/stepfour")]
        public async Task<ActionResult> StepFourPost([FromForm] StepFourFormModel form)
        {
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // Remove any references to User in session so on next Load it fetches the updated values;
            HttpContext.Session.Remove("User");
            string correctPage = await GetCorrectPage(userId);
            if (correctPage != "/registration/stepfour")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            try
            {
                if (!form.ChampionRoleUnderstood)
                {
                    form.ChampionPostcodes = new System.Collections.Generic.List<string>();
                }
                _logger.LogInformation($"Step 4 submission for {userId}");
                await _userService.CreateUserStepFourAsync(
                    userId,
                    form.ChampionRoleUnderstood,
                    form.ChampionPostcodes);

                return Redirect("/registration/stepfive");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 4");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/stepfour?failure=error");
            }
        }

        [HttpGet("[controller]/stepfive")]
        public async Task<IActionResult> StepFive()
        {       
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // Remove any references to User in session so on next Load it fetches the updated values;
            HttpContext.Session.Remove("User");
            string correctPage = await GetCorrectPage(int.Parse(userId));
            if (correctPage != "/registration/stepfive")
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }     
            var viewModel = new RegistrationViewModel { ActiveStep = 5, EncodedUserID = Base64Helpers.Base64Encode(userId) };
            return View(viewModel);
        }

        public async Task<string> GetCorrectPage(int userId)
        {
            User user = await _userService.GetUserAsync(userId);
            return GetCorrectPage(user);
        }

        public static string GetCorrectPage(User user)
        {
            if (user.RegistrationHistory.Count > 0)
            {
                int maxStep = user.RegistrationHistory.Max(a => a.Key);

                switch (maxStep)
                {
                    case 1:
                        return "/registration/steptwo";
                    case 2:
                        return "/registration/stepthree";
                    case 3:
                        return "/registration/stepfour";
                    case 4:
                        return "/registration/stepfive";
                    default:
                        return string.Empty; //Registration journey is complete
                }
            }

            return "/registration/stepone";
        }
    }
}
