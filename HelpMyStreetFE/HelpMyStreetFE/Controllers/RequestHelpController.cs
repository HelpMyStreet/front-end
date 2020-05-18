using HelpMyStreet.Contracts.RequestService.Response;
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

        public IActionResult Success(Fulfillable fulfillable, bool onBehalf)
        {
        
            string message = "<p>Your request will be passed to a local volunteer who should get in touch shortly.</p>";
            string button = " <a href='/' class='btn cta large fill mt16 btn--request-help cta--orange'>Done</a>";                   
            
 
            if (fulfillable == Fulfillable.Accepted_ManualReferral || fulfillable == Fulfillable.Rejected_Unfulfillable)
            {
                message = "<p>We’ve just launched HelpMyStreet and we’re building our network across the country. We’re working hard to ensure we have local volunteers in your area who can get the right help to the right people. You’ll be contacted soon to progress your request.</p>";                
            }            

            if (onBehalf)
            {
                message += "<p>Are you Volunteering in your local area? Sign up as a Street Champion or Helper to help and support local people shelter safely at home </p>";
                button = " <a href='registration/stepone' class='btn cta large fill mt16 btn--sign-up '>Sign up</a>";
            }

            List<NotificationModel> notifications = new List<NotificationModel> {
            new NotificationModel
            {
                Title = "Thank you",
                Subtitle = "Your request has been received",
                Type = Enums.Account.NotificationType.Success,
                Message = message,
                Button = button
            }
            };
        
            return View(notifications);
        }

    }
}