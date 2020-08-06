using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Controllers { 


    [Route("api/requesthelp")]
    [ApiController]
    public class RequestHelpAPIController : ControllerBase
    {
        private readonly ILogger<RequestHelpAPIController> _logger;
        private readonly IRequestService _requestService;
        public RequestHelpAPIController(ILogger<RequestHelpAPIController> logger, IRequestService requestService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        }


        [Authorize]
        [HttpGet("set-request-status")]
        public async Task<ActionResult<bool>> SetRequestStatus(string j, JobStatuses s, string u)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var jobId = DecodeJobID(j);
                switch (s)
                {
                    case JobStatuses.InProgress:
                        int targetUserId = string.IsNullOrEmpty(u) ? userId : Convert.ToInt32(Base64Utils.Base64Decode(u));
                        return await _requestService.UpdateJobStatusToInProgressAsync(jobId, userId, targetUserId, HttpContext);
                    case JobStatuses.Done:
                        return await _requestService.UpdateJobStatusToDoneAsync(jobId, userId, HttpContext);
                    case JobStatuses.Cancelled:
                        return await _requestService.UpdateJobStatusToCancelledAsync(jobId, userId, HttpContext);
                    case JobStatuses.Open:
                        return await _requestService.UpdateJobStatusToOpenAsync(jobId, userId, HttpContext);
                    default:
                        throw new Exception($"Unexpected JobStatus {s}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in SetRequestStatus", ex);
                return StatusCode(500);
            }
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
