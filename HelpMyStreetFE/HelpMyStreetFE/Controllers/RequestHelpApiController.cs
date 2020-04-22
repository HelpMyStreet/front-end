using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RequestHelpApiController : ControllerBase
	{
		private readonly IOptions<EmailConfig> appSettings;
		private readonly IRequestService _requestService;
		private readonly ILogger<RequestHelpApiController> _logger;

		public RequestHelpApiController(ILogger<RequestHelpApiController> logger,
			 IOptions<EmailConfig> app,
			IRequestService requestService)
		{
			appSettings = app;
			_requestService = requestService;
			_logger = logger;
		}

		[HttpGet("logRequest/{postCode}")]
		public async Task<ActionResult<LogRequestResponse>> LogRequest(string postCode)
		{
			_logger.LogInformation($"PostCode {postCode}");

			return await _requestService.LogRequestAsync(postCode);
		}
	}
}