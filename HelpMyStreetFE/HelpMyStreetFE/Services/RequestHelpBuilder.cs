using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Enums.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;
using HelpMyStreetFE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class RequestHelpBuilder : IRequestHelpBuilder
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        public RequestHelpBuilder(IRequestHelpRepository requestHelpRepository)
        {
            _requestHelpRepository = requestHelpRepository;
        }

        public async Task<RequestHelpViewModel> GetSteps(RequestHelpSource source)
        {

            var model =  new RequestHelpViewModel
            {
                Source = source,
                CurrentStepIndex = 0,
                Steps = new List<IRequestHelpStageViewModel>
                {
                    new RequestHelpRequestStageViewModel
                    {
                        Tasks = await GetRequestHelpTasks(source),
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
                                Type = RequestorType.Myself
                            },
                            new RequestorViewModel
                            {
                                ID = 2,
                                ColourCode = "dark-blue",
                                Title = "On behalf of someone else",
                                Text = "I'm looking for help for a relative, neighbour or friend",
                                IconDark = "request-behalf.svg",
                                IconLight = "request-behalf-white.svg",
                                Type = RequestorType.OnBehalf
                            },
                                        new RequestorViewModel
                            {
                                ID = 3,
                                ColourCode = "dark-blue",
                                Title = "On behalf of an organisation",
                                Text = "I'm looking for help for an organisation",
                                IconDark = "request-organisation.svg",
                                IconLight = "request-organisation-white.svg",
                                Type = RequestorType.Organisation                                
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
                    new RequestHelpDetailStageViewModel(),
                    new RequestHelpReviewStageViewModel(),
                }

            };
            if (source == RequestHelpSource.DIY) {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Requestors.RemoveAll(x => x.Type ==  RequestorType.Myself);                
            }
            return model;
        }

        public int? GetVolunteerUserID(RequestHelpRequestStageViewModel requestStage,  RequestorType type,  RequestHelpSource source, int userId)
        {
            if (source == RequestHelpSource.DIY && requestStage.Tasks
                                                    .Where(x => x.IsSelected).FirstOrDefault()
                                                    ?.Questions.Where(x => x.ID == (int)Questions.WillYouCompleteYourself)
                                                    .FirstOrDefault()?.Model == "true")
            {
                return userId;
            }

            return null;
        }

        private async Task<List<TasksViewModel>> GetRequestHelpTasks(RequestHelpSource source)
        {
            var tasks = new List<TasksViewModel>();
            if (source == RequestHelpSource.VitalsForVeterans)
            {
                tasks.Add(new TasksViewModel { ID = 11, SupportActivity = SupportActivities.WellbeingPackage });
            }

            if (source == RequestHelpSource.FtLOS)
            {
                tasks.Add(new TasksViewModel { ID = 2, SupportActivity = SupportActivities.FaceMask, IsSelected = true });
            }
            else
            {
                tasks.AddRange(new List<TasksViewModel>
            {
                    new TasksViewModel { ID = 1,SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { ID = 2, SupportActivity = SupportActivities.FaceMask },
                    new TasksViewModel { ID = 3, SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { ID = 4, SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { ID = 5, SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { ID = 6, SupportActivity = SupportActivities.MealPreparation },
                    new TasksViewModel { ID = 7, SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { ID = 9, SupportActivity = SupportActivities.HomeworkSupport },
                    new TasksViewModel { ID = 10, SupportActivity = SupportActivities.Other },
             });
            }

            var questions = await _requestHelpRepository.GetQuestionsByActivity(new GetQuestionsByActivitiesRequest
            {
                ActivitesRequest = new ActivitesRequest
                {
                    Activities = tasks.Select(x => x.SupportActivity).ToList()
                }
            });

            tasks.ForEach(x => x.Questions = questions.SupportActivityQuestions[x.SupportActivity].Select(x => new RequestHelpQuestion
            {
                ID = x.Id,
                InputType = x.Type,
                Label = x.Name,
                Required = x.Required,
                AdditionalData = x.AddtitonalData,
                VisibleForRequestorTypes = GetRequestorTypeQuestion(source, x.Id)
            }).ToList());

            if(source != RequestHelpSource.DIY)
            { 
                //question for DIY only
                tasks.ForEach(x => x.Questions.RemoveAll(x => x.ID == (int)Questions.WillYouCompleteYourself));
            }

            return tasks;
        }
        public List<RequestorType> GetRequestorTypeQuestion(RequestHelpSource source, int questionId)
        {
            if (source == RequestHelpSource.DIY && ((Questions)questionId) == Questions.WillYouCompleteYourself)
            {
                return new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation };
            }
            return null;
        }

        public RequestPersonalDetails MapRecipient(RequestHelpDetailStageViewModel detailStage)
        {
            return new RequestPersonalDetails
            {
                FirstName = detailStage.Recipient.Firstname,
                LastName = detailStage.Recipient.Lastname,
                MobileNumber = detailStage.Recipient.MobileNumber,
                OtherNumber = detailStage.Recipient.AlternatePhoneNumber,
                EmailAddress = detailStage.Recipient.Email,
                Address = new Address
                {
                    AddressLine1 = detailStage.Recipient.AddressLine1,
                    AddressLine2 = detailStage.Recipient.AddressLine2,
                    Locality = detailStage.Recipient.Town,
                    Postcode = PostcodeFormatter.FormatPostcode(detailStage.Recipient.Postcode),
                }
            };
        }

        public RequestPersonalDetails MapRequestor(RequestHelpDetailStageViewModel detailStage)
        {
            return new RequestPersonalDetails
            {
                FirstName = detailStage.Requestor.Firstname,
                LastName = detailStage.Requestor.Lastname,
                MobileNumber = detailStage.Requestor.MobileNumber,
                OtherNumber = detailStage.Requestor.AlternatePhoneNumber,
                EmailAddress = detailStage.Requestor.Email,
                Address = new Address
                {
                    Postcode = PostcodeFormatter.FormatPostcode(detailStage.Requestor.Postcode),
                }
            };
        }
    }
}
