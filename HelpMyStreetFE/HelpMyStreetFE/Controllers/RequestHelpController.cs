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
        [Route("/RequestHelp/Send", Name = "RequestHelpSubmit")]
        public IActionResult SendEmail(RequestHelpFormModel requestHelpFormModel)
        { 
            if (ModelState.IsValid)
            {
                try
                {
                    _requestHelpRepository.PersistForm(requestHelpFormModel);

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