using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace HelpMyStreetFE.Controllers
{
    public class RequestHelpController : Controller
    {
        private readonly IOptions<EmailConfig> appSettings;
        private readonly IEmailService _emailService;
        private readonly IRequestHelpRepository _requestHelpRepository;

        public RequestHelpController(IOptions<EmailConfig> app, IEmailService emailService, IRequestHelpRepository requestHelpRepository)
        {
            appSettings = app;
            _emailService = emailService;
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

                    var subject = "Help My Street: Request For Help";
                    var textContent = $"Name: {vm.FirstName} {vm.LastName} \r\nEmail: {vm.Email} \r\nPhone: {vm.PhoneNumber} \r\nMessage: {vm.Message}";

                    var htmlContent = $"<p>Name: {vm.FirstName} {vm.LastName} <br>Email: {vm.Email} <br>Phone: {vm.PhoneNumber} <br>Message: {vm.Message}";

                    if (vm.HelpNeeded.Count > 0)
                    {
                        textContent += "\r\nHelp needed with: ";

                        htmlContent += "<br>Help needed with: ";

                        foreach (var helpOption in vm.HelpNeeded)
                        {
                            textContent += "\r\n{helpOption}";
                            htmlContent += "<br>{helpOption}";
                        }
                    }

                    List<RecipientModel> recipients = new List<RecipientModel>
                    {
                        new RecipientModel
                        {
                            Email = appSettings.Value.ToEmail,
                            Name = appSettings.Value.ToName
                        }
                    };

                   // _emailService.SendEmail(subject, textContent, htmlContent, recipients).Wait();
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