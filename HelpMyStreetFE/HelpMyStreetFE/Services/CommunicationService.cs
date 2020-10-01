using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.CommunicationService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreetFE.Models.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class CommunicationService : BaseHttpService, ICommunicationService
    {
        private readonly ILogger<CommunicationService> _logger;
        public CommunicationService(
            ILogger<CommunicationService> logger,
            IConfiguration configuration,
            HttpClient client) : base(client, configuration, "Services:Communication")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetLinkDestination(string token)
        {
            return "/Feedback/PostTaskFeedbackCapture?j=MzU5&r=Mg==";
        }

        public async Task<bool> SendEmail(string subject, string textContent, string htmlContent, RecipientModel recipient)
        {
            SendEmailRequest sendEmailRequest = new SendEmailRequest()
            {
                BodyHTML = htmlContent,
                BodyText = textContent,
                Subject = subject,
                ToAddress = recipient.Email,
                ToName = recipient.Name
            };

            string json = JsonConvert.SerializeObject(sendEmailRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            
            using (HttpResponseMessage response = await Client.PostAsync("/api/SendEmail", data))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var sendEmailResponse = JsonConvert.DeserializeObject<ResponseWrapper<SendEmailResponse, CommunicationServiceErrorCode>>(jsonResponse);
                if (sendEmailResponse.HasContent && sendEmailResponse.IsSuccessful)
                {
                    return sendEmailResponse.Content.Success;
                }
            }
            return false;
        }

        public async Task<bool> SendInterUserMessage(MessageParticipant from, MessageParticipant to, string message, int? jobId)
        {
            var interUserMessageRequest = new InterUserMessageRequest()
            {
                From = from,
                To = to,
                Content = message,
                JobId = jobId,
            };

            string json = JsonConvert.SerializeObject(interUserMessageRequest);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await Client.PostAsync("/api/InterUserMessage", data))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                // What here?
                if (true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
