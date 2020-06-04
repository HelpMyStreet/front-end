using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Helpers.CustomModelBinder;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
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
        

        [HttpPost]
        public ActionResult RequestHelp(
        [ModelBinder(BinderType = typeof(RequestHelpModelBinder))]RequestHelpNewViewModel requestHelp,
        [ModelBinder(BinderType = typeof(RequestHelpStepsViewModelBinder))] IRequestHelpStageViewModel step)
        {            
            requestHelp.Steps[requestHelp.CurrentStepIndex] = step;
            if(requestHelp.Action == "back")
            {
                requestHelp.CurrentStepIndex--;
                return View(requestHelp);
            }
            
            if (ModelState.IsValid)
            {
                if (requestHelp.Action == "next")
                {
                    requestHelp.CurrentStepIndex++;
                    if (step is RequestHelpRequestStageViewModel)
                    {
                        var requestStep = (RequestHelpRequestStageViewModel)step;
                        var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).First();
                        detailStage.Type = requestStep.Requestors.Where(x => x.IsSelected).First().Type;
                    }
                }

                if(requestHelp.Action == "finish")
                {
                    // call api;
                }
                
            }            
            return View(requestHelp);
        }
    

    public IActionResult RequestHelp()
        {
            _logger.LogInformation("request-help");
             var model = _requestService.GetRequestHelpSteps();
            return View(model);
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

        [HttpPost]
        public async Task<ActionResult> Questions([FromBody]QuestionRequest request)
        {
            TasksViewModel model = request.Step.Tasks.Where(x => x.ID == request.TaskID).First();
            return PartialView("_Questions", model);
        }
        
        public class QuestionRequest
        {
            public RequestHelpRequestStageViewModel Step { get; set; }
            public int TaskID { get; set; }
        }
    }
}