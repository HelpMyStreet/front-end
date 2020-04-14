using HelpMyStreetFE.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
   public interface IEmailService
    {
        Task<bool> SendEmail(string Subject, string textContet, string htmlContent, List<RecipientModel> recipients);
    }
    
}
