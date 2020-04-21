using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

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
            _logger.LogInformation("Request Help");

            var model = new RequestHelpFormModel
            {
                HasErrors = false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/RequestHelp/Send", Name = "RequestHelpSubmit")]
        public IActionResult SendEmail(RequestHelpFormModel requestHelpFormModel)
        { 
            if (ModelState.IsValid)
            {
                try
                {
                    _requestService.UpdateRequest(requestHelpFormModel);

                    return View("Confirmation", requestHelpFormModel);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            requestHelpFormModel.HasErrors = true;
            return View("RequestHelp", requestHelpFormModel);
        }
    }
}