using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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

        [HttpPost]        
        public async Task<ActionResult<LogRequestResponse>> RequestHelp([FromBody] RequestHelpViewModel model)
        {
            try
            {
                return await _requestService.LogRequestAsync(model);
            }catch(Exception ex)
            {
                _logger.LogError("an error occured requesting help", ex);
                return StatusCode(500);
            }            
        }
    }
}
