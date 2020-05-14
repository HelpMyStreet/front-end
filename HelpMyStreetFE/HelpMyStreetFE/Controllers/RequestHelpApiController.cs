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

        public RequestHelpAPIController(ILogger<RequestHelpAPIController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           
        }

        [HttpPost]        
        public ActionResult RequestHelp([FromBody]RequestHelpViewModel model)
        {
            return StatusCode(500);
        }
    }
}
