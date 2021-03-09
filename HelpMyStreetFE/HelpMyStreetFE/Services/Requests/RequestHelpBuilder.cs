using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
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

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestHelpBuilder : IRequestHelpBuilder
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        public RequestHelpBuilder(IRequestHelpRepository requestHelpRepository)
        {
            _requestHelpRepository = requestHelpRepository;
        }

        public async Task<RequestHelpViewModel> GetSteps(RequestHelpJourney requestHelpJourney, int referringGroupID, string source)
        {
            RequestHelpFormVariant requestHelpFormVariant = requestHelpJourney.RequestHelpFormVariant;

            var model =  new RequestHelpViewModel
            {
                ReferringGroupID = referringGroupID,
                Source = source,
                RequestHelpFormVariant = requestHelpFormVariant,
                CurrentStepIndex = 0,
                Steps = new List<IRequestHelpStageViewModel>
                {
                    new RequestHelpRequestStageViewModel
                    {
                        PageHeading = GetHelpRequestPageTitle(requestHelpFormVariant),
                        IntoText = GetHelpRequestPageIntroText(requestHelpFormVariant),
                        Tasks = await GetRequestHelpTasks(requestHelpFormVariant),
                        Requestors = GetRequestorViewModels(requestHelpFormVariant),
                        Timeframes = GetRequestHelpTimeViewModels(requestHelpFormVariant),
                    },
                    new RequestHelpDetailStageViewModel()
                    {
                        ShowRequestorFields = !requestHelpJourney.RequestorDefinedByGroup,
                        FullRecipientAddressRequired = GetFullRecipientAddressRequired(requestHelpFormVariant),
                    },
                    new RequestHelpReviewStageViewModel(),
                }
            };

            if (requestHelpFormVariant == RequestHelpFormVariant.LincolnshireVolunteers)
            {
                model.Steps.Remove(model.Steps.Where(x => x is RequestHelpDetailStageViewModel).First());
            }

            return model;
        }

        private string GetHelpRequestPageTitle(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => "How can For the Love of Scrubs help?",
                RequestHelpFormVariant.HLP_CommunityConnector => "Get in touch with a Community Connector",
                RequestHelpFormVariant.Ruddington => "Request help from Ruddington Community Response Team",
                RequestHelpFormVariant.AgeUKNottsBalderton => "Request help from Balderton Community Support",
                RequestHelpFormVariant.AgeUKNottsNorthMuskham => "Request help from North Muskham Community Support",
                RequestHelpFormVariant.AgeUKSouthKentCoast_Public => "Request Help from Age UK South Kent Coast",
                RequestHelpFormVariant.AgeUKSouthKentCoast_RequestSubmitter => "Request Help from Age UK South Kent Coast",
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public => "Request Help from Age UK Faversham And Sittingbourne",
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_RequestSubmitter => "Request Help from Age UK Faversham And Sittingbourne",
                RequestHelpFormVariant.AgeUKNorthWestKent_Public => "Request Help from Age UK North West Kent",
                RequestHelpFormVariant.AgeUKNorthWestKent_RequestSubmitter => "Request Help from Age UK North West Kent",
                RequestHelpFormVariant.Sandbox_RequestSubmitter => "SANDBOX request form",
                _ => "What type of help are you looking for?"
            };
        }

        private string GetHelpRequestPageIntroText(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => "We have volunteers across the country donating their time and skills to help us beat coronavirus. If you need reusable fabric face coverings, we can help.",
                RequestHelpFormVariant.HLP_CommunityConnector => "If you’re feeling down, anxious or just ‘stuck’ and wanting someone to help you take action to improve your wellbeing, we can put you in touch with a trained volunteer Community Connector. Calls are free, confidential and focused on an issue that you want to make progress on.",
                RequestHelpFormVariant.AgeUKWirral => string.Empty,
                RequestHelpFormVariant.AgeUKSouthKentCoast_Public => "If you need help from Age UK South Kent Coast, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKSouthKentCoast_RequestSubmitter => "If you need help from Age UK South Kent Coast, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public => "If you need help from Age UK Faversham and Sittingbourne, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_RequestSubmitter => "If you need help from Age UK Faversham and Sittingbourne, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKNorthWestKent_Public => "If you need help from Age UK North West Kent, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKNorthWestKent_RequestSubmitter => "If you need help from Age UK North West Kent, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.Sandbox_RequestSubmitter => "Requests made through **this** form will be available within the Sandbox area of HelpMyStreet, for testing purposes, and will not trigger notifications to general users of HelpMyStreet.\r\n\r\nPlease ensure you can see this message whenever you wish to submit a Sandbox request.",
                _ => "People across the country are helping their neighbours and community to stay safe. Whatever you need, we have people who can help."
            };
        }

        private bool GetFullRecipientAddressRequired(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.HLP_CommunityConnector => false,
                _ => true
            };
        }

        private async Task<List<TasksViewModel>> GetRequestHelpTasks(RequestHelpFormVariant requestHelpFormVariant)
        {
            var tasks = new List<TasksViewModel>();
            if (requestHelpFormVariant == RequestHelpFormVariant.VitalsForVeterans)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.WellbeingPackage });
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.Shopping });
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions });
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.Errands });
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.Other });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.FtLOS)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = true });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.HLP_CommunityConnector)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.CommunityConnector, IsSelected = true });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.Ruddington)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = (requestHelpFormVariant == RequestHelpFormVariant.FaceMasks) },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.MealPreparation },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKWirral)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.ColdWeatherArmy },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNottsBalderton)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNottsNorthMuskham)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKSouthKentCoast_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.MealtimeCompanion},
                    new TasksViewModel { SupportActivity = SupportActivities.MealsToYourDoor },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKSouthKentCoast_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.MealtimeCompanion},
                    new TasksViewModel { SupportActivity = SupportActivities.MealsToYourDoor },
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.MealtimeCompanion},
                    new TasksViewModel { SupportActivity = SupportActivities.MealsToYourDoor },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.MealtimeCompanion},
                    new TasksViewModel { SupportActivity = SupportActivities.MealsToYourDoor },
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNorthWestKent_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.MealsToYourDoor },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNorthWestKent_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.MealsToYourDoor },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.LincolnshireVolunteers)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.VaccineSupport, IsSelected = true });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.Sandbox_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.VaccineSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = (requestHelpFormVariant == RequestHelpFormVariant.FaceMasks) },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.MealPreparation },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.HomeworkSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }

            return tasks;
        }

        public async Task<List<RequestHelpQuestion>> GetQuestionsForTask(RequestHelpFormVariant requestHelpFormVariant, RequestHelpFormStage requestHelpFormStage, SupportActivities supportActivity, int groupId)
        {
            var questions = await _requestHelpRepository.GetQuestionsByActivity(new GetQuestionsByActivitiesRequest
            {
                ActivitesRequest = new ActivitesRequest
                {
                    Activities = new List<SupportActivities>() { supportActivity }
                },
                RequestHelpFormVariantRequest = new RequestHelpFormVariantRequest
                {
                    RequestHelpFormVariant = requestHelpFormVariant
                },
                RequestHelpFormStageRequest = new RequestHelpFormStageRequest
                {
                    RequestHelpFormStage = requestHelpFormStage
                },
                GroupId = groupId,
            });

            List<RequestHelpQuestion> requestHelpQuestions = questions.SupportActivityQuestions[supportActivity].Select(x => new RequestHelpQuestion
            {
                ID = x.Id,
                InputType = x.Type,
                Label = x.Name,
                Required = x.Required,
                PlaceholderText = x.PlaceholderText,
                SubText = x.SubText,
                Location = x.Location,
                AdditionalData = x.AddtitonalData,
            }).ToList();


            return requestHelpQuestions;
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

        private List<RequestHelpTimeViewModel> GetRequestHelpTimeViewModels(RequestHelpFormVariant variant)
        {
            return variant switch
            {
                RequestHelpFormVariant.FtLOS => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before }, false),
                RequestHelpFormVariant.HLP_CommunityConnector => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before }, false),
                RequestHelpFormVariant.AgeUKSouthKentCoast_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, false),
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, false),
                RequestHelpFormVariant.AgeUKNorthWestKent_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, false),

                RequestHelpFormVariant.LincolnshireVolunteers => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.SpecificStartAndEndTimes }, false),
              
                RequestHelpFormVariant.Sandbox_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.SpecificStartAndEndTimes }, true),

                _ => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, true),
            };
        }


        private List<RequestHelpTimeViewModel> GetRequestHelpTimeViewModels(List<DueDateType> dueDateTypes, bool includeToday)
        {
            var vms = new List<RequestHelpTimeViewModel>();

            if (dueDateTypes.Contains(DueDateType.Before))
            {
                if (includeToday)
                {
                    vms.Add(new RequestHelpTimeViewModel { ID = 1, DueDateType = DueDateType.Before, TimeDescription = "Today", Days = 0 });
                    vms.Add(new RequestHelpTimeViewModel { ID = 9, DueDateType = DueDateType.Before, TimeDescription = "Within 3 days", Days = 3 });
                }
                vms.Add(new RequestHelpTimeViewModel { ID = 3, DueDateType = DueDateType.Before, TimeDescription = "Within a week", Days = 7 });
                vms.Add(new RequestHelpTimeViewModel { ID = 8, DueDateType = DueDateType.Before, TimeDescription = "Within 2 weeks", Days = 14 });
                vms.Add(new RequestHelpTimeViewModel { ID = 4, DueDateType = DueDateType.Before, TimeDescription = "When convenient", Days = 30 });
            }

            if (dueDateTypes.Contains(DueDateType.On))
            {
                vms.Add(new RequestHelpTimeViewModel() { ID = 6, TimeDescription = "On a Specific Date", DueDateType = DueDateType.On });
            }

            if (dueDateTypes.Contains(DueDateType.SpecificStartAndEndTimes))
            {
                vms.Add(new RequestHelpTimeViewModel() { ID = 7, TimeDescription = "On a Specific Date", DueDateType = DueDateType.SpecificStartAndEndTimes });
            }

            if (vms.Count == 1)
            {
                vms.First().IsSelected = true;
            }

            return vms;
        }
        private List<RequestorViewModel> GetRequestorViewModels(RequestHelpFormVariant variant)
        {
            return variant switch
            {
                RequestHelpFormVariant.HLP_CommunityConnector => GetRequestorViewModels(new List<RequestorType> { RequestorType.Myself, RequestorType.OnBehalf }),
                RequestHelpFormVariant.AgeUKWirral => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf }),
                RequestHelpFormVariant.LincolnshireVolunteers => GetRequestorViewModels(new List<RequestorType> { RequestorType.Organisation }),
                _ => GetRequestorViewModels(new List<RequestorType> { RequestorType.Myself, RequestorType.OnBehalf, RequestorType.Organisation }),
            };
        }

        private List<RequestorViewModel> GetRequestorViewModels(List<RequestorType> requestorTypes)
        {
            var vms = new List<RequestorViewModel>();

            if (requestorTypes.Contains(RequestorType.Myself))
            {
                vms.Add(new RequestorViewModel
                {
                    ID = 1,
                    ColourCode = "orange",
                    Title = "I am requesting help for myself",
                    Text = "I'm the person in need of help",
                    IconDark = "request-myself.svg",
                    IconLight = "request-myself-white.svg",
                    Type = RequestorType.Myself
                });
            }

            if (requestorTypes.Contains(RequestorType.OnBehalf))
            {
                vms.Add(new RequestorViewModel
                {
                    ID = 2,
                    ColourCode = "dark-blue",
                    Title = "On behalf of someone else",
                    Text = "I'm looking for help for a relative, neighbour or friend",
                    IconDark = "request-behalf.svg",
                    IconLight = "request-behalf-white.svg",
                    Type = RequestorType.OnBehalf
                });
            }

            if (requestorTypes.Contains(RequestorType.Organisation))
            {
                vms.Add(new RequestorViewModel
                {
                    ID = 3,
                    ColourCode = "dark-blue",
                    Title = "On behalf of an organisation",
                    Text = "I'm looking for help for an organisation",
                    IconDark = "request-organisation.svg",
                    IconLight = "request-organisation-white.svg",
                    Type = RequestorType.Organisation
                });
            }

            return vms;
        }
    }
}
