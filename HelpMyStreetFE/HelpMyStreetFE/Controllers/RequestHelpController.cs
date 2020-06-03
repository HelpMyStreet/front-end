using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Interface;
using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{

    public class RequestHelpController : Controller
    {        
        private readonly ILogger<RequestHelpController> _logger;
        private readonly IRequestService _requestService;
        public RequestHelpController(ILogger<RequestHelpController> logger, IRequestService requestService)
        {                  
            _logger = logger;
            _requestService = requestService;
         }

        public IActionResult RequestHelpNew()
        {
            _logger.LogInformation("request-help");

            Models.RequestHelp.NewMVCForm.Models.RequestHelpNewViewModel model = new Models.RequestHelp.NewMVCForm.Models.RequestHelpNewViewModel
            {

                CurrentStepIndex = 0,
                Steps = new List<Models.RequestHelp.NewMVCForm.Interface.IRequestHelpStepsViewModel>
                {
                    new Models.RequestHelp.NewMVCForm.Models.RequestHelpRequestStageViewModel
                    {
                        Tasks = _requestService.GetRequestHelpTasks(), 
                        Requestors = new List<RequestorViewModel>
                        {
                            new RequestorViewModel
                            {
                                ID = 1,
                                ColourCode = "orange",
                                Title = "I am requesting help for myself",
                                Text = "I'm the person in need of help",
                                IconDark = "request-myself.svg",
                                IconLight = "request-myself-white.svg",
                            },
                            new RequestorViewModel
                            {
                                ID = 2,
                                ColourCode = "dark-blue",
                                Title = "On behalf of someone else",
                                Text = "I'm looking for help for a relative, neighbour or friend",
                                IconDark = "request-behalf.svg",
                                IconLight = "request-behalf-white.svg.svg",
                            }
                        },
                        Timeframes =  new List<RequestHelpTimeViewModel>
                        {
                            new RequestHelpTimeViewModel{ID = 1, TimeDescription = "Today", Days = 0},
                            new RequestHelpTimeViewModel{ID = 2, TimeDescription = "Within 24 Hours", Days = 1},
                            new RequestHelpTimeViewModel{ID = 3, TimeDescription = "Within a Week", Days = 7},
                            new RequestHelpTimeViewModel{ID = 4, TimeDescription = "When Convenient", Days = 30},
                            new RequestHelpTimeViewModel{ID = 5, TimeDescription = "Something Else", AllowCustom = true},
                        },
                    },  
                    
                    new Models.RequestHelp.NewMVCForm.Models.RequestHelpRequestStageViewModel
                    {
                        Tasks  = _requestService.GetRequestHelpTasks(),
                    }
                }
                
            };
            HttpContext.Session.SetObjectAsJson("request-help", model);
            return View("RequestHelpNew/RequestHelpNew", model);
        }

        [HttpPost]
        public ActionResult RequestHelpNew(        
        [ModelBinder(BinderType = typeof(Models.RequestHelp.NewMVCForm.Models.RequestHelpStepsViewModelBinder))] Models.RequestHelp.NewMVCForm.Interface.IRequestHelpStepsViewModel step)
        {
            RequestHelpNewViewModel requestHelp = HttpContext.Session.GetObjectFromJson<RequestHelpNewViewModel>("request-help");            
            requestHelp.Steps[requestHelp.CurrentStepIndex] = step;
            requestHelp.CurrentStepIndex++;
            if (ModelState.IsValid)
            {
              
            }
            HttpContext.Session.SetObjectAsJson("request-help", requestHelp);
            return View("RequestHelpNew/RequestHelpNew", requestHelp);
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
                button = " <a href='/registration/stepone' class='btn cta large fill mt16 btn--sign-up '>Sign up</a>";
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


        public async Task<ActionResult> Questions(int taskID)
        {         
            TasksViewModel model = _requestService.GetRequestHelpTasks().Where(x => x.ID == taskID).First();
            return PartialView("_Questions", model);
        }
    }
}