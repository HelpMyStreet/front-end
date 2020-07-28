﻿using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Helpers.CustomModelBinder;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        public RequestHelpController(ILogger<RequestHelpController> logger, IRequestService requestService, IGroupService groupService, IRequestHelpBuilder requestHelpBuilder)
        {
            _logger = logger;
            _requestService = requestService;
            _groupService = groupService;
            _requestHelpBuilder = requestHelpBuilder;
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
                        requestStep.RequestHelpQuestions = await UpdateQuestionsViewModel(requestStep.RequestHelpQuestions, requestHelp.RequestHelpFormVariant, RequestHelpFormStage.Request, (SupportActivities)requestHelp.SelectedSupportActivity());
                        detailStage.Questions = await UpdateQuestionsViewModel(detailStage.Questions, requestHelp.RequestHelpFormVariant, RequestHelpFormStage.Detail, (SupportActivities)requestHelp.SelectedSupportActivity());
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
                        reviewStage.RequestStageQuestions = requestStage.RequestHelpQuestions.Questions;
                        reviewStage.DetailsStageQuestions = detailStage.Questions.Questions;
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
                    if (requestHelp.RequestHelpFormVariant == RequestHelpFormVariant.DIY && userId == 0)
                    {
                        requestHelp.Errors.Add("To \"Submit & Accept\" a Request, you must be logged in, to submit a normal request, please click on the Request Help link above");
                        throw new ValidationException("User tired to submit DIY Request without being logged in");
                    }

                    var isFTLOSJourney = requestHelp.RequestHelpFormVariant == RequestHelpFormVariant.FtLOS ? true : false;

                    var response = await _requestService.LogRequestAsync(requestStage, detailStage, requestHelp.ReferringGroupID, requestHelp.Source, userId, HttpContext);
                    if (response.HasContent && response.IsSuccessful)
                    {
                        return RedirectToRoute("request-help/success", new
                        {
                            fulfillable = response.Content.Fulfillable,
                            isFTLOS = isFTLOSJourney,
                            referringGroupId = requestHelp.ReferringGroupID,
                            source = requestHelp.Source
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

            int referringGroupId = DecodeGroupIdOrGetDefault(referringGroup);

            // Fix to allow existing routing
            if (referringGroup == "v4v")
            {
                referringGroupId = (await _groupService.GetGroupByKey("ageuklsl")).GroupId;
            }

            var groupServiceResponse = await _groupService.GetRequestHelpFormVariant(referringGroupId, source);
            RequestHelpFormVariant requestHelpFormVariant = groupServiceResponse == null ? RequestHelpFormVariant.Default : groupServiceResponse.RequestHelpFormVariant;

            if (requestHelpFormVariant == RequestHelpFormVariant.DIY && (!User.Identity.IsAuthenticated))
            {
                string encodedReferringGroupId = Base64Utils.Base64Encode(referringGroupId.ToString());
                return Redirect($"/login?ReturnUrl=request-help/{encodedReferringGroupId}/{source}");
            }

            var model = await _requestService.GetRequestHelpSteps(requestHelpFormVariant, referringGroupId, source);
            var requestStage = (RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();

            return View(model);
        }

        public IActionResult Success(Fulfillable fulfillable, bool isFTLOS, int referringGroupId, string source)
        {

            string message = "<p>Your request has been received and we are looking for a volunteer who can help. Someone should get in touch shortly.</p>";

            string doneLink = User.Identity.IsAuthenticated ? "/account" : "/";
            string button = $"<a href='{doneLink}' class='btn cta large fill mt16 btn--request-help cta--orange'>Done</a>";

            string encodedReferringGroupId = Base64Utils.Base64Encode(referringGroupId.ToString());
            string requestLink = $"/request-help/{encodedReferringGroupId}/{source}";

            string facemaskmessage = "<p>For the Love of Scrubs ask for a small donation of £3 - £4 per face covering to cover the cost of materials and help support their communities. Without donations they aren’t able to continue their good work.</p>" +
                "<p>If you are able to donate, you can do so on their Go Fund Me page <a href=\"https://www.gofundme.com/f/for-the-love-of-scrubs-face-coverings\" target=\"_blank\">here</a>.<p>";

            if (isFTLOS)
            {
                message += facemaskmessage;
            }

            if (!User.Identity.IsAuthenticated)
            {    
                message += "<p><strong>Would you be happy to help a neighbour?</strong></p>";
                message += "<p>Could you help a member of your local community if they needed something? There are lots of different ways you can help, from offering a friendly chat, to picking up groceries or prescriptions, or even sewing a face covering. Please take 5 minutes to sign-up now.</p>";
                button = $"<a href='/registration/step-one/{encodedReferringGroupId}/help-request-success' class='btn cta large fill mt16 btn--sign-up '>Sign up</a>";
            }
           
            if (fulfillable == Fulfillable.Accepted_DiyRequest)
            {
                message = "Your request will now be available in the 'My Accepted Requests' area of your profile.";

                if(isFTLOS)
                {
                    message += facemaskmessage;
                }

                button = "<a href='/account/accepted-requests' class='btn cta large fill mt16 btn--request-help cta--orange'>Done</a>";
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
        public async Task<ActionResult> Questions([FromBody] QuestionRequest request)
        {
            RequestorType? requestorType = null;
            if (request.RequestorId.HasValue)
            {
                //TODO: fix or remove
                //requestorType = request.Step.Requestors.Where(x => x.ID == request.RequestorId.Value).First().Type;
            }

            RequestHelpFormVariant requestHelpFormVariant = Enum.Parse<RequestHelpFormVariant>(request.FormVariant);
            RequestHelpFormStage requestHelpFormStage = Enum.Parse<RequestHelpFormStage>(request.FormStage);
            SupportActivities supportActivity = Enum.Parse<SupportActivities>(request.SupportActivity);

            QuestionsViewModel questionsViewModel = new QuestionsViewModel()
            {
                Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, supportActivity)
            };

            foreach (var question in questionsViewModel.Questions)
            {
                var matchedAnswer = request.Answers.Where(x => x.Id == question.ID && !string.IsNullOrEmpty(x.Answer)).FirstOrDefault();
                if (matchedAnswer != null)
                {
                    question.Model = matchedAnswer.Answer;
                }
                question.Show = question.Show(request.Position, requestorType);
            }

            return PartialView("_Questions", questionsViewModel);
        }

        public async Task<QuestionsViewModel> GetQuestionsViewModel(RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities supportActivity)
        {
            QuestionsViewModel questionsViewModel = new QuestionsViewModel()
            {
                Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, supportActivity)
            };

            foreach (var question in questionsViewModel.Questions)
            {
                //var matchedAnswer = request.Answers.Where(x => x.Id == question.ID && !string.IsNullOrEmpty(x.Answer)).FirstOrDefault();
                //var matchedPreviousAnswer = request.PreviousAnswers.Where(x => x.ID == question.ID && !string.IsNullOrEmpty(x.Model)).FirstOrDefault();
                //if (matchedAnswer != null)
                //{
                //    question.Model = matchedAnswer.Answer;
                //}
                //else if (matchedPreviousAnswer != null)
                //{
                //    question.Model = matchedPreviousAnswer.Model;
                //}
                question.Show = true;// question.Show(request.Position, requestorType);
            }

            return questionsViewModel;
        }

        private async Task<QuestionsViewModel> UpdateQuestionsViewModel(QuestionsViewModel previousQuestionsViewModel, RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities selectedSupportActivity)
        {
            QuestionsViewModel updatedQuestionsViewModel = new QuestionsViewModel() { Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, selectedSupportActivity) };

            if (previousQuestionsViewModel != null)
            {
                foreach (RequestHelpQuestion question in updatedQuestionsViewModel.Questions)
                {
                    var matchedQuestion = previousQuestionsViewModel.Questions.Where(pq => pq.ID == question.ID && !string.IsNullOrEmpty(pq.Model)).FirstOrDefault();
                    if (matchedQuestion != null)
                    {
                        question.Model = matchedQuestion.Model;
                    }
                }
            }

            return updatedQuestionsViewModel;
        }

        public class QuestionRequest
        {
            public string FormVariant { get; set; }
            public string FormStage { get; set; }
            public string SupportActivity { get; set; }
            public string Position { get; set; }
            public int? RequestorId { get; set; }
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