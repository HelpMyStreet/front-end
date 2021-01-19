using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Enums.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Helpers;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using HelpMyStreetFE.ViewComponents;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using System.Net.Http;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreetFE.Services;

namespace HelpMyStreetFE.Controllers
{
    [Authorize]
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IOptions<YotiOptions> _yotiOptions;
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly ICommunicationService _communicationService;

        private static readonly string REGISTRATION_URL = "/registration/step-two";
        private static readonly string PROFILE_URL = "/account/open-requests";

        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IAddressService addressService,
            ICommunicationService communicationService,
            IOptions<YotiOptions> yotiOptions,
            IRequestService requestService,
            IAuthService authService,
            IGroupMemberService groupMemberService
            )
        {
            _logger = logger;
            _userService = userService;
            _yotiOptions = yotiOptions;
            _requestService = requestService;
            _authService = authService;
            _groupMemberService = groupMemberService;
            _communicationService = communicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string next, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }
            else if (next == "verify")
            {
                return await Profile(next, cancellationToken);
            }
            else
            {
                return Redirect(PROFILE_URL);
            }
        }

        [Route("profile")]
        public async Task<IActionResult> Profile(string next, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken, next == "verify");
            viewModel.CurrentPage = MenuPage.UserDetails;
            var userDetails = _userService.GetUserDetails(user);
            viewModel.PageModel = userDetails;
            return View("Index", viewModel);
        }


        [Route("open-requests")]
        [Route("open-requests/j/{encodedJobId}")]
        [HttpGet]
        public async Task<IActionResult> OpenRequests(string encodedJobId, CancellationToken cancellationToken)
        {

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.OpenRequests;

            if (!string.IsNullOrEmpty(encodedJobId))
            {
                viewModel.HighlightJobId = Base64Utils.Base64DecodeToInt(encodedJobId);
            }

            return View("Index", viewModel);
        }


        [Route("accepted-requests")]
        [Route("accepted-requests/j/{encodedJobId}")]
        [HttpGet]
        public async Task<IActionResult> AcceptedRequests(string encodedJobId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.AcceptedRequests;

            if (!string.IsNullOrEmpty(encodedJobId))
            {
                viewModel.HighlightJobId = Base64Utils.Base64DecodeToInt(encodedJobId);
            }

            return View("Index", viewModel);
        }

        [Route("completed-requests")]
        [Route("completed-requests/j/{encodedJobId}")]
        [HttpGet]
        public async Task<IActionResult> CompletedRequests(string encodedJobId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.CompletedRequests;

            if (!string.IsNullOrEmpty(encodedJobId))
            {
                viewModel.HighlightJobId = Base64Utils.Base64DecodeToInt(encodedJobId);
            }

            return View("Index", viewModel);
        }

        [Route("my-shifts")]
        [HttpGet]
        public async Task<IActionResult> MyShifts(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.MyShifts;

            return View("Index", viewModel);
        }

        [Route("open-shifts")]
        [HttpGet]
        public async Task<IActionResult> OpenShifts(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.OpenShifts;

            return View("Index", viewModel);
        }

        [Route("get-awards-component")]
        [HttpGet]
        public async Task<IActionResult> LoadAwardsComponent(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken); 
            return ViewComponent("Awards", new { userID = user.ID, cancellationToken = cancellationToken });
        }

        [Route("print-job-details")]
        [HttpGet]
        public async Task<IActionResult> PrintJobDetails(string j, CancellationToken cancellationToken)
        {
            var jobID = Base64Utils.Base64DecodeToInt(j);
            User user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            return ViewComponent("JobDetail", new { JobID = jobID, User = user, Jobset = JobSet.UserAcceptedRequests, ToPrint = true});
        }

        [Route("email-job-details")]
        [HttpGet]
        [AllowAnonymous]
        [AuthorizeAttributeNoRedirect]
        public async Task<IActionResult> EmailJobDetails(string j, CancellationToken cancellationToken)
        {
            var jobID = Base64Utils.Base64DecodeToInt(j);
            
            User user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (user == null)
            {
                return Unauthorized();
            }

            var commsResponse = await _communicationService.RequestCommunication(null, user.ID, jobID, new CommunicationJob() { CommunicationJobType = CommunicationJobTypes.TaskDetail });

            if (commsResponse)
            {
                return Ok();
            } else
            {
                return new StatusCodeResult(500);
            }

        }

        [Route("g/{groupKey}")]
        [HttpGet]
        public async Task<IActionResult> Group(string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            if (await _groupMemberService.GetUserHasRole(user.ID, groupKey, GroupRoles.TaskAdmin, cancellationToken))
            {
                return await GroupRequests(groupKey, null, cancellationToken);
            }
            else if (await _groupMemberService.GetUserHasRole_Any(user.ID, groupKey, new List<GroupRoles> { GroupRoles.UserAdmin, GroupRoles.UserAdmin_ReadOnly }, cancellationToken))
            {
                return await GroupVolunteers(groupKey, cancellationToken);
            }

            return Redirect(PROFILE_URL);
        }

        [Route("g/{groupKey}/requests")]
        [Route("g/{groupKey}/requests/j/{encodedJobId}")]
        [HttpGet]
        public async Task<IActionResult> GroupRequests(string groupKey, string encodedJobId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!_groupMemberService.GetUserHasRole(viewModel.UserGroups, groupKey, GroupRoles.TaskAdmin))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupRequests;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();

            if (!string.IsNullOrEmpty(encodedJobId))
            {
                viewModel.HighlightJobId = Base64Utils.Base64DecodeToInt(encodedJobId);
            }

            return View("Index", viewModel);
        }

        [Route("g/{groupKey}/shifts")]
        [HttpGet]
        public async Task<IActionResult> GroupShifts(string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!_groupMemberService.GetUserHasRole(viewModel.UserGroups, groupKey, GroupRoles.TaskAdmin))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupShifts;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();

            return View("Index", viewModel);
        }

        [Route("get-logged-in-status")]
        [HttpGet]
        [AllowAnonymous]
        [AuthorizeAttributeNoRedirect]
        public async Task<bool> GetLoggedInStatus()
        {
            return true;
        }

        [Route("get-navigation-badge")]
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

            int count = await new AccountNavBadgeViewComponent(_requestService, _groupMemberService).GetCount(user, menuPage, groupKey, cancellationToken);

            return count;
        }

        [Route("g/{groupKey}/volunteers")]
        [HttpGet]
        public async Task<IActionResult> GroupVolunteers(string groupKey, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!_groupMemberService.GetUserHasRole_Any(viewModel.UserGroups, groupKey, new List<GroupRoles> { GroupRoles.UserAdmin, GroupRoles.UserAdmin_ReadOnly }))
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

        private async Task<AccountViewModel> GetAccountViewModel(User user, CancellationToken cancellationToken, bool verifyNow = false)
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
                            "</ul> " +
                        "</div>" +
                        "<p>Keep an eye on your email inbox for the latest updates. Thanks for joining HelpMyStreet!</p>",
                        Type = NotificationType.Success
                    }
                };
                var userDetails = _userService.GetUserDetails(user);
                viewModel.Notifications = notifications;
                viewModel.VerificationViewModel = new VerificationViewModel
                {
                    YotiOptions = _yotiOptions.Value,
                    EncodedUserID = Base64Utils.Base64Encode(user.ID),
                    DisplayName = userDetails.DisplayName,
                    StartAtStep = verifyNow ? 1 : 0,
                };

                viewModel.UserDetails = userDetails;

                viewModel.UserGroups = await _groupMemberService.GetUserGroupRoles(user.ID, cancellationToken);

                viewModel.VerificationViewModel.IsVerified = await _groupMemberService.GetUserIsVerified(user.ID, cancellationToken);
            }

            return viewModel;
        }
    }
}
