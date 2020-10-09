using System;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Services;
using HelpMyStreetFE.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Groups;

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
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        public RegistrationController(
            ILogger<RegistrationController> logger,
            IUserService userService,
            IAuthService authService,
            IAddressService addressService,
            IConfiguration configuration,
            IGroupService groupService,
            IGroupMemberService groupMemberService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService;
            _authService = authService;
            _addressService = addressService;
            _configuration = configuration;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> StepOne(string referringGroup = "", string source = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/account");
            }

            int referringGroupId = -1;

            if (!string.IsNullOrEmpty(referringGroup))
            {
                try
                {
                    referringGroupId = Base64Utils.Base64DecodeToInt(referringGroup);
                }
                catch { }
            }

            RegistrationFormVariant registrationFormVariant = await _groupService.GetRegistrationFormVariant(referringGroupId, source) ?? RegistrationFormVariant.Default;

            return View(new RegistrationViewModel
            {
                ActiveStep = 1,
                FirebaseConfiguration = _configuration["Firebase:Configuration"],
                RegistrationFormVariant = registrationFormVariant,
                ReferringGroupID = referringGroupId,
                Source = source,
            });
        }

        [AllowAnonymous]
        [HttpPost("[controller]/step-one")]
        public async Task<ActionResult> StepOnePost([FromBody] NewUserModel userData)
        {
            try
            {
                _logger.LogInformation("Posting new user");
                var uid = await _authService.VerifyIdTokenAsync(userData.Token);
                await _userService.CreateUserAsync(userData.Email, uid, Convert.ToInt32(userData.ReferringGroupId), userData.Source);
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

        [HttpGet("[controller]/step-two")]
        public async Task<ActionResult> StepTwo(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            string correctPage = await GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-two"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            return View(new RegistrationViewModel
            {
                ActiveStep = 2,
                RegistrationFormVariant = await GetRegistrationJourney(user.ID, cancellationToken)
            });
        }

        [HttpPost("[controller]/step-two")]
        public async Task<ActionResult> StepTwoPost([FromForm] StepTwoFormModel form, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            string correctPage = await GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-two"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            try
            {
                await _userService.CreateUserStepTwoAsync(user.ID, form.Postcode, form.FirstName, form.LastName, form.AddressLine1, form.AddressLine2, form.County, form.City, form.MobilePhone, form.OtherPhone, form.DateOfBirth, cancellationToken);
                return Redirect("/registration/step-three");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 2");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/step-two?failure=error");
            }
        }

        [HttpGet("[controller]/step-three")]
        public async Task<ActionResult> StepThree(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            string correctPage = await GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-three"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            return View(new RegistrationViewModel
            {
                ActiveStep = 3,
                RegistrationFormVariant = await GetRegistrationJourney(user.ID, cancellationToken),
            });
        }

        [HttpPost("[controller]/step-three")]
        public async Task<ActionResult> StepThreePost([FromForm] StepThreeFormModel form, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            string correctPage = await GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-three"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            if(form.HasCustomDistance)
            {
                form.VolunteerDistance = form.CustomDistance;
            }

            try
            {
                _logger.LogInformation($"Step 3 submission for {user.ID}");

                await _userService.CreateUserStepThreeAsync(
                    user.ID,
                    form.VolunteerOptions,
                    form.VolunteerDistance,
                    cancellationToken);

                await _groupMemberService.AddUserToDefaultGroups(user.ID);

                return Redirect("/registration/step-four");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 3");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/step-three?failure=error");
            }
        }

        [HttpGet("[controller]/step-four")]
        public async Task<ActionResult> StepFour(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            string correctPage = await GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-four"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

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

        [HttpPost("[controller]/step-four")]
        public async Task<ActionResult> StepFourPost([FromForm] StepFourFormModel form, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            string correctPage = await GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-four"))
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
                _logger.LogInformation($"Step 4 submission for {user.ID}");
                await _userService.CreateUserStepFourAsync(
                    user.ID,
                    form.ChampionRoleUnderstood,
                    form.ChampionPostcodes,
                    cancellationToken);

                return Redirect("/account");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 4");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/step-four?failure=error");
            }
        }

        private async Task<RegistrationFormVariant> GetRegistrationJourney(int userId, CancellationToken cancellationToken)
        {
            User user = await _userService.GetUserAsync(userId, cancellationToken);

            if (!user.ReferringGroupId.HasValue)
            {
                return RegistrationFormVariant.Default;
            }
            else
            {
                return await _groupService.GetRegistrationFormVariant(user.ReferringGroupId.Value, user.Source) ?? RegistrationFormVariant.Default;
            }
        }

        private async Task<string> GetCorrectPage(User user)
        {
            if (user != null && user.RegistrationHistory.Count > 0)
            {
                int maxStep = user.RegistrationHistory.Max(a => a.Key);

                switch (maxStep)
                {
                    case 1:
                        return "/registration/step-two";
                    case 2:
                        return "/registration/step-three";
                    case 3:
                        return "/registration/step-four";
                    default:
                        return "/account"; //Registration journey is complete
                }
            }

            return "/registration/step-one";
        }
    }
}
