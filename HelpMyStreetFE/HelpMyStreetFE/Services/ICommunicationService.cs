using HelpMyStreet.Contracts.CommunicationService.Request;
using HelpMyStreetFE.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface ICommunicationService
    {
        Task<bool> SendEmail(string subject, string textContent, string htmlContent, RecipientModel recipient);
        Task<bool> SendInterUserMessage(MessageParticipant from, MessageParticipant to, string message, int? jobId = null);
        Task<bool> RequestCommunication(int? groupID, int? recipientID, int? jobID, CommunicationJob communicationJob);
        Task<string> GetLinkDestination(string token);
    }
}
