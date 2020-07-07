using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Enums.RequestHelp;
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
        private readonly IGroupService _groupService;
        public RequestHelpController(ILogger<RequestHelpController> logger, IRequestService requestService, IGroupService groupService)
        {
            _logger = logger;
            _requestService = requestService;
            _groupService = groupService;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RequestHelp(
        [ModelBinder(BinderType = typeof(RequestHelpModelBinder))]RequestHelpViewModel requestHelp,
        [ModelBinder(BinderType = typeof(RequestHelpStepsViewModelBinder))] IRequestHelpStageViewModel step)
        {
            try
            {
                requestHelp.Errors = new List<string>();
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

                        detailStage.ShowOtherDetails = 
                            requestStep.Tasks.Where(x => x.IsSelected).First().SupportActivity == HelpMyStreet.Utils.Enums.SupportActivities.FaceMask ? false : true;
                   

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
                                case RequestorType.Organisation:
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
                        reviewStage.Task = requestStage.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.OrganisationName = detailStage.Organisation;
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

                    // if they've come through as DIY and there not logged in, throw an error telling them they cant do that
                    if (requestHelp.RequestHelpFormVariant == RequestHelpFormVariant.DIY && userId == 0 )
                    {
                        requestHelp.Errors.Add("To submit a DIY Request, you must be logged in, to submit a normal request, please click on the Request Help link above");
                        throw new ValidationException("User tired to submit DIY Request without being logged in");
                    }

                    var response = await _requestService.LogRequestAsync(requestStage, detailStage, requestHelp.ReferringGroupID, requestHelp.Source, requestHelp.RequestHelpFormVariant, userId, HttpContext);
                    if (response.HasContent && response.IsSuccessful)
                    {
                        return RedirectToRoute("request-help/success", new
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


        public async Task<IActionResult> RequestHelp(string referringGroup, string source)
        {
            _logger.LogInformation("request-help");

            RequestHelpFormVariant requestHelpFormVariant = RequestHelpFormVariant.Default;
            int referringGroupId = DecodeGroupIdOrGetDefault(referringGroup);

            // TODO: Replace this with a call to Group Service (GetRequestHelpFormVariant) ...
            string groupKey = "";

            var getGroupResponse = await _groupService.GetGroup(referringGroupId);
            if (getGroupResponse.IsSuccessful)
            {
                groupKey = getGroupResponse.Content.Group.GroupKey;
            }

            if (source == "DIY")
            {
                requestHelpFormVariant = RequestHelpFormVariant.DIY;
            }
            else if (groupKey == "ftlos")
            {
                requestHelpFormVariant = RequestHelpFormVariant.FtLOS;
            }
            else if (groupKey == "v4v")
            {
                requestHelpFormVariant = RequestHelpFormVariant.VitalsForVeterans;
            }
            // END


            if (requestHelpFormVariant == RequestHelpFormVariant.DIY && (!User.Identity.IsAuthenticated))
                return Redirect("/login?ReturnUrl=request-help/0/DIY");

            var model = await _requestService.GetRequestHelpSteps(requestHelpFormVariant, referringGroupId, source);
            var requestStage = (RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();

            return View(model);
        }

        public IActionResult Success(Fulfillable fulfillable, bool onBehalf)
        {

            string message = "<p>Your request will be passed to a local volunteer who should get in touch shortly.</p>";

            string link = User.Identity.IsAuthenticated ? "/account" : "/";

            string button = $" <a href='{link}' class='btn cta large fill mt16 btn--request-help cta--orange'>Done</a>";
            string requestLink = "/request-help";

            if (fulfillable == Fulfillable.Accepted_ManualReferral || fulfillable == Fulfillable.Rejected_Unfulfillable)
            {
                message = "<p>We’ve just launched HelpMyStreet and we’re building our network across the country. We’re working hard to ensure we have local volunteers in your area who can get the right help to the right people. You’ll be contacted soon to progress your request.</p>";
            }

            if (onBehalf && !User.Identity.IsAuthenticated)
            {
                message += "<p>Are you Volunteering in your local area? Sign up as a Street Champion or Helper to help and support local people shelter safely at home.</p>";
                button = " <a href='/registration/stepone' class='btn cta large fill mt16 btn--sign-up '>Sign up</a>";
            }

            if (fulfillable == Fulfillable.Accepted_DiyRequest)
            {
                message = "Your request will now be available in the 'My Accepted Requests' area of your profile.";
                button = " <a href='/account/accepted-requests' class='btn cta large fill mt16 btn--request-help cta--orange'>Done</a>";
                requestLink = "/request-help/0/DIY";
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

            SuccessViewModel vm = new SuccessViewModel
            {
                Notifications = notifications,
                RequestLink = requestLink
            };

            return View(vm);
        }
 

        [HttpPost]
        public async Task<ActionResult> Questions([FromBody]QuestionRequest request)
        {
            TasksViewModel model = request.Step.Tasks.Where(x => x.ID == request.TaskID).First();
            RequestorType? requestorType = null;
            if (request.RequestorId.HasValue)
            {
                requestorType = request.Step.Requestors.Where(x => x.ID == request.RequestorId.Value).First().Type;
            }
            
            foreach (var question in model.Questions)
            {
                var matchedAnswer = request.Answers.Where(x => x.Id == question.ID && !string.IsNullOrEmpty(x.Answer)).FirstOrDefault();
                if(matchedAnswer != null)
                {
                    question.Model = matchedAnswer.Answer;
                }
                question.Show = question.Show(request.Position, requestorType);
            }

            return PartialView("_Questions", model);
        }

        public class QuestionRequest
        {
            public RequestHelpRequestStageViewModel Step { get; set; }
            public int TaskID { get; set; }
            public string Position { get; set; }
            public int? RequestorId  {get;set;}
            public List<QuestionAnswer> Answers { get; set; }
            public class QuestionAnswer
            {
                public int Id { get; set; }
                public string Answer { get; set; }
            }
        }

        private int DecodeGroupIdOrGetDefault(string encodedGroupId)
        {
            try
            {
                return Convert.ToInt32(Base64Utils.Base64Decode(encodedGroupId));
            }
            catch
            {
                return -1;
            }
        }

    }
}