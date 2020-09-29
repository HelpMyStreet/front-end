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
        Task<bool> SendInterUserMessage(string content);
    }
}
