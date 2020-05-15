using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{

    public class RequestHelpController : Controller
    {        
        private readonly ILogger<RequestHelpController> _logger;    
        public RequestHelpController(ILogger<RequestHelpController> logger)
        {                  
            _logger = logger;
         }

        public IActionResult RequestHelp()
        {
            _logger.LogInformation("request-help");           
            return View();
        }

        public IActionResult Success()
        {
            List<NotificationModel> notifications = new List<NotificationModel> {
            new NotificationModel
            {
                Title = "Thank you!",
                Type = Enums.Account.NotificationType.Success,
                Message = "We'll do what we can to find someone who can help in that area if we're succesful then we'll let you know - or a local volunteer may contact you directly (if you gave us permission to do those things)."
            }
            };
        

            return View(notifications);
        }

    }
}