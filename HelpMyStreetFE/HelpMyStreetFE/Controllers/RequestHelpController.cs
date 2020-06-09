using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Helpers.CustomModelBinder;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RequestHelp(
        [ModelBinder(BinderType = typeof(RequestHelpModelBinder))]RequestHelpViewModel requestHelp,
        [ModelBinder(BinderType = typeof(RequestHelpStepsViewModelBinder))] IRequestHelpStageViewModel step)
        {
            try
            {
                requestHelp.Steps[requestHelp.CurrentStepIndex] = step;
                if (requestHelp.Action == "EditRequest")
                {
                    requestHelp.CurrentStepIndex = 0;
                    return View(requestHelp);
                }
                if (requestHelp.Action == "EditDetails")
                {
                    requestHelp.CurrentStepIndex = 1;
                    return View(requestHelp);
                }
                if (requestHelp.Action == "back")
                {
                    requestHelp.CurrentStepIndex--;
                    return View(requestHelp);
                }

                if (!ModelState.IsValid) 
                    throw new ValidationException("Model Validation failed");
                

                    if (requestHelp.Action == "next")
                    {
                        requestHelp.CurrentStepIndex++;
                        if (step is RequestHelpRequestStageViewModel)
                        {

                            var requestStep = (RequestHelpRequestStageViewModel)step;
                            var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).First();
                            
                            if (requestStep.Tasks.Where(x => x.IsSelected).First().SupportActivity == HelpMyStreet.Utils.Enums.SupportActivities.FaceMask)
                            {
                                detailStage.ShowOtherDetails = false;                              
                            }                            

                            detailStage.Type = requestStep.Requestors.Where(x => x.IsSelected).First().Type;                            
                            if (HttpContext.Session.Keys.Contains("User"))
                            {
                                var loggedInUser = HttpContext.Session.GetObjectFromJson<User>("User");
                                switch (detailStage.Type)
                                {


                                    case RequestorType.Myself:
                                    if (detailStage.Recipient == null)
                                    {
                                        detailStage.Recipient = new RecipientDetails
                                        {
                                            Firstname = loggedInUser.UserPersonalDetails.FirstName,
                                            Lastname = loggedInUser.UserPersonalDetails.LastName,
                                            AddressLine1 = loggedInUser.UserPersonalDetails.Address.AddressLine1,
                                            AddressLine2 = loggedInUser.UserPersonalDetails.Address.AddressLine2,
                                            AlternatePhoneNumber = loggedInUser.UserPersonalDetails.OtherPhone,
                                            MobileNumber = loggedInUser.UserPersonalDetails.MobilePhone,
                                            Email = loggedInUser.UserPersonalDetails.EmailAddress,
                                            Postcode = loggedInUser.UserPersonalDetails.Address.Postcode,
                                            Town = loggedInUser.UserPersonalDetails.Address.Locality
                                        };
                                    }
                                        break;
                                    case RequestorType.OnBehalf:
                                    if (detailStage.Requestor == null)
                                    {
                                        detailStage.Requestor = new RequestorDetails
                                        {
                                            Firstname = loggedInUser.UserPersonalDetails.FirstName,
                                            Lastname = loggedInUser.UserPersonalDetails.LastName,
                                            AlternatePhoneNumber = loggedInUser.UserPersonalDetails.OtherPhone,
                                            MobileNumber = loggedInUser.UserPersonalDetails.MobilePhone,
                                            Email = loggedInUser.UserPersonalDetails.EmailAddress,
                                            Postcode = loggedInUser.UserPersonalDetails.Address.Postcode,
                                        };
                                    }
                                        break;
                                case RequestorType.Organisation:
                                    if (detailStage.OrganisationRequestor == null)
                                    {
                                        detailStage.OrganisationRequestor = new OrganisationDetails
                                        {
                                            Firstname = loggedInUser.UserPersonalDetails.FirstName,
                                            Lastname = loggedInUser.UserPersonalDetails.LastName,
                                            AlternatePhoneNumber = loggedInUser.UserPersonalDetails.OtherPhone,
                                            MobileNumber = loggedInUser.UserPersonalDetails.MobilePhone,
                                            Email = loggedInUser.UserPersonalDetails.EmailAddress,
                                            Postcode = loggedInUser.UserPersonalDetails.Address.Postcode,
                                        };
                                    }
                                    break;
                                }
                            }

                        }
                        if (step is RequestHelpDetailStageViewModel)
                        {
                            var requestStage = (RequestHelpRequestStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();
                            var detailStage = (RequestHelpDetailStageViewModel)step;
                            var reviewStage = (RequestHelpReviewStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpReviewStageViewModel).First();
                            reviewStage.Recipient = detailStage.Recipient;
                            reviewStage.Requestor = detailStage.Requestor;
                            reviewStage.OrganisationRequestor = detailStage.OrganisationRequestor;
                            reviewStage.Task = requestStage.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                            reviewStage.HealthCritical = requestStage.IsHealthCritical;
                            reviewStage.TimeRequested = requestStage.Timeframes.Where(X => X.IsSelected).FirstOrDefault();
                            reviewStage.RequestedFor = requestStage.Requestors.Where(x => x.IsSelected).FirstOrDefault();
                            reviewStage.CommunicationNeeds = detailStage.CommunicationNeeds;
                            reviewStage.OtherDetails = detailStage.OtherDetails;
                            reviewStage.ShowOtherDetails = detailStage.ShowOtherDetails;
                        }
                    }
                if (requestHelp.Action == "finish")
                {
                    var requestStage = (RequestHelpRequestStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();
                    var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).First();
                    int userId = 0;
                    if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
                    {
                        userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                    var response = await _requestService.LogRequestAsync(requestStage, detailStage, userId, HttpContext);
                    if (response.HasContent && response.IsSuccessful)
                    {                        
                        return RedirectToAction("Success", new
                        {
                            fulfillable = response.Content.Fulfillable,
                            onBehalf = detailStage.Type == RequestorType.OnBehalf ? true : false
                        });
                    }
                }
                 
                
            }
            catch (ValidationException vex)
            {
                _logger.LogError(vex, "a validation error occured in request help form action");   
             }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occured in request help form action");
                requestHelp.Errors.Add("Oops! an error occured sumbitting your request, please try again later.");
            }
        

            return View(requestHelp);
        }
    

    public async Task<IActionResult> RequestHelp(string source)
        {
            _logger.LogInformation("request-help");
             var model = await  _requestService.GetRequestHelpSteps(source);
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
            foreach(var question in model.Questions)
            {
                if (question.Location() != request.Position)
                    question.DontShow = true;
            }

            return PartialView("_Questions", model);
        }
        
        public class QuestionRequest
        {
            public RequestHelpRequestStageViewModel Step { get; set; }
            public int TaskID { get; set; }

            public string Position { get; set; }
        }
    }
}