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

        [Route("/RequestHelpA", Name = "RequestHelpA")]
        public IActionResult RequestHelp()
        {
            _logger.LogInformation("RequestHelpA");

            var model = new RequestHelpFormModel
            {
                HasErrors = false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/RequestHelpB", Name = "RequestHelpB")]
        public async Task<IActionResult> SendEmail(RequestHelpFormModel requestHelpFormModel)
        {
            _logger.LogInformation("RequestHelpB");


            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _requestService.UpdateRequest(requestHelpFormModel);

                    return View("Confirmation", requestHelpFormModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError("RequestHelpB", ex);

                    requestHelpFormModel.HasErrors = true;
                    return View("RequestHelp", requestHelpFormModel);
                }
            }

            requestHelpFormModel.HasErrors = true;
            return View("RequestHelp", requestHelpFormModel);
        }
    }
}