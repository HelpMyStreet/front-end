using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{

    public class RequestHelpController : Controller
    {
        private readonly IOptions<EmailConfig> appSettings;
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestHelpController> _logger;
    
        public RequestHelpController(ILogger<RequestHelpController> logger,
             IOptions<EmailConfig> app,
            IRequestService requestService)
        {
            appSettings = app;
            _requestService = requestService;
            _logger = logger;
           }

        public IActionResult RequestHelp()
        {
            _logger.LogInformation("request-help");
            return View();
        }            

    }
}