using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
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
        [HttpPost("accept-request")]
        public async Task<ActionResult<bool>> AcceptRequest([FromBody]UpdateJobRequest job)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(job);

                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);                             
                return await _requestService.UpdateJobStatusToInProgressAsync(DecodeJobID(job.JobID), userId, userId, HttpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError("an error occured accepting help", ex);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPost("complete-request")]
        public async Task<ActionResult<bool>> CompleteRequest([FromBody]UpdateJobRequest job)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(job);

                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return await _requestService.UpdateJobStatusToDoneAsync(DecodeJobID(job.JobID), userId, HttpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError("an error occured completing job", ex);
                return StatusCode(500);
            }
        }
        
        [Authorize]
        [HttpPost("release-request")]
        public async Task<ActionResult<bool>> ReleaseRequest([FromBody]UpdateJobRequest job)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(job);

                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return await _requestService.UpdateJobStatusToOpenAsync(DecodeJobID(job.JobID), userId, HttpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError("an error occured releasing job", ex);
                return StatusCode(500);
            }
        }

        private int DecodeJobID(string encodedJobId)
        {
            int jobId;
            if (!int.TryParse(Base64Utils.Base64Decode(encodedJobId), out jobId))
            {
                throw new Exception("Could not decode Job ID: " + encodedJobId);
            }
            return jobId;

        }

        [HttpPost]        
        public async Task<ActionResult<BaseRequestHelpResponse<LogRequestResponse>>> RequestHelp([FromBody] RequestHelpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(model);

                int userId = 0;
                    if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
                    {
                        userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                    return await _requestService.LogRequestAsync(model, userId);                
            }catch(Exception ex)
            {
                _logger.LogError("an error occured requesting help", ex);
                return StatusCode(500);
            }            
        }
    }
}
