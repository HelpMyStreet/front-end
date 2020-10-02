using System;
using Microsoft.AspNetCore.Mvc;
using HelpMyStreetFE.Models.ContactForm;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Services;
using Microsoft.Extensions.Options;

namespace HelpMyStreetFE.Controllers
{
    public class ContactFormController : Controller
    {
        private readonly IOptions<EmailConfig> appSettings;
        private readonly ICommunicationService _communicationService;

        public ContactFormController(IOptions<EmailConfig> app, ICommunicationService communicationService)
        {
            appSettings = app;
            _communicationService = communicationService;
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

                    _communicationService.SendEmail(subject, textContent, htmlContent, new RecipientModel() { Email = appSettings.Value.ToEmail, Name = appSettings.Value.ToName }).Wait();                    
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return PartialView("_ContactForm", vm);
        }   
    }
}