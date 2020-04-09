using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{

    [Route("api/[controller]")]
    [ApiController]    
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(IConfiguration configuration, ILogger<ConfigurationController> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        [HttpGet("{appSetting}")]
        public async Task<ActionResult> Get(string key)
        {
            _logger.LogInformation($"Loading Key {key} from App Settings");
            var parameterValue = _configuration[key];
            return Json(new { parameter = parameterValue });                        
        }
    }
}
