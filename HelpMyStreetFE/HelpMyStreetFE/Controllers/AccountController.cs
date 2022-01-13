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
using HelpMyStreet.Utils.Extensions;
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
using System.Text;
using System.Net;

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
        private readonly IRequestCachingService _requestCachingService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IGroupService _groupService;
        private readonly ICommunicationService _communicationService;
        private readonly IAddressService _addressService;
        private readonly IFilterService _filterService;

        private static readonly string REGISTRATION_URL = "/registration/step-two";
        private static readonly string PROFILE_URL = "/account/open-requests";

        public AccountController(
            ILogger<AccountController> logger,
            IUserService userService,
            IAddressService addressService,
            IGroupService groupService,
            ICommunicationService communicationService,
            IOptions<YotiOptions> yotiOptions,
            IRequestService requestService,
            IAuthService authService,
            IGroupMemberService groupMemberService,
            IFilterService filterService,
            IRequestCachingService requestCachingService
            )
        {
            _logger = logger;
            _userService = userService;
            _yotiOptions = yotiOptions;
            _requestService = requestService;
            _authService = authService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _communicationService = communicationService;
            _addressService = addressService;
            _filterService = filterService;
            _requestCachingService = requestCachingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string next, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
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
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken, next == "verify");
            viewModel.CurrentPage = MenuPage.UserDetails;
            var userDetails = await _userService.GetUserDetails(user, cancellationToken);
            viewModel.PageModel = userDetails;
            return View("Index", viewModel);
        }


        [Route("open-requests")]
        [Route("open-requests/j/{encodedJobId}")]
        [Route("open-requests/r/{encodedRequestId}")]
        [HttpGet]
        public async Task<IActionResult> OpenRequests(string encodedJobId, string encodedRequestId, CancellationToken cancellationToken)
        {

            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.OpenRequests;
            AddHighlightIdsToViewModel(viewModel, encodedJobId, encodedRequestId);

            return View("Index", viewModel);
        }


        [Route("my-requests")]
        [Route("my-requests/j/{encodedJobId}")]
        [Route("my-requests/r/{encodedRequestId}")]
        [HttpGet]
        public async Task<IActionResult> MyRequests(string encodedJobId, string encodedRequestId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.MyRequests;
            AddHighlightIdsToViewModel(viewModel, encodedJobId, encodedRequestId);

            return View("Index", viewModel);
        }

        [Route("my-shifts")]
        [Route("my-shifts/j/{encodedJobId}")]
        [Route("my-shifts/r/{encodedRequestId}")]
        [HttpGet]
        public async Task<IActionResult> MyShifts(string encodedJobId, string encodedRequestId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.MyShifts;
            AddHighlightIdsToViewModel(viewModel, encodedJobId, encodedRequestId);

            return View("Index", viewModel);
        }

        [Route("open-shifts")]
        [Route("open-shifts/j/{encodedJobId}")]
        [Route("open-shifts/r/{encodedRequestId}")]
        [HttpGet]
        public async Task<IActionResult> OpenShifts(string encodedJobId, string encodedRequestId, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            viewModel.CurrentPage = MenuPage.OpenShifts;
            AddHighlightIdsToViewModel(viewModel, encodedJobId, encodedRequestId);

            return View("Index", viewModel);
        }

        [Route("get-awards-component")]
        [HttpGet]
        public async Task<IActionResult> LoadAwardsComponent(CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(cancellationToken); 
            return ViewComponent("Awards", new { userID = user.ID, cancellationToken = cancellationToken });
        }

        [Route("print-job-details")]
        [HttpGet]
        public async Task<IActionResult> PrintJobDetails(string j, CancellationToken cancellationToken)
        {
            var jobID = Base64Utils.Base64DecodeToInt(j);
            User user = await _authService.GetCurrentUser(cancellationToken);

            return ViewComponent("JobDetail", new { JobID = jobID, User = user, Jobset = JobSet.UserMyRequests, ToPrint = true});
        }

        [Route("get-directions-link")]
        [HttpGet]
        public async Task<IActionResult> GetDirectionsLink(string r, CancellationToken cancellationToken)
        {
            int.TryParse(Base64Utils.Base64Decode(r), out int requestId);
            var shiftDetails = (await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken));
            string locationPostcode;

            if (shiftDetails.RequestType == RequestType.Task)
            {
                locationPostcode = WebUtility.UrlEncode(shiftDetails.PostCode);
            }
            else
            {
                var location = shiftDetails.Shift.Location;
                LocationDetails locationDetails = await _addressService.GetLocationDetails(location, cancellationToken);

                locationPostcode = WebUtility.UrlEncode(locationDetails.Address.Postcode);
            }
            var directionsLink = $"https://www.google.com/maps/dir/?api=1&destination={locationPostcode}";

            return new OkObjectResult(directionsLink);
        }

        [Route("get-shift-calendar")]
        [HttpGet]
        public async Task<IActionResult> GetShiftCalendar(string j, CancellationToken cancellationToken)
        {
            int.TryParse(Base64Utils.Base64Decode(j), out int jobID);
            User user = await _authService.GetCurrentUser(cancellationToken);
            var shiftDetails = await _requestService.GetJobAndRequestSummaryAsync(jobID, cancellationToken);
            if (shiftDetails == null || shiftDetails.RequestType != RequestType.Shift)
            {
                throw new Exception("Job does not exist or Not a shift");
            }
            if (shiftDetails.VolunteerUserID == null || shiftDetails.VolunteerUserID != user.ID)
            {
                throw new Exception("User not assigned to shift");
            }

            var location = shiftDetails.RequestSummary.Shift.Location;
            LocationDetails locationDetails = await _addressService.GetLocationDetails(location, cancellationToken);

            var startDate = shiftDetails.RequestSummary.Shift.StartDate.ToUniversalTime()
                         .ToString("yyyy''MM''dd'T'HH''mm''ss'Z'");
            var stopDate = shiftDetails.RequestSummary.Shift.EndDate.ToUniversalTime()
                         .ToString("yyyy''MM''dd'T'HH''mm''ss'Z'");
            var stampDate = DateTime.Now.ToUniversalTime()
                         .ToString("yyyy''MM''dd'T'HH''mm''ss'Z'");
            var group = await _groupService.GetGroupById(shiftDetails.RequestSummary.ReferringGroupID, cancellationToken);

            var icalContent = $"BEGIN:VCALENDAR\n" +
                $"VERSION:2.0\n" +
                $"PRODID:-//Help My Street/iCal Support\\EN\n" +
                $"BEGIN:VEVENT\n" +
                $"UID:hms-vacc-{group.GroupId}-{j}\n" +
                $"DTSTAMP:{stampDate}\n" +
                $"ORGANIZER;CN={group.GroupName}MAILTO:no-reply@helpymstreet.org\n" +
                $"DTSTART:{startDate}\n" +
                $"DTEND:{stopDate}\n" +
                $"SUMMARY:{shiftDetails.SupportActivity.FriendlyNameShort()} Shift\n" +
                $"LOCATION:{locationDetails.Address.AddressLine1}, {locationDetails.Address.Postcode}\n" +
                $"GEO:{locationDetails.Latitude};{locationDetails.Longitude}\n" +
                $"URL:https://www.helpmystreet.org/link/j/{j}\n" +
                $"END:VEVENT\n" +
                $"END:VCALENDAR";
            var calBytes = Encoding.ASCII.GetBytes(icalContent);

            return File(fileContents: calBytes, fileDownloadName: "shift-event.ics", contentType: "text/calendar");
        }

        [Route("email-job-details")]
        [HttpGet]
        [AllowAnonymous]
        [AuthorizeAttributeNoRedirect]
        public async Task<IActionResult> EmailJobDetails(string j, CancellationToken cancellationToken)
        {
            var jobID = Base64Utils.Base64DecodeToInt(j);
            
            User user = await _authService.GetCurrentUser(cancellationToken);
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
            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            if (await _groupMemberService.GetUserHasRole(user.ID, group.GroupId, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                if (group.TasksEnabled)
                {
                    return await GroupRequests(groupKey, null, null, cancellationToken);
                }
                else if (group.ShiftsEnabled)
                {
                    return await GroupShifts(groupKey, null, null, cancellationToken);
                }
            }
            else if (await _groupMemberService.GetUserHasRole_Any(user.ID, group.GroupId, new List<GroupRoles> { GroupRoles.UserAdmin, GroupRoles.UserAdmin_ReadOnly }, true, cancellationToken))
            {
                return await GroupVolunteers(groupKey, cancellationToken);
            }

            return Redirect(PROFILE_URL);
        }

        [Route("g/{groupKey}/requests")]
        [Route("g/{groupKey}/requests/j/{encodedJobId}")]
        [Route("g/{groupKey}/requests/r/{encodedRequestId}")]
        [HttpGet]
        public async Task<IActionResult> GroupRequests(string groupKey, string encodedJobId, string encodedRequestId, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!await _groupMemberService.GetUserHasRole(user.ID, group.GroupId, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupRequests;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();
            AddHighlightIdsToViewModel(viewModel, encodedJobId, encodedRequestId);

            return View("Index", viewModel);
        }

        [Route("g/{groupKey}/shifts")]
        [Route("g/{groupKey}/shifts/j/{encodedJobId}")]
        [Route("g/{groupKey}/shifts/r/{encodedRequestId}")]
        [HttpGet]
        public async Task<IActionResult> GroupShifts(string groupKey, string encodedJobId, string encodedRequestId, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!await _groupMemberService.GetUserHasRole(user.ID, group.GroupId, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupShifts;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();
            AddHighlightIdsToViewModel(viewModel, encodedJobId, encodedRequestId);

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
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return 0;
            }

            int count = await new AccountNavBadgeViewComponent(_requestService, _filterService, _groupMemberService, _groupService).GetCount(user, menuPage, groupKey, cancellationToken);

            return count;
        }

        [Route("g/{groupKey}/volunteers")]
        [HttpGet]
        public async Task<IActionResult> GroupVolunteers(string groupKey, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);
            if (!_userService.GetRegistrationIsComplete(user))
            {
                return Redirect(REGISTRATION_URL);
            }

            var viewModel = await GetAccountViewModel(user, cancellationToken);
            if (!await _groupMemberService.GetUserHasRole_Any(user.ID, group.GroupId, new List<GroupRoles> { GroupRoles.UserAdmin, GroupRoles.UserAdmin_ReadOnly }, true, cancellationToken))
            {
                return Redirect(PROFILE_URL);
            }

            viewModel.CurrentPage = MenuPage.GroupVolunteers;
            viewModel.CurrentGroup = viewModel.UserGroups.Where(a => a.GroupKey == groupKey).FirstOrDefault();

            return View("Index", viewModel);
        }

        [Route("g/{groupKey}/reports")]
        [HttpGet]
        public async Task<IActionResult> Reports(string groupKey, CancellationToken cancellationToken)
        {
            var group = await _groupService.GetGroupByKey(groupKey, cancellationToken);
            var user = await _authService.GetCurrentUser(cancellationToken);
            var viewModel = await GetAccountViewModel(user, cancellationToken);

            viewModel.CurrentPage = MenuPage.Reports;
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
                var userDetails = await _userService.GetUserDetails(user, cancellationToken);
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

        private void AddHighlightIdsToViewModel(AccountViewModel viewModel, string encodedJobId, string encodedRequestId)
        {
            if (!string.IsNullOrEmpty(encodedJobId))
            {
                viewModel.HighlightJobId = Base64Utils.Base64DecodeToInt(encodedJobId);
            }
            if (!string.IsNullOrEmpty(encodedRequestId))
            {
                viewModel.HighlightRequestId = Base64Utils.Base64DecodeToInt(encodedRequestId);
            }
        }
    }
}
