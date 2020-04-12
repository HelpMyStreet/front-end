using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using HelpMyStreetFE.Models.ContactForm;
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

        public ContactFormController(IOptions<EmailConfig> app)
        {
            appSettings = app;
        }

        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ContactForm/Send", Name = "SendEmail")]
        public IActionResult SendEmail(ContactFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var subject = "Help My Street: Web Enquiry";
                    var textContent = $"Name: {vm.Name} \r\nEmail: {vm.Email} \r\nMobile: {vm.MobileNumber} \r\nOther phone: {vm.OtherNumber} \r\nOrganisation: {vm.Organisation} \r\nRole: {vm.Role} \r\nMessage: {vm.Message}";
                    var htmlContent = $"<p>Name: {vm.Name} <br>Email: {vm.Email} <br>Mobile: {vm.MobileNumber} <br>Other phone: {vm.OtherNumber} <br>Organisation: {vm.Organisation} <br>Role: {vm.Role} <br>Message: {vm.Message}";

                    SendEmail(subject, textContent, htmlContent).Wait();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return PartialView("_ContactForm", vm);
        }

        async Task SendEmail(string Subject, string textContet, string htmlContent)
        {
            var apiKey = appSettings.Value.ApiKey;
            if (apiKey == string.Empty)
            {
                throw new Exception("SendGrid Api Key missing.");
            }

            var client = new SendGridClient(apiKey);
            var eml = new SendGridMessage()
            {
                From = new EmailAddress(appSettings.Value.FromEmail, appSettings.Value.FromName),
                Subject = Subject,
                PlainTextContent = textContet,
                HtmlContent = htmlContent
            };
            eml.AddTo(new EmailAddress(appSettings.Value.ToEmail, appSettings.Value.ToName));

            var response = await client.SendEmailAsync(eml);
        }

    }
}