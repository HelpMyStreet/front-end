using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Email;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailConfig> appSettings;
        public EmailService(IOptions<EmailConfig> app)
        {
            appSettings = app;
        }
        public async Task<bool> SendEmail(string Subject, string textContet, string htmlContent, List<RecipientModel> recipients)
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
            foreach(var recipient in recipients)
            {
                eml.AddTo(new EmailAddress(recipient.Email, recipient.Name));
            }            
            var response = await client.SendEmailAsync(eml);
            return response.StatusCode == System.Net.HttpStatusCode.OK ? true : false;
        }
    }
}
