using System;
using System.Linq;
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
        private readonly IAddressService _addressService;

        public RegistrationController(
            ILogger<RegistrationController> logger,
            IUserService userService,
            IAuthService authService,
            IAddressService addressService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService;
            _authService = authService;
            _addressService = addressService;
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
            try
            {
                _logger.LogInformation("Posting new user");
                var uid = await _authService.VerifyIdTokenAsync(userData.Token);
                await _userService.CreateUserAsync(userData.Email, uid);
                await _authService.LoginWithTokenAsync(userData.Token, HttpContext);

                return Ok();
            } catch (Exception ex)
            {
                _logger.LogError("Error executing step 1");
                _logger.LogError(ex.ToString());
                return StatusCode(500);
            }
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
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                await _userService.CreateUserStepTwoAsync(id, form.Postcode, form.FirstName, form.LastName, form.AddressLine1, form.AddressLine2, form.County, form.City, form.MobilePhone, form.OtherPhone, form.DateOfBirth);
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
        public ActionResult StepThree()
        {
            return View(new RegistrationViewModel
            {
                ActiveStep = 3
            });
        }

        [HttpPost("[controller]/stepthree")]
        public async Task<ActionResult> StepThreePost([FromForm] StepThreeFormModel form)
        {
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                _logger.LogInformation($"Step 3 submission for {id}");

                await _userService.CreateUserStepThreeAsync(
                    id,
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
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userService.GetUserAsync(id);
            var nearby = await _addressService.GetPostcodeDetailsNearUser(user);

            var nearbyWithoutUser = nearby.Where(p => p.Postcode != user.PostalCode).OrderBy(c => c.ChampionCount).ToList();
            var userPostcode = nearby.Where(p => p.Postcode == user.PostalCode).FirstOrDefault();
            var localAvailability = !nearby.Aggregate(false, (acc, next) => acc || next.ChampionCount < 2);

            // If the user's postcode is available or no other postcodes are available, lead with the user's post code
            if (userPostcode.ChampionCount < 2 || localAvailability)
            {
                nearbyWithoutUser.Insert(0, userPostcode);
            } else
            {
                nearbyWithoutUser.Insert(0, userPostcode);
            }

            return View(new StepFourRegistrationViewModel
            {
                ActiveStep = 4,
                NearbyPostCodes = nearby,
                UsersPostCode = userPostcode,
                LocalAvailability = localAvailability
            });
        }

        [HttpPost("[controller]/stepfour")]
        public async Task<ActionResult> StepFourPost([FromForm] StepFourFormModel form)
        {
            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                _logger.LogInformation($"Step 4 submission for {id}");

                await _userService.CreateUserStepFourAsync(
                    id,
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
    }
}
