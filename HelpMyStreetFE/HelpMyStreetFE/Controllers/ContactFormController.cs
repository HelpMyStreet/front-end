using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using HelpMyStreetFE.Models.ContactForm;
using HelpMyStreetFE.Models.Email;
using SendGrid;
using SendGrid.Helpers.Mail;
using HelpMyStreetFE.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HelpMyStreetFE.Controllers
{
    public class ContactFormController : Controller
    {
        private readonly IOptions<EmailConfig> appSettings;
        private readonly IEmailService _emailService;

        public ContactFormController(IOptions<EmailConfig> app, IEmailService emailService)
        {
            appSettings = app;
            _emailService = emailService;
        }

        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ContactForm/Send", Name = "SendEmail")]
        public  IActionResult SendEmail(ContactFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var subject = "Help My Street: Web Enquiry";
                    var textContent = $"Name: {vm.Name} \r\nEmail: {vm.Email} \r\nMobile: {vm.MobileNumber} \r\nOther phone: {vm.OtherNumber} \r\nOrganisation: {vm.Organisation} \r\nRole: {vm.Role} \r\nMessage: {vm.Message}";
                    var htmlContent = $"<p>Name: {vm.Name} <br>Email: {vm.Email} <br>Mobile: {vm.MobileNumber} <br>Other phone: {vm.OtherNumber} <br>Organisation: {vm.Organisation} <br>Role: {vm.Role} <br>Message: {vm.Message}";

                    List<RecipientModel> recipients = new List<RecipientModel>
                    {
                        new RecipientModel
                        {
                            Email = appSettings.Value.ToEmail,
                            Name = appSettings.Value.ToName
                        }
                    };
                    _emailService.SendEmail(subject, textContent, htmlContent, recipients).Wait();                    
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return PartialView("_ContactForm", vm);
        }   
    }
}