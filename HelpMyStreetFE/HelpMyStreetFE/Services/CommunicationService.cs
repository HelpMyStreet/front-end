using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreet.Contracts.CommunicationService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class CommunicationService : BaseHttpRepository, ICommunicationService
    {
        private readonly ILogger<CommunicationService> _logger;
        public CommunicationService(
            ILogger<CommunicationService> logger,
            IConfiguration configuration,
            HttpClient client) : base(client, configuration, logger, "Services:Communication")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetLinkDestination(string token)
        {
            try
            {
                var response = await GetAsync<ResponseWrapper<GetLinkDestinationResponse, CommunicationServiceErrorCode>>($"/api/GetLinkDestination?token={token}");

                if (response.HasContent && response.IsSuccessful)
                {
                    return response.Content.Url;
                }
            }
            catch { }
            return null;
        }

        public async Task<bool> SendEmail(string subject, string textContent, string htmlContent, RecipientModel recipient)
        {
            var sendEmailRequest = new SendEmailRequest
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
            var interUserMessageRequest = new InterUserMessageRequest
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
                var interUserMessageResponse = JsonConvert.DeserializeObject<ResponseWrapper<bool, CommunicationServiceErrorCode>>(jsonResponse);
                if (interUserMessageResponse.HasContent && interUserMessageResponse.IsSuccessful)
                {
                    return interUserMessageResponse.Content;
                }
            }
            return false;
        }
    }
}
