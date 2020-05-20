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
        public async Task<ActionResult<bool>> AcceptRequest([FromBody]UpdateJobRequest Job)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                int jobId = -1;
                int.TryParse(Base64Utils.Base64Decode(Job.JobID.Replace("job-", "")), out jobId);
                return await _requestService.UpdateJobStatusToOpenAsync(jobId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError("an error occured accepting help", ex);
                return StatusCode(500);
            }
        }


        [HttpPost]        
        public async Task<ActionResult<BaseRequestHelpResponse<LogRequestResponse>>> RequestHelp([FromBody] RequestHelpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Server side model validation failed");
                
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
