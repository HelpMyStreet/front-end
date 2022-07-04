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
using HelpMyStreetFE.Services;
using System.Net;
using System.Collections.Generic;

namespace HelpMyStreetFE.Controllers {


    [Route("api/request-help")]
    [ApiController]
    public class RequestHelpAPIController : Controller
    {
        private readonly ILogger<RequestHelpAPIController> _logger;
        private readonly IRequestService _requestService;
        private readonly IJobCachingService _jobCachingService;
        private readonly IRequestUpdatingService _requestUpdatingService;
        private readonly IAuthService _authService;
        private readonly IFeedbackService _feedbackService;

        public RequestHelpAPIController(
            ILogger<RequestHelpAPIController> logger, 
            IRequestService requestService,
            IJobCachingService jobCachingService,
            IAuthService authService, 
            IFeedbackService feedbackService, 
            IRequestUpdatingService requestUpdatingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _jobCachingService = jobCachingService ?? throw new ArgumentNullException(nameof(jobCachingService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _requestUpdatingService = requestUpdatingService ?? throw new ArgumentNullException(nameof(requestUpdatingService));
        }


        [AuthorizeAttributeNoRedirect]
        [HttpGet("set-job-status")]
        public async Task<ActionResult<SetJobStatusResult>> SetJobStatus(string j, string rq, JobStatuses s, string r, string u, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetCurrentUser(cancellationToken);

                UpdateJobStatusOutcome? outcome;
                bool requestFeedback = false;

                if (string.IsNullOrEmpty(j))
                {
                    int requestId = Base64Utils.Base64DecodeToInt(rq);
                    outcome = await _requestUpdatingService.UpdateRequestStatusAsync(requestId, s, user.ID, cancellationToken);
                }
                else
                {
                    RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);
                    int? targetUserId = null;
                    if (s == JobStatuses.Accepted || s == JobStatuses.InProgress || s== JobStatuses.AppliedFor)
                    { 
                        targetUserId = (requestRole == RequestRoles.Volunteer ? user.ID : Base64Utils.Base64DecodeToInt(u));
                    }

                    int jobId = Base64Utils.Base64DecodeToInt(j);
                    outcome = await _requestUpdatingService.UpdateJobStatusAsync(jobId, s, user.ID, targetUserId, cancellationToken);

                    var job = await _jobCachingService.GetJobBasicAsync(jobId, cancellationToken);
                    if (job.RequestType.Equals(RequestType.Task))
                    {
                        requestFeedback = (await GetJobFeedbackStatus(jobId, user.ID, requestRole, cancellationToken, s)).FeedbackDue;
                    }
                }

                switch (outcome)
                {
                    case UpdateJobStatusOutcome.AlreadyInThisStatus:
                    case UpdateJobStatusOutcome.Success:
                        return new SetJobStatusResult
                        {
                            NewStatus = s.FriendlyName(),
                            RequestFeedback = requestFeedback,
                            LockQuestions = (s.Complete()),
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
            var user = await _authService.GetCurrentUser(cancellationToken);

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
        [Route("get-accept-job-series-popup")]
        public IActionResult GetStatusChangePopup(string rq, int stg)
        {
            int requestId = Base64Utils.Base64DecodeToInt(rq);
            return ViewComponent("AcceptJobSeriesPopup", new { requestId, stage = stg });
        }

        [AuthorizeAttributeNoRedirect]
        [HttpGet("get-feedback-component")]
        public async Task<IActionResult> GetFeedbackComponent(string j, string r, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);
            RequestRoles requestRole = (RequestRoles)Base64Utils.Base64DecodeToInt(r);

            var user = await _authService.GetCurrentUser(cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var feedbackStatus = await GetJobFeedbackStatus(jobId, user.ID, requestRole, cancellationToken, null);

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

        [AuthorizeAttributeNoRedirect]
        [Route("get-view-location-popup")]
        public IActionResult GetViewLocationPopup(string r)
        {
            try
            {
                var requestId = Base64Utils.Base64DecodeToInt(r);
                return ViewComponent("ViewLocationPopup", new { requestId });
            } catch (Exception e) {
                throw new Exception("Unable to generate location popup", e);
            }
        }

        [AuthorizeAttributeNoRedirect]
        [HttpPost("update-job-question")]
        public async Task<ActionResult<string>> UpdateJobQuestion(string j, string q, [FromBody] Dictionary<string, string> body, CancellationToken cancellationToken)
        {
            int jobId = Base64Utils.Base64DecodeToInt(j);
            int questionId = Base64Utils.Base64DecodeToInt(q);
            string answer = body[$"currentStep.Questions.[{questionId}].Model"];

            var user = await _authService.GetCurrentUser(cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            var outcome = await _requestUpdatingService.UpdateJobQuestion(jobId, questionId, answer, user.ID, cancellationToken);

            switch (outcome)
            {
                case UpdateJobOutcome.AlreadyInThisState:
                case UpdateJobOutcome.Success:
                    return answer.ToHtmlSafeStringWithLineBreaks();
                case UpdateJobOutcome.BadRequest:
                    return StatusCode(400);
                case UpdateJobOutcome.Unauthorized:
                    return StatusCode(401);
                default:
                    return StatusCode(500);
            }
        }

        private async Task<JobFeedbackStatus> GetJobFeedbackStatus(int jobId, int userId, RequestRoles role, CancellationToken cancellationToken, JobStatuses? newJobStatus)
        {
            var job = await _jobCachingService.GetJobSummaryAsync(jobId, cancellationToken);

            if ((newJobStatus == JobStatuses.Done || newJobStatus == JobStatuses.Cancelled) || 
                (newJobStatus == null && (job.JobStatus == JobStatuses.Done || job.JobStatus == JobStatuses.Cancelled)))
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
                else if (!job.Archive && (role.Equals(RequestRoles.GroupAdmin) || job.VolunteerUserID.Equals(userId)))
                {
                    _authService.PutSessionAuthorisedUrl($"/api/feedback/get-post-task-feedback-popup?j={Base64Utils.Base64Encode(jobId)}&r={Base64Utils.Base64Encode((int)role)}");
                    _authService.PutSessionAuthorisedUrl($"/api/feedback/put-feedback?j={Base64Utils.Base64Encode(jobId)}&r={Base64Utils.Base64Encode((int)role)}");
                    
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
