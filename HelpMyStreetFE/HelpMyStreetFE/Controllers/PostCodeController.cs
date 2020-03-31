using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCodeController : ControllerBase
    {
        private readonly ILogger<PostCodeController> _logger;
        private readonly IAddressService _addressService;

        public PostCodeController(ILogger<PostCodeController> logger, IAddressService addressService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        }

        [HttpGet("{postCode}")]
        public async Task<ActionResult<PostCodeResponse>> Get(string postCode)
        {
            _logger.LogInformation($"PostCode {postCode}");
            return await _addressService.CheckPostCode(postCode);
        }
    }
}
