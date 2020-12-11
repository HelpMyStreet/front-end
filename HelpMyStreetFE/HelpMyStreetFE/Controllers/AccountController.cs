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
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using System.Net.Http;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.CommunicationService.Request;

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
        private readonly IGroupMemberService _groupMemberService;
        private readonly ICommunicationService _communicationService;

        private static readonly string REGISTRATION_URL = "/registration/step-two";
        private static readonly string PROFILE_URL = "/account/open-requests";

        private Dictionary<string, string> Errors = new Dictionary<string, string>()
        {
            { "login", "Sorry, we couldn't find an account with that email address and password.  Please check and try again" },
            {"server", "Uh-oh, something has gone wrong at our end. Please try again" },
            {"email", "Please enter a valid email address" },
            {"password", "Please enter a valid password" }
        };

        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IAddressService addressService,
            ICommunicationService communicationService,
            IConfiguration configuration,
            IOptions<YotiOptions> yotiOptions,
            IRequestService requestService,
            IGroupService groupService,
            IAuthService authService,
            IGroupMemberService groupMemberService
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
            _groupMemberService = groupMemberService;
            _communicationService = communicationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string er, string referringGroup, string source, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
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
        public async Task<IActionResult> LoadAwardsComponent(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken); 
            return ViewComponent("Awards", new { userID = user.ID, cancellationToken = cancellationToken });
        }

        [HttpGet]
        public async Task<IActionResult> PrintJobDetails(string id, CancellationToken cancellationToken)
        {
            var jobID = Base64Utils.Base64DecodeToInt(id);
            User user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (user == null)
            {
                return Redirect("/account/login?returnURL=/account/printJobDetails?id=" + id);
            }

            var jovm = new JobDetailOutputViewModel()
            {
                JobID = jobID,
                UserID = user.ID
            };

            return View(jovm);
        }

        [HttpGet]
        public async Task<IActionResult> EmailJobDetails(string id, CancellationToken cancellationToken)
        {
            var jobID = Base64Utils.Base64DecodeToInt(id);
            
            User user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
            if (user == null)
            {
                return Unauthorized();
            }

            var job = await _requestService.GetJobDetailsAsync(jobID, user.ID, cancellationToken);


            var commsResponse = await _communicationService.RequestCommunication(job.JobSummary.ReferringGroupID, user.ID, jobID, new CommunicationJob() { CommunicationJobType = CommunicationJobTypes.TaskDetail });

            if (commsResponse)
            {
                return Ok();
            } else
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobHTML(int JobID, int UserID, CancellationToken cancellationToken)
        {
            return ViewComponent("JobDetailOutput", new { JobID = JobID, UserID = UserID });
        }


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
                return await GroupRequests(groupKey, cancellationToken);
            }
            else if (await _groupMemberService.GetUserHasRole_Any(user.ID, groupKey, new List<GroupRoles> { GroupRoles.UserAdmin, GroupRoles.UserAdmin_ReadOnly }, cancellationToken))
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
            if (!_groupMemberService.GetUserHasRole(viewModel.UserGroups, groupKey, GroupRoles.TaskAdmin))
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
        public async Task<bool> GetLoggedInStatus()
        {
            return true;
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

            int count = await new AccountNavBadgeViewComponent(_requestService, _groupMemberService).GetCount(user, menuPage, groupKey, cancellationToken);

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
