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
                            new RequestHelpTimeViewModel { ID = 1, DueDateType = DueDateType.Before, TimeDescription = "Today", Days = 0 },
                            new RequestHelpTimeViewModel { ID = 2, DueDateType = DueDateType.Before, TimeDescription = "Within 24 Hours", Days = 1 },
                            new RequestHelpTimeViewModel { ID = 3, DueDateType = DueDateType.Before, TimeDescription = "Within a Week", Days = 7 },
                            new RequestHelpTimeViewModel { ID = 4, DueDateType = DueDateType.Before, TimeDescription = "When Convenient", Days = 30 },
                            new RequestHelpTimeViewModel { ID = 5, DueDateType = DueDateType.Before, TimeDescription = "Other", AllowCustom = true },
                        },
                    },
                    new RequestHelpDetailStageViewModel()
                    {
                        ShowRequestorFields = !requestHelpJourney.RequestorDefinedByGroup,
                        FullRecipientAddressRequired = true,
                    },
                    new RequestHelpReviewStageViewModel(),
                }

            };
            if (requestHelpFormVariant == RequestHelpFormVariant.FtLOS)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKWirral || requestHelpFormVariant == RequestHelpFormVariant.VitalsForVeterans)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Requestors.RemoveAll(x => x.Type == RequestorType.Myself);

                requestStep.Timeframes.Insert(0, new RequestHelpTimeViewModel() { ID = 6, TimeDescription = "On a Specific Date", DueDateType = DueDateType.On });
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKSouthKentCoast_Public)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKSouthKentCoast_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Timeframes.Insert(0, new RequestHelpTimeViewModel() { ID = 6, TimeDescription = "On a Specific Date", DueDateType = DueDateType.On });
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Timeframes.Insert(0, new RequestHelpTimeViewModel() { ID = 6, TimeDescription = "On a Specific Date", DueDateType = DueDateType.On });
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNorthWestKent_Public)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.MeadowsCommunityHelpers_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Requestors.RemoveAll(x => x.Type == RequestorType.Myself);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.MeadowsCommunityHelpers_Public)
            {
                ((RequestHelpRequestStageViewModel)model.Steps.First()).Timeframes.RemoveRange(0, 2);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNorthWestKent_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Timeframes.Insert(0, new RequestHelpTimeViewModel() { ID = 6, TimeDescription = "On a Specific Date", DueDateType = DueDateType.On });
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeConnectsCardiff_Public)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Timeframes.RemoveRange(0, 2);
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.AgeConnectsCardiff_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Requestors.RemoveAll(x => x.Type == RequestorType.Myself);
                requestStep.Timeframes.Insert(0, new RequestHelpTimeViewModel() { ID = 6, TimeDescription = "On a Specific Date", DueDateType = DueDateType.On });
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.LincolnshireVolunteers || requestHelpFormVariant == RequestHelpFormVariant.Mansfield_CVS || requestHelpFormVariant == RequestHelpFormVariant.ApexBankStaff_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Requestors.RemoveAll(x => x.Type == RequestorType.Myself);
                requestStep.Requestors.RemoveAll(x => x.Type == RequestorType.OnBehalf);

                requestStep.Timeframes.Clear();
                requestStep.Timeframes.Insert(0, new RequestHelpTimeViewModel() { ID = 7, TimeDescription = "On a Specific Date", DueDateType = DueDateType.SpecificStartAndEndTimes, IsSelected = true });

                model.Steps.Remove(model.Steps.Where(x => x is RequestHelpDetailStageViewModel).First());
            }

            if (requestHelpFormVariant == RequestHelpFormVariant.Sandbox_RequestSubmitter)
            {
                var requestStep = ((RequestHelpRequestStageViewModel)model.Steps.Where(x => x is RequestHelpRequestStageViewModel).First());
                requestStep.Timeframes.Add(new RequestHelpTimeViewModel() { ID = 7, TimeDescription = "On a Specific Date", DueDateType = DueDateType.SpecificStartAndEndTimes });
            }


            return model;
        }

        private string GetHelpRequestPageTitle(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => "How can For the Love of Scrubs help?",
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
                RequestHelpFormVariant.MeadowsCommunityHelpers_Public => "Request Help from the Meadows Community Helpers",
                RequestHelpFormVariant.MeadowsCommunityHelpers_RequestSubmitter => "Request Help from the Meadows Community Helpers",
                RequestHelpFormVariant.AgeConnectsCardiff_Public => "Request Help from Age Connects Cardiff and the Vale",
                RequestHelpFormVariant.AgeConnectsCardiff_RequestSubmitter => "Request Help from Age Connects Cardiff and the Vale",
                RequestHelpFormVariant.Soutwell_Public => "Request Help from Southwell Torpedos",
                _ => "What type of help are you looking for?"
            };
        }

        private string GetHelpRequestPageIntroText(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.FtLOS => "We have volunteers across the country donating their time and skills to help us beat coronavirus. If you need reusable fabric face coverings, we can help.", 
                RequestHelpFormVariant.AgeUKWirral => string.Empty,
                RequestHelpFormVariant.AgeUKSouthKentCoast_Public => "If you need help from Age UK South Kent Coast, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKSouthKentCoast_RequestSubmitter => "If you need help from Age UK South Kent Coast, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public => "If you need help from Age UK Faversham and Sittingbourne, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_RequestSubmitter => "If you need help from Age UK Faversham and Sittingbourne, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKNorthWestKent_Public => "If you need help from Age UK North West Kent, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeUKNorthWestKent_RequestSubmitter => "If you need help from Age UK North West Kent, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.Sandbox_RequestSubmitter => "Requests made through **this** form will be available within the Sandbox area of HelpMyStreet, for testing purposes, and will not trigger notifications to general users of HelpMyStreet.\r\n\r\nPlease ensure you can see this message whenever you wish to submit a Sandbox request.",
                RequestHelpFormVariant.MeadowsCommunityHelpers_Public => "If you need help from a volunteer in the Meadows, fill in this short form to see if there is someone who can help!",
                RequestHelpFormVariant.MeadowsCommunityHelpers_RequestSubmitter => "If you need help from a volunteer in the Meadows, fill in this short form to see if there is someone who can help!",
                RequestHelpFormVariant.AgeConnectsCardiff_Public => "If you need help from Age Connects Cardiff and the Vale, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.AgeConnectsCardiff_RequestSubmitter => "If you need help from Age Connects Cardiff and the Vale, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                _ => "People across the country are helping their neighbours and community to stay safe. Whatever you need, we have people who can help."
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
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport });
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.Other });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.FtLOS)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.FaceMask, IsSelected = true });
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
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKNottsBalderton)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking },
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
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking },
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
            else if (requestHelpFormVariant == RequestHelpFormVariant.ApexBankStaff_RequestSubmitter)
            {
                tasks.Add(new TasksViewModel { SupportActivity = SupportActivities.BankStaffVaccinator, IsSelected = true });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.MeadowsCommunityHelpers_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.DigitalSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.BinDayAssistance },
                    new TasksViewModel { SupportActivity = SupportActivities.Covid19Help },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.MeadowsCommunityHelpers_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.FaceMask },
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.Errands },
                    new TasksViewModel { SupportActivity = SupportActivities.DigitalSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.BinDayAssistance },
                    new TasksViewModel { SupportActivity = SupportActivities.Covid19Help },
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
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeConnectsCardiff_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.InPersonBefriending },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PracticalSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeConnectsCardiff_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping },
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { SupportActivity = SupportActivities.InPersonBefriending },
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { SupportActivity = SupportActivities.PracticalSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.Mansfield_CVS)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel {SupportActivity = SupportActivities.VaccineSupport, IsSelected = true}
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
    }
}
