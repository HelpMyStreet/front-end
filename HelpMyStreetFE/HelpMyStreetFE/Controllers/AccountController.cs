using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Enums.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HelpMyStreetFE.Services;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models;
using Microsoft.Extensions.Configuration;
using HelpMyStreetFE.Helpers;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Email;
using Microsoft.AspNetCore.Http;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Jobs;
using System.Threading;
using HelpMyStreetFE.ViewComponents;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]

    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IConfiguration _configuration;
        private readonly IOptions<YotiOptions> _yotiOptions;
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;
        private readonly IAuthService _authService;

        private static readonly string REGISTRATION_URL = "/registration/step-two";
        private static readonly string PROFILE_URL = "/account";
        private static readonly string DEFAULT_VERIFIED_URL = "/account/open-requests";

        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IAddressService addressService,
            IConfiguration configuration,
            IOptions<YotiOptions> yotiOptions,
            IRequestService requestService,
            IGroupService groupService,
            IAuthService authService
            )
        {
            _logger = logger;
            _userService = userService;
            _addressService = addressService;
            _configuration = configuration;
            _yotiOptions = yotiOptions;
            _requestService = requestService;
            _groupService = groupService;
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            LoginViewModel model = new LoginViewModel
            {
                FirebaseConfiguration = _configuration["Firebase:Configuration"]
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            if (user.IsVerified.HasValue && user.IsVerified.Value)
            {
                return Redirect(DEFAULT_VERIFIED_URL);
            }
            else
            {
                return await Profile(cancellationToken);
            }
        }

        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.UserDetails;
            var userDetails = _userService.GetUserDetails(user);
            viewModel.PageModel = userDetails;
            return View("Index", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Streets(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.Notifications.Clear();
            viewModel.CurrentPage = MenuPage.MyStreets;
            var streetsViewModel = new StreetsViewModel();

            var friendlyPostcodes = await _addressService.GetFriendlyNames(viewModel.UserDetails.ChampionPostcodes);

            foreach (var postcode in viewModel.UserDetails.ChampionPostcodes)
            {
                Street street = new Street();
                street.Name = postcode;
                if (friendlyPostcodes.Content != null)
                {
                    street.FriendlyName = friendlyPostcodes.Content.PostcodesResponse[HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode)].FriendlyName;
                }
                var helpers = await _userService.GetHelpersByPostcode(postcode);
                var champs = await _userService.GetChampionsByPostcode(postcode);
                helpers.Users.AddRange(champs.Users);
                if (helpers.Users != null)
                {
                    foreach (var helper in helpers.Users.GroupBy(x => x.ID).Select(g => g.First()).ToList())// de duping
                    {
                        if (helper.ID == user.ID) continue;
                        if (!helper.IsVerified.HasValue || !helper.IsVerified.Value) continue;
                        bool isStreetChampion = (helper.StreetChampionRoleUnderstood.Value && helper.ChampionPostcodes.Contains(postcode));
                        street.Helpers.Add(new Helper
                        {
                            Name = helper.UserPersonalDetails.DisplayName,
                            PhoneNumber = helper.UserPersonalDetails.MobilePhone,
                            AlternatePhoneNumber = helper.UserPersonalDetails.OtherPhone,
                            Email = helper.UserPersonalDetails.EmailAddress,
                            SupportedActivites = helper.SupportActivities,
                            IsStreetChampion = isStreetChampion
                        });
                    }
                    streetsViewModel.Streets.Add(street);
                }
            }
            viewModel.PageModel = streetsViewModel;
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OpenRequests(CancellationToken cancellationToken)
        {

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.OpenRequests;
            return View("Index", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> AcceptedRequests(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.AcceptedRequests;

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CompletedRequests(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.CompletedRequests;

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LoadComponent(CancellationToken cancellationToken)
        {
            var user = await GetCurrentUser(cancellationToken); 
            return ViewComponent("Awards", new { userID = user.ID, cancellationToken = cancellationToken });
        }

        [HttpGet]
        public async Task<IActionResult> Group(string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            if (await _groupService.GetUserHasRole(user.ID, groupKey, GroupRoles.TaskAdmin, cancellationToken))
            {
                return await GroupRequests(groupKey, cancellationToken);
            }
            else if (await _groupService.GetUserHasRole(user.ID, groupKey, GroupRoles.UserAdmin, cancellationToken))
            {
                return await GroupVolunteers(groupKey, cancellationToken);
            }

            return Redirect(PROFILE_URL);
        }

        [HttpGet]
        public async Task<IActionResult> GroupRequests(string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!_groupService.GetUserHasRole(viewModel.UserGroups, groupKey, GroupRoles.TaskAdmin))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupRequests;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();

            return View("Index", viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        [AuthorizeAttributeNoRedirect]
        public async Task<int> NavigationBadge(MenuPage menuPage, string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return 0;
            }

            int count = await new AccountNavBadgeViewComponent(_requestService, _groupService).GetCount(user, menuPage, groupKey, cancellationToken);

            return count;
        }

        [HttpGet]
        public async Task<IActionResult> GroupVolunteers(string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!_groupService.GetUserHasRole(viewModel.UserGroups, groupKey, GroupRoles.UserAdmin))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupVolunteers;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();

            return View("Index", viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> CloseNotification(Guid id)
        {
            //TODO: Pass this id to something that will stop that notification from being sent
            await Task.CompletedTask;
            return Ok();
        }

        private async Task<AccountViewModel> GetAccountViewModel(User user, CancellationToken cancellationToken)
        {
            var viewModel = new AccountViewModel();

            if (user != null)
            {
                List<NotificationModel> notifications = new List<NotificationModel>()
                {
                    new NotificationModel
                    {
                        Id = Guid.NewGuid(),
                        Title = "Good news " + user.UserPersonalDetails.FirstName +"!",
                        Message = "<p>Your account is all set up.</p>" +
                        " <div> Coming Soon: " +
                            "<ul style='margin-top:2px;'> " +
                                "<li>You will soon be able to update the personal and volunteering details on your profile page. </li>" +
                                "<li>Street Champions will be able to manage their streets, search for local volunteers, and handle requests for help.</li>" +
                            "</ul> " +
                        "</div>" +
                        "<p>Keep an eye on your email inbox for the latest updates. Thanks for joining HelpMyStreet!</p>",
                        Type = NotificationType.Success
                    }
                };
                var userDetails = _userService.GetUserDetails(user);
                viewModel.Notifications = notifications;
                var jobs = await _requestService.GetJobsForUserAsync(user.ID, true, cancellationToken);
                viewModel.Jobs = jobs.ToList();
                viewModel.VerificationViewModel = new Models.Yoti.VerificationViewModel
                {
                    YotiOptions = _yotiOptions.Value,
                    EncodedUserID = Base64Utils.Base64Encode(user.ID),
                    DisplayName = userDetails.DisplayName,
                    IsStreetChampion = userDetails.IsStreetChampion,
                    IsVerified = userDetails.IsVerified,
                };

                viewModel.UserDetails = userDetails;

                viewModel.UserGroups = await _groupService.GetUserGroupRoles(user.ID, cancellationToken);
            }

            return viewModel;
        }
    }
}
