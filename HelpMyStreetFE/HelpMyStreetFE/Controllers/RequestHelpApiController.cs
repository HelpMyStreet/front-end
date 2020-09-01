using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Account.Jobs;
using System.Threading;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreet.Utils.Extensions;

namespace HelpMyStreetFE.Controllers {


    [Route("api/requesthelp")]
    [ApiController]
    public class RequestHelpAPIController : Controller
    {
        private readonly ILogger<RequestHelpAPIController> _logger;
        private readonly IRequestService _requestService;
        private readonly IGroupService _groupService;

        public RequestHelpAPIController(ILogger<RequestHelpAPIController> logger, IRequestService requestService, IGroupService groupService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _groupService = groupService;
        }


        [AuthorizeAttributeNoRedirect]
        [HttpGet("set-job-status")]
        public async Task<ActionResult<string>> SetJobStatus(string j, JobStatuses s, string u, CancellationToken cancellationToken)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var jobId = DecodeJobID(j);

                int? targetUserId = null;
                if (s == JobStatuses.InProgress)
                {
                    targetUserId = string.IsNullOrEmpty(u) ? userId : Convert.ToInt32(Base64Utils.Base64Decode(u));
                }

                bool success = await _requestService.UpdateJobStatusAsync(jobId, s, userId, targetUserId, cancellationToken);

                if (success)
                {
                    return s.FriendlyName();
                }
                else
                {
                    return StatusCode(400);
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
        public async Task<IActionResult> GetJobDetails(string j)
        {
            var jobId = DecodeJobID(j);

            User user = HttpContext.Session.GetObjectFromJson<User>("User");

            if (user == null)
            {
                throw new UnauthorizedAccessException("No user in session");
            }

            return ViewComponent("JobDetail", new { jobId, user });
        }

        [AuthorizeAttributeNoRedirect]
        [HttpPost("get-filtered-jobs")]
        public async Task<IActionResult> GetFilteredJobs([FromBody]JobFilterRequest jobFilterRequest)
        {
            return ViewComponent("JobList", new { jobFilterRequest });
        }

        private int DecodeJobID(string encodedJobId)
        {
            if (!int.TryParse(Base64Utils.Base64Decode(encodedJobId), out int jobId))
            {
                throw new Exception("Could not decode Job ID: " + encodedJobId);
            }
            return jobId;
        }
    }
}
