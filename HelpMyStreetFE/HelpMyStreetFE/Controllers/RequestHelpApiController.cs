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
using HelpMyStreetFE.Services;
using System.Net;

namespace HelpMyStreetFE.Controllers {


    [Route("api/request-help")]
    [ApiController]
    public class RequestHelpAPIController : Controller
    {
        private readonly ILogger<RequestHelpAPIController> _logger;
        private readonly IRequestService _requestService;
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;

        public RequestHelpAPIController(ILogger<RequestHelpAPIController> logger, IRequestService requestService, IAuthService authService, IFeedbackService feedbackService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }


        [AuthorizeAttributeNoRedirect]
        [HttpGet("set-job-status")]
        public async Task<ActionResult<SetJobStatusResult>> SetJobStatus(string j, string rq, JobStatuses s, string r, string u, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

                UpdateJobStatusOutcome? outcome;
                bool requestFeedback = false;

                if (string.IsNullOrEmpty(j))
                {
                    int requestId = Base64Utils.Base64DecodeToInt(rq);
                    outcome = await _requestService.UpdateRequestStatusAsync(requestId, s, user.ID, cancellationToken);
                }
                else
                {
                    RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);
                    int? targetUserId = null;
                    if (s == JobStatuses.InProgress)
                    { 
                        targetUserId = (requestRole == RequestRoles.Volunteer ? user.ID : Base64Utils.Base64DecodeToInt(u));
                    }

                    int jobId = Base64Utils.Base64DecodeToInt(j);
                    outcome = await _requestService.UpdateJobStatusAsync(jobId, s, user.ID, targetUserId, cancellationToken);
                    requestFeedback = (await GetJobFeedbackStatus(jobId, user.ID, requestRole, cancellationToken)).FeedbackDue;
                }

                switch (outcome)
                {
                    case UpdateJobStatusOutcome.AlreadyInThisStatus:
                    case UpdateJobStatusOutcome.Success:
                        return new SetJobStatusResult
                        {
                            NewStatus = s.FriendlyName(),
                            RequestFeedback = requestFeedback
                        };
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
        public async Task<IActionResult> GetJobDetails(string j, string rq, JobSet js, CancellationToken cancellationToken)
        {
            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            if (string.IsNullOrEmpty(j))
            {
                int requestId = Base64Utils.Base64DecodeToInt(rq);
                return ViewComponent("RequestDetail", new { requestId, user, jobSet = js });
            }
            else
            {
                int jobId = Base64Utils.Base64DecodeToInt(j);
                return ViewComponent("JobDetail", new { jobId, user, jobSet = js });
            }
        }

        [AuthorizeAttributeNoRedirect]
        [HttpPost("get-filtered-jobs")]
        public IActionResult GetFilteredJobs([FromBody]JobFilterRequest jobFilterRequest)
        {
            return ViewComponent("JobList", new { jobFilterRequest });
        }

        [AuthorizeAttributeNoRedirect]
        [Route("get-status-change-popup")]
        public IActionResult GetStatusChangePopup(string j, string rq, JobStatuses s)
        {
            if (string.IsNullOrEmpty(j))
            {
                int requestId = Base64Utils.Base64DecodeToInt(rq);
                return ViewComponent("JobStatusChangePopup", new { requestId, targetStatus = s });
            }
            else
            {
                int jobId = Base64Utils.Base64DecodeToInt(j);
                return ViewComponent("JobStatusChangePopup", new { jobId, targetStatus = s });
            }
        }

        [AuthorizeAttributeNoRedirect]
        [HttpGet("get-feedback-component")]
        public async Task<IActionResult> GetFeedbackComponent(string j, string r, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            var user = await _authService.GetCurrentUser(HttpContext, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var feedbackStatus = await GetJobFeedbackStatus(jobId, user.ID, requestRole, cancellationToken);

            if (feedbackStatus.FeedbackDue)
            {
                return PartialView("_FeedbackDue");
            }
            else if (feedbackStatus.FeedbackSubmitted)
            {
                return PartialView("_FeedbackSubmitted");
            }
            return StatusCode((int)HttpStatusCode.OK);
        }

        private async Task<JobFeedbackStatus> GetJobFeedbackStatus(int jobId, int userId, RequestRoles role, CancellationToken cancellationToken)
        {
            var job = await _requestService.GetJobSummaryAsync(jobId, cancellationToken);

            if (job.JobStatus == JobStatuses.Done || job.JobStatus == JobStatuses.Cancelled)
            {
                bool feedbackSubmitted = await _feedbackService.GetFeedbackExists(jobId, role, userId);

                if (feedbackSubmitted)
                {
                    return new JobFeedbackStatus
                    {
                        FeedbackSubmitted = true,
                        FeedbackDue = false,
                    };
                }
                else if (!(job.Archive ?? false))
                {
                    _authService.PutSessionAuthorisedUrl(HttpContext, $"/api/feedback/get-post-task-feedback-popup?j={Base64Utils.Base64Encode(jobId)}&r={Base64Utils.Base64Encode((int)role)}");
                    _authService.PutSessionAuthorisedUrl(HttpContext, $"/api/feedback/put-feedback?j={Base64Utils.Base64Encode(jobId)}&r={Base64Utils.Base64Encode((int)role)}");
                    
                    return new JobFeedbackStatus
                    {
                        FeedbackSubmitted = false,
                        FeedbackDue = true,
                    };
                }
            }

            return new JobFeedbackStatus
            {
                FeedbackSubmitted = false,
                FeedbackDue = false,
            };
        }
    }

    class JobFeedbackStatus
    {
        public bool FeedbackDue { get; set; }
        public bool FeedbackSubmitted { get; set; }
    }
}
