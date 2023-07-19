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
        
        public RequestHelpViewModel GetSteps(RequestHelpJourney requestHelpJourney, int referringGroupID, string source, string language)
        {
            RequestHelpFormVariant requestHelpFormVariant = requestHelpJourney.RequestHelpFormVariant;

            var model = new RequestHelpViewModel
            {
                ReferringGroupID = referringGroupID,
                Source = source,
                Language = language,
                RequestHelpFormVariant = requestHelpFormVariant,
                CurrentStepIndex = 0,
                Steps = new List<IRequestHelpStageViewModel>
                {
                    new RequestHelpRequestStageViewModel
                    {
                        PageHeading = GetHelpRequestPageTitle(requestHelpFormVariant, referringGroupID),
                        IntoText = GetHelpRequestPageIntroText(requestHelpFormVariant, referringGroupID),
                        PageHeadingClass = GetHelpRequestPageHeadingClass(requestHelpFormVariant),
                        Tasks = GetRequestHelpTasks(requestHelpFormVariant),
                        Requestors = GetRequestorViewModels(requestHelpFormVariant),
                        Frequencies = GetFrequencies(requestHelpFormVariant),
                        Timeframes = GetRequestHelpTimeViewModels(requestHelpFormVariant),
                    },
                    new RequestHelpDetailStageViewModel()
                    {
                        ShowRequestorFields = !requestHelpJourney.RequestorDefinedByGroup,
                        RecipientPostcodeRequired = GetRecipientAddressRequired(requestHelpFormVariant),
                        FullRecipientAddressRequired = GetRecipientAddressRequired(requestHelpFormVariant),
                    },
                    new RequestHelpReviewStageViewModel(),
                }
            };

            if (requestHelpFormVariant == RequestHelpFormVariant.Mansfield_CVS)
            {
                model.Steps.Remove(model.Steps.Where(x => x is RequestHelpDetailStageViewModel).First());
            }

            return model;
        }

        private string GetHelpRequestPageTitle(RequestHelpFormVariant requestHelpFormVariant, int referringGroupId)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.Ruddington => "Request help from Ruddington Community Response Team",
                RequestHelpFormVariant.Sandbox_RequestSubmitter => "SANDBOX request form",
                RequestHelpFormVariant.Soutwell_Public => "Request Help from Southwell Torpedos",
                _ => "What type of help are you looking for?"
            };
        }

        private string GetHelpRequestPageIntroText(RequestHelpFormVariant requestHelpFormVariant, int referringGroupId)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.AgeUKWirral => string.Empty,
                RequestHelpFormVariant.Sandbox_RequestSubmitter => "Requests made through **this** form will be available within the Sandbox area of HelpMyStreet, for testing purposes, and will not trigger notifications to general users of HelpMyStreet.\r\n\r\nPlease ensure you can see this message whenever you wish to submit a Sandbox request.",                
                _ => "People across the country are helping their neighbours and community to stay safe. Whatever you need, we have people who can help."
            };
        }

        private string GetHelpRequestPageHeadingClass(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                _ => ""
            };
        }

        private List<TasksViewModel> GetRequestHelpTasks(RequestHelpFormVariant requestHelpFormVariant)
        {
            var tasks = new List<TasksViewModel>();
            if (requestHelpFormVariant == RequestHelpFormVariant.Ruddington)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask, },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.MealPreparation },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking },
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
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
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
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
                    new TasksViewModel { SupportActivity = SupportActivities.EmergencySupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }            
            else if (requestHelpFormVariant == RequestHelpFormVariant.Soutwell_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }            
            else
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },                    
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

        public RequestPersonalDetails MapRecipient(RequestHelpDetailStageViewModel detailStage, string alternativePostcode)
        {
            string postcode;
            try
            {
                postcode = PostcodeFormatter.FormatPostcode(detailStage.Recipient.Postcode ?? alternativePostcode);
            }
            catch
            {
                throw new Exception($"Invalid recipient postcode {detailStage.Recipient.Postcode ?? alternativePostcode}");
            }

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
                    Postcode = postcode,
                }
            };
        }

        public RequestPersonalDetails MapRequestor(RequestHelpDetailStageViewModel detailStage)
        {
            string postcode;
            try
            {
                postcode = PostcodeFormatter.FormatPostcode(detailStage.Requestor.Postcode);
            }
            catch
            {
                throw new Exception($"Invalid requestor postcode {detailStage.Requestor.Postcode}");
            }

            return new RequestPersonalDetails
            {
                FirstName = detailStage.Requestor.Firstname,
                LastName = detailStage.Requestor.Lastname,
                MobileNumber = detailStage.Requestor.MobileNumber,
                OtherNumber = detailStage.Requestor.AlternatePhoneNumber,
                EmailAddress = detailStage.Requestor.Email,
                Address = new Address
                {
                    Postcode = postcode,
                }
            };
        }

        private List<RequestHelpTimeViewModel> GetRequestHelpTimeViewModels(RequestHelpFormVariant variant)
        {
            return variant switch
            {
                RequestHelpFormVariant.Sandbox_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.SpecificStartAndEndTimes }, true),                
                _ => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.On }, true),
            };
        }


        private List<RequestHelpTimeViewModel> GetRequestHelpTimeViewModels(List<DueDateType> dueDateTypes, bool includeWhenConvenient)
        {
            var vms = new List<RequestHelpTimeViewModel>();

            if (dueDateTypes.Contains(DueDateType.ASAP))
            {
                vms.Add(new RequestHelpTimeViewModel { ID = 10, DueDateType = DueDateType.ASAP, Description = "As soon as possible", HideForSupportActivities = new List<SupportActivities> { SupportActivities.FaceMask, SupportActivities.VaccineSupport, SupportActivities.AdvertisingRoles, SupportActivities.NHSSteward } });
            }

            if (dueDateTypes.Contains(DueDateType.Before))
            {
                vms.Add(new RequestHelpTimeViewModel { ID = 3, DueDateType = DueDateType.Before, Description = "Within a week", Days = 7, HideForRepeatRequests = true, HideForSupportActivities = new List<SupportActivities> { SupportActivities.VaccineSupport, SupportActivities.AdvertisingRoles, SupportActivities.NHSSteward } });
                vms.Add(new RequestHelpTimeViewModel { ID = 8, DueDateType = DueDateType.Before, Description = "Within 2 weeks", Days = 14, HideForRepeatRequests = true, HideForSupportActivities = new List<SupportActivities> { SupportActivities.VaccineSupport, SupportActivities.AdvertisingRoles, SupportActivities.NHSSteward } });
                if (includeWhenConvenient)
                {
                    vms.Add(new RequestHelpTimeViewModel { ID = 4, DueDateType = DueDateType.Before, Description = "Within 30 days", Days = 30, HideForRepeatRequests = true, HideForSupportActivities = new List<SupportActivities> { SupportActivities.VaccineSupport, SupportActivities.AdvertisingRoles, SupportActivities.NHSSteward } });
                }
            }

            if (dueDateTypes.Contains(DueDateType.On))
            {
                vms.Add(new RequestHelpTimeViewModel() { ID = 6, Description = "On a specific date", DueDateType = DueDateType.On, HideForSupportActivities = new List<SupportActivities> { SupportActivities.FaceMask, SupportActivities.AdvertisingRoles, SupportActivities.NHSSteward } });
            }

            if (dueDateTypes.Contains(DueDateType.SpecificStartAndEndTimes))
            {
                vms.Add(new RequestHelpTimeViewModel() { ID = 7, Description = "On a specific date", DueDateType = DueDateType.SpecificStartAndEndTimes, HideForSupportActivities = new List<SupportActivities> { SupportActivities.FaceMask, SupportActivities.AdvertisingRoles, SupportActivities.NHSCheckInAndChat, SupportActivities.NHSCheckInAndChatPlus, SupportActivities.NHSTransport, SupportActivities.EmergencySupport } });
            }

            if (dueDateTypes.Contains(DueDateType.OpenUntil))
            {
                vms.Add(new RequestHelpTimeViewModel() { ID = 11, Description = "Opportunity start date", DueDateType = DueDateType.OpenUntil, HideForSupportActivities = new List<SupportActivities> { SupportActivities.FaceMask, SupportActivities.Other } });
            }

            return vms;
        }
        private List<RequestorViewModel> GetRequestorViewModels(RequestHelpFormVariant variant)
        {
            return variant switch
            {
                RequestHelpFormVariant.AgeUKWirral => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf }),
                _ => GetRequestorViewModels(new List<RequestorType> { RequestorType.Myself, RequestorType.OnBehalf }),
            };
        }

        private List<FrequencyViewModel> GetFrequencies(RequestHelpFormVariant variant)
        {            
            return new List<FrequencyViewModel>
            {
                new FrequencyViewModel(Frequency.Once, null),
                new FrequencyViewModel(Frequency.Daily, new List<SupportActivities>{SupportActivities.FaceMask}),
                new FrequencyViewModel(Frequency.Weekly, new List<SupportActivities>{SupportActivities.FaceMask}),
                new FrequencyViewModel(Frequency.Fortnightly, new List<SupportActivities>{SupportActivities.FaceMask}),
                new FrequencyViewModel(Frequency.EveryFourWeeks, new List<SupportActivities>{SupportActivities.FaceMask}),
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
                    Description = "I am requesting help for myself",
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
                    Description = "On behalf of someone else",
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
                    Description = "On behalf of an organisation",
                    Text = "I'm looking for help for an organisation",
                    IconDark = "request-organisation.svg",
                    IconLight = "request-organisation-white.svg",
                    Type = RequestorType.Organisation
                });
            }

            return vms;
        }

        private bool GetRecipientAddressRequired(RequestHelpFormVariant variant)
        {
            return true;
        }
    }
}
