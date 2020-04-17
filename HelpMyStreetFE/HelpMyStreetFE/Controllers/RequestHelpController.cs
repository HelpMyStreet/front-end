using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace HelpMyStreetFE.Controllers
{
    public class RequestHelpController : Controller
    {
        private readonly IOptions<EmailConfig> appSettings;
        private readonly IRequestHelpRepository _requestHelpRepository;

        public RequestHelpController(IOptions<EmailConfig> app, IRequestHelpRepository requestHelpRepository)
        {
            appSettings = app;
            _requestHelpRepository = requestHelpRepository;
        }

        public IActionResult RequestHelp()
        {
            var model = new RequestHelpFormModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/RequestHelp/Send", Name = "RequestHelpSendEmail")]
        public IActionResult SendEmail(RequestHelpFormModel vm)
        { 
            if (ModelState.IsValid)
            {
                try
                {
                    _requestHelpRepository.PersistForm(vm);

                    return View("Confirmation", vm);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return View("RequestHelp", vm);
        }
    }
}