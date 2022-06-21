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
using System.Globalization;

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
        public async Task<ActionResult> StepOne(string referringGroup, string source, string email, CancellationToken cancellationToken)
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

            RegistrationFormVariant registrationFormVariant = await _groupService.GetRegistrationFormVariant(referringGroupId, source, cancellationToken);
            var group = await _groupService.GetGroupById(referringGroupId, CancellationToken.None);

            return View(new RegistrationViewModel
            {
                ActiveStep = 1,
                FirebaseConfiguration = _configuration["Firebase:Configuration"],
                RegistrationFormVariant = registrationFormVariant,
                ReferringGroupID = referringGroupId,
                Source = source,
                GroupName= group.GroupName,
                Email = email
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
                await _authService.LoginWithTokenAsync(userData.Token);

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
            var user = await _authService.GetCurrentUser(cancellationToken);
            string correctPage = GetCorrectPage(user);
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
            var user = await _authService.GetCurrentUser(cancellationToken);
            string correctPage = GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-two"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            try
            {
                DateTime dob = DateTime.ParseExact(form.DateOfBirth, DatePickerHelpers.DATE_PICKER_DATE_FORMAT, new CultureInfo("en-GB"));
                await _userService.CreateUserStepTwoAsync(user.ID, form.Postcode, form.FirstName, form.LastName, form.AddressLine1, form.AddressLine2, form.County, form.City, form.MobilePhone, form.OtherPhone, dob, cancellationToken);
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
            var user = await _authService.GetCurrentUser(cancellationToken);
            string correctPage = GetCorrectPage(user);
            
            if (!correctPage.StartsWith("/registration/step-three"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }
            var registrationFormVariant = await GetRegistrationJourney(user.ID, cancellationToken);
            var supportActivities = (await _groupService.GetSupportActivitesForRegistrationForm(registrationFormVariant)).OrderBy(x => x.DisplayOrder)
                .Select(x => new SupportActivityViewModel()
                {
                    SupportActivities = x.SupportActivity,
                    Description = x.Label,
                    Selected = x.IsPreSelected
                }).ToList();

            return View(new RegistrationViewModel
            {
                ActiveStep = 3,
                RegistrationFormVariant = registrationFormVariant,
                ActivityDetails = supportActivities
            });
        }

        [HttpPost("[controller]/step-three")]
        public async Task<ActionResult> StepThreePost([FromForm] StepThreeFormModel form, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            string correctPage = GetCorrectPage(user);
            if (!correctPage.StartsWith("/registration/step-three"))
            {
                // A different step needs to be completed at this point
                return Redirect(correctPage);
            }

            var registrationFormVariant = await GetRegistrationJourney(user.ID, cancellationToken);

            try
            {
                _logger.LogInformation($"Step 3 submission for {user.ID}");

                await _userService.CreateUserStepThreeAsync(
                    user.ID,
                    form.VolunteerOptions,
                    form.VolunteerDistance,
                    registrationFormVariant,
                    cancellationToken);

                await _groupMemberService.AddUserToDefaultGroups(user.ID);

                if (user.ReferringGroupId.HasValue && user.ReferringGroupId != -1)
                {
                    return Redirect($"/community/joinandgo/{Base64Utils.Base64Encode(user.ReferringGroupId.Value)}");
                }
                else
                {
                    return Redirect("/account");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error executing step 3");
                _logger.LogError(ex.ToString());
                return Redirect("/registration/step-three?failure=error");
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
                return await _groupService.GetRegistrationFormVariant(user.ReferringGroupId.Value, user.Source, cancellationToken);
            }
        }

        private string GetCorrectPage(User user)
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
                    default:
                        return "/account"; //Registration journey is complete
                }
            }

            return "/registration/step-one";
        }
    }
}
