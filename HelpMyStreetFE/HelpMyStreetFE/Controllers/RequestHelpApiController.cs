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
using HelpMyStreetFE.Enums.Account;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

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


        [Authorize]
        [HttpGet("set-job-status")]
        public async Task<ActionResult<bool>> SetJobStatus(string j, JobStatuses s, string u, CancellationToken cancellationToken)
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

                return await _requestService.UpdateJobStatusAsync(jobId, s, userId, targetUserId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in SetRequestStatus", ex);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpGet("get-job-details")]
        public async Task<IActionResult> GetJobDetails(string j)
        {
            var jobId = DecodeJobID(j);
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return ViewComponent("JobDetail", new { jobId, userId });

        }

        [Authorize]
        [HttpPost("get-filtered-jobs")]
        public async Task<IActionResult> GetFilteredJobs([FromBody]JobFilterRequest jobFilterRequest)
        {
            var referrerComponents = Request.Headers["Referer"].ToString().Split('/');

            JobSet jobSet;
            int? groupId = null;
            switch (referrerComponents[4])
            {
                case "accepted-requests": jobSet = JobSet.UserAcceptedRequests; break;
                case "completed-requests": jobSet = JobSet.UserCompletedRequests; break;
                case "open-requests": jobSet = JobSet.UserOpenRequests_NotMatchingCriteria; break;
                case "g":
                    jobSet = JobSet.GroupRequests;
                    groupId = await _groupService.GetGroupIdByKey(referrerComponents[5]);
                    break;
                default:
                    throw new Exception($"Unexpected URL component: {referrerComponents[4]}");
            }

            return ViewComponent("JobList", new { jobSet, groupId, jobFilterRequest });
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
