using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Jobs;
using System.Threading;
using HelpMyStreetFE.Helpers;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Models.Account;

namespace HelpMyStreetFE.Controllers {


    [Route("api/request-help")]
    [ApiController]
    public class RequestHelpAPIController : Controller
    {
        private readonly ILogger<RequestHelpAPIController> _logger;
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;

        public RequestHelpAPIController(ILogger<RequestHelpAPIController> logger, IRequestService requestService, IAuthService authService, IGroupMemberService groupMemberService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
        }


        [AuthorizeAttributeNoRedirect]
        [HttpGet("set-job-status")]
        public async Task<ActionResult<string>> SetJobStatus(string j, JobStatuses s, string u, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);
                int jobId = Base64Utils.Base64DecodeToInt(j);

                int? targetUserId = null;
                if (s == JobStatuses.InProgress)
                {
                    targetUserId = u == "self" ? user.ID : Base64Utils.Base64DecodeToInt(u);
                }

                UpdateJobStatusOutcome? outcome = await _requestService.UpdateJobStatusAsync(jobId, s, user.ID, targetUserId, cancellationToken);

                switch (outcome)
                {
                    case UpdateJobStatusOutcome.AlreadyInThisStatus:
                    case UpdateJobStatusOutcome.Success:
                        return s.FriendlyName();
                    case UpdateJobStatusOutcome.BadRequest:
                        return StatusCode(400);
                    case UpdateJobStatusOutcome.Unauthorized:
                        return StatusCode(401);
                    default:
                        return StatusCode(500);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in SetRequestStatus", ex);
                return StatusCode(500);
            }
        }

        [AuthorizeAttributeNoRedirect]
        [HttpGet("get-job-details")]
        public async Task<IActionResult> GetJobDetails(string j, JobSet js, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            return ViewComponent("JobDetail", new { jobId, user, jobSet = js });
        }

        [AuthorizeAttributeNoRedirect]
        [HttpPost("get-filtered-jobs")]
        public async Task<IActionResult> GetFilteredJobs([FromBody]JobFilterRequest jobFilterRequest)
        {
            return ViewComponent("JobList", new { jobFilterRequest });
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-status-change-popup")]
        public async Task<IActionResult> GetStatusChangePopup(string j, JobStatuses s)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);

            return ViewComponent("JobStatusChangePopup", new { jobId, targetStatus = s });
        }
    }
}
