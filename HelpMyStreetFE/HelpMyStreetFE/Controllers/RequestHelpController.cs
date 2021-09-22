using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreetFE.Helpers.CustomModelBinder;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Requests;
using HelpMyStreetFE.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Controllers
{

    public class RequestHelpController : Controller
    {
        private readonly ILogger<RequestHelpController> _logger;
        private readonly IRequestService _requestService;
        private readonly IRequestUpdatingService _requestUpdatingService;
        private readonly IGroupService _groupService;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        private readonly IAuthService _authService;
        private readonly IGroupMemberService _groupMemberService;
        public RequestHelpController(ILogger<RequestHelpController> logger, IRequestService requestService, IGroupService groupService, IRequestHelpBuilder requestHelpBuilder, IAuthService authService, IGroupMemberService groupMemberService, IRequestUpdatingService requestUpdatingService)
        {
            _logger = logger;
            _requestService = requestService;
            _groupService = groupService;
            _requestHelpBuilder = requestHelpBuilder;
            _authService = authService;
            _groupMemberService = groupMemberService;
            _requestUpdatingService = requestUpdatingService;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RequestHelp(
        [ModelBinder(BinderType = typeof(RequestHelpModelBinder))]RequestHelpViewModel requestHelp,
        [ModelBinder(BinderType = typeof(RequestHelpStepsViewModelBinder))] IRequestHelpStageViewModel step,
        CancellationToken cancellationToken)
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
                        var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).FirstOrDefault();
                        var reviewStage = (RequestHelpReviewStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpReviewStageViewModel).First();

                        if (detailStage != null)
                        {
                            detailStage.Type = requestStep.Requestors.Where(x => x.IsSelected).First().Type;
                            detailStage.Questions = await UpdateQuestionsViewModel(detailStage.Questions, requestHelp.RequestHelpFormVariant, RequestHelpFormStage.Detail, (SupportActivities)requestHelp.SelectedSupportActivity(), requestHelp.ReferringGroupID);
                            detailStage.NeedBothNames = requestStep.Tasks.Where(x => x.IsSelected).Any(x => x.SupportActivity == SupportActivities.CollectingPrescriptions);

                            var loggedInUser = await _authService.GetCurrentUser(cancellationToken);
                            if (loggedInUser != null)
                            {
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

                        reviewStage.Task = requestStep.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.FrequencyRequested = requestStep.Frequencies.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.TimeRequested = requestStep.Timeframes.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.OccurrencesRequested = requestStep.Occurrences;
                        reviewStage.RequestedFor = requestStep.Requestors.Where(x => x.IsSelected).FirstOrDefault();
                        reviewStage.RequestStageQuestions = requestStep.Questions.Questions;
                    }
                    if (step is RequestHelpDetailStageViewModel)
                    {
                        var requestStage = (RequestHelpRequestStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();
                        var detailStage = (RequestHelpDetailStageViewModel)step;
                        var reviewStage = (RequestHelpReviewStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpReviewStageViewModel).First();
                        reviewStage.Recipient = detailStage.Recipient;
                        reviewStage.Requestor = detailStage.Requestor;
                        reviewStage.OrganisationName = detailStage.Organisation;
                        reviewStage.DetailsStageQuestions = detailStage.Questions.Questions;
                        reviewStage.ShowRequestor = detailStage.ShowRequestorFields && (reviewStage.RequestedFor.Type != RequestorType.Myself);
                    }
                }
                if (requestHelp.Action == "finish")
                {
                    var requestStage = (RequestHelpRequestStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();
                    var detailStage = (RequestHelpDetailStageViewModel)requestHelp.Steps.Where(x => x is RequestHelpDetailStageViewModel).FirstOrDefault();
                    var user = await _authService.GetCurrentUser(cancellationToken);

                    string language = requestHelp.Language;

                    var response = await _requestUpdatingService.LogRequestAsync(requestStage, detailStage, requestHelp.ReferringGroupID, requestHelp.Source, language, user, cancellationToken);
                    if (response.Equals(Fulfillable.Accepted_ManualReferral))
                    {
                        return RedirectToRoute("request-help/success", new
                        {
                            fulfillable = response,
                            requestHelpFormVariant = requestHelp.RequestHelpFormVariant,
                            referringGroup = Base64Utils.Base64Encode(requestHelp.ReferringGroupID),
                            source = requestHelp.Source
                        });
                    }
                    else
                    {
                        throw new Exception($"Bad response from PostRequestForHelpRequest: {response}");
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
                requestHelp.Errors.Add("Oops! An error occured submitting your request; please try again later.");
            }


            return View(requestHelp);
        }


        public async Task<IActionResult> RequestHelp(string referringGroup, string source, string language, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request-help");

            if (referringGroup == "v4v")
            {
                return Redirect("/account");
            }

            int referringGroupId = DecodeGroupIdOrGetDefault(referringGroup);
            source = ValidateSource(source);

            var requestHelpJourney = await _groupService.GetRequestHelpFormVariant(referringGroupId, source);

            if (requestHelpJourney.AccessRestrictedByRole)
            {
                var user = await _authService.GetCurrentUser(cancellationToken);
                var userHasPermission = user != null && await _groupMemberService.GetUserHasRole(user.ID, referringGroupId, GroupRoles.RequestSubmitter, true, cancellationToken);
                if (!userHasPermission)
                {
                    return RedirectToAction("403", "Error");
                }
            }

            if (requestHelpJourney.RequestHelpFormVariant == RequestHelpFormVariant.ChildGroupSelector)
            {
                return await ChildGroupSelector(referringGroupId, cancellationToken);
            }

            var model = _requestHelpBuilder.GetSteps(requestHelpJourney, referringGroupId, source, language);
            var requestStage = (RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First();

            SupportActivities? selectedTask = requestStage.Tasks.Where(t => t.IsSelected).FirstOrDefault()?.SupportActivity;
            if (selectedTask != null)
            {
                requestStage.Questions = await UpdateQuestionsViewModel(null, requestHelpJourney.RequestHelpFormVariant, RequestHelpFormStage.Request, selectedTask.Value, referringGroupId);
            }
            requestStage.RequestGuid = Guid.NewGuid();

            return View(model);
        }

        public IActionResult Success(Fulfillable fulfillable, RequestHelpFormVariant requestHelpFormVariant, string referringGroup, string source)
        {
            source = ValidateSource(source);

            string button;

            string message = requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => @"<p>Your request has been received and we are looking for a volunteer who can help. Someone should get in touch shortly.</p>
                                                    <p>For the Love of Scrubs ask for a small donation of £3 - £4 per face covering to cover the cost of materials and help support their communities. Without donations they aren’t able to continue their good work.</p>
                                                    <p>If you are able to donate, you can do so on their Go Fund Me page <a href='https://www.gofundme.com/f/for-the-love-of-scrubs-face-coverings\' target=\'_blank\'>here</a>.<p>",
                RequestHelpFormVariant.Ruddington => @"<p>Your request has been received and we're looking for a volunteer who can help, as soon as we find someone we’ll let you know by email. Please be aware that we cannot guarantee help, but we’ll do our best to find a volunteer near you.</p>",
                _ => @"<p>Your request has been received and we are looking for a volunteer who can help. Someone should get in touch shortly.</p>"
            };

            if (User.Identity.IsAuthenticated)
            {
                button = $"<a href='/account' class='btn cta large fill mt16 cta--orange'>Done</a>";
            }
            else
            {    
                message += "<p><strong>Would you be happy to help a neighbour?</strong></p>";
                message += "<p>Could you help a member of your local community if they needed something? There are lots of different ways you can help, from offering a friendly chat, to picking up groceries or prescriptions, or even sewing a face covering. Please take 5 minutes to sign up now.</p>";
                button = $"<a href='/login' class='btn cta large fill mt16 '>Sign Up or Log In</a>";
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
                RequestLink = $"/request-help/{referringGroup}/{source}"
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<ActionResult> Questions([FromBody] QuestionRequest request)
        {
            RequestHelpFormVariant requestHelpFormVariant = Enum.Parse<RequestHelpFormVariant>(request.FormVariant);
            RequestHelpFormStage requestHelpFormStage = Enum.Parse<RequestHelpFormStage>(request.FormStage);
            SupportActivities supportActivity = Enum.Parse<SupportActivities>(request.SupportActivity);

            QuestionsViewModel questionsViewModel = new QuestionsViewModel()
            {
                Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, supportActivity, request.GroupId)
            };

            questionsViewModel = questionsViewModel.GetQuestionsByLocation(request.Position);

            foreach (var question in questionsViewModel.Questions)
            {
                var matchedAnswer = request.Answers.Where(x => x.Id == question.ID && !string.IsNullOrEmpty(x.Answer)).FirstOrDefault();
                if (matchedAnswer != null)
                {
                    question.Model = matchedAnswer.Answer;
                }
            }

            return PartialView("_Questions", questionsViewModel);
        }

        private async Task<QuestionsViewModel> UpdateQuestionsViewModel(QuestionsViewModel previousQuestionsViewModel, RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities selectedSupportActivity, int groupId)
        {
            QuestionsViewModel updatedQuestionsViewModel = new QuestionsViewModel()
            {
                Questions = await _requestHelpBuilder.GetQuestionsForTask(requestHelpFormVariant, requestHelpFormStage, selectedSupportActivity, groupId)
            };

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
            public int GroupId { get; set; }
            public string FormVariant { get; set; }
            public string FormStage { get; set; }
            public string SupportActivity { get; set; }
            public string Position { get; set; }
            public List<QuestionAnswer> Answers { get; set; }
            public class QuestionAnswer
            {
                public int Id { get; set; }
                public string Answer { get; set; }
            }
        }

        private async Task<IActionResult> ChildGroupSelector(int groupId, CancellationToken cancellationToken)
        {
            var vm = new ChildGroupSelectorViewModel
            {
                Groups = await _groupService.GetChildGroups(groupId)
            };

            return View("ChildGroupSelector", vm);
        }

        private int DecodeGroupIdOrGetDefault(string encodedGroupId)
        {
            try
            {
                return Base64Utils.Base64DecodeToInt(encodedGroupId);
            }
            catch
            {
                return -1;
            }
        }

        private string ValidateSource(string source)
        {
            if (source != null && source.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                return source;
            }
            else
            {
                return null;
            }
        }
    }
}