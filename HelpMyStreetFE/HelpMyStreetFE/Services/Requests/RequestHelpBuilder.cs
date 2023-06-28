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

            if (requestHelpFormVariant == RequestHelpFormVariant.LincolnshireVolunteers || requestHelpFormVariant == RequestHelpFormVariant.Mansfield_CVS || requestHelpFormVariant == RequestHelpFormVariant.ApexBankStaff_RequestSubmitter)
            {
                model.Steps.Remove(model.Steps.Where(x => x is RequestHelpDetailStageViewModel).First());
            }

            return model;
        }

        private string GetLincolnshirePageTitle(int referringGroupId)
        {
            return ((HelpMyStreet.Utils.Enums.Groups)referringGroupId) switch
            {
                HelpMyStreet.Utils.Enums.Groups.LincolnshireVolunteers => "Request help from Lincolnshire Volunteers",
                HelpMyStreet.Utils.Enums.Groups.LincolnshireLCVS => "Request help as Lincolnshire VCS (LCVS)",
                HelpMyStreet.Utils.Enums.Groups.LincolnshireVCS => "Request help as Lincolnshire VCS (VCS)",
                _ => throw new ArgumentException($"Unexpected ReferringGroup value: {referringGroupId}", nameof(referringGroupId))
            };
        }

        private string GetLincolnshirePageIntroText(int referringGroupId)
        {
            return ((HelpMyStreet.Utils.Enums.Groups)referringGroupId) switch
            {
                HelpMyStreet.Utils.Enums.Groups.LincolnshireVolunteers => "If you would like to request help from Lincolnshire Volunteers, complete this form to make your request visible to our pool of volunteers.\r\n\r\nIf you would like to log a request as one of our partner organisations (e.g. VCS, LCVS, PCNs etc.) please use the relevant request form or email mailto:contact@helpmystreet.org if you require access.",
                HelpMyStreet.Utils.Enums.Groups.LincolnshireLCVS => "If you would like to request help as Lincolnshire VCS (LCVS), complete this form to make your request visible to the pool of Lincolnshire Volunteers.\r\n\r\nIf you would like to log a request as another partner organisations or the 'parent group' (Lincolnshire Volunteers) please use the relevant request form or email mailto:contact@helpmystreet.org if you require access.",
                HelpMyStreet.Utils.Enums.Groups.LincolnshireVCS => "If you would like to request help as Lincolnshire VCS (VCS), complete this form to make your request visible to the pool of Lincolnshire Volunteers.\r\n\r\nIf you would like to log a request as another partner organisations or the 'parent group' (Lincolnshire Volunteers) please use the relevant request form or email mailto:contact@helpmystreet.org if you require access.",
                _ => throw new ArgumentException($"Unexpected ReferringGroup value: {referringGroupId}", nameof(referringGroupId))
            };
        }

        private string GetHelpRequestPageTitle(RequestHelpFormVariant requestHelpFormVariant, int referringGroupId)
        {
            return requestHelpFormVariant switch
            {
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
                RequestHelpFormVariant.AgeUKMidMersey_RequestSubmitter => "Request Help from Age UK Mid Mersey",
                RequestHelpFormVariant.BostonGNS_Public => "Request Help from Boston Good Neighbour Scheme",
                RequestHelpFormVariant.BostonGNS_RequestSubmitter => "Request Help from Boston Good Neighbour Scheme",
                RequestHelpFormVariant.LincolnshireVolunteersRequests_RequestSubmitter => GetLincolnshirePageTitle(referringGroupId),
                RequestHelpFormVariant.NHSVRDemo_RequestSubmitter => "DEMO Request Form",
                _ => "What type of help are you looking for?"
            };
        }

        private string GetHelpRequestPageIntroText(RequestHelpFormVariant requestHelpFormVariant, int referringGroupId)
        {
            return requestHelpFormVariant switch
            {
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
                RequestHelpFormVariant.AgeUKMidMersey_RequestSubmitter => "If you need help from Age UK Mid Mersey, complete this form to let us know what you need. We'll give you a call back within two working days to let you know how we can help.",
                RequestHelpFormVariant.BostonGNS_Public => "If you need help in Boston complete this form to let us know what you need.\r\n\r\nPlease remember, Good Neighbour Schemes do not replace the work/services provided by Adult Social Care or other professional care agencies and should not be seen as a free or cheap way to do skilled tasks that require the use of qualified trades people. No tasks are undertaken that require certified qualification such as electrical, gas or plumbing work. Such work is normally beyond the scope of Good Neighbour Schemes and their insurance cover.",
                RequestHelpFormVariant.BostonGNS_RequestSubmitter => "If you need help in Boston complete this form to let us know what you need.\r\n\r\nPlease remember, Good Neighbour Schemes do not replace the work/services provided by Adult Social Care or other professional care agencies and should not be seen as a free or cheap way to do skilled tasks that require the use of qualified trades people. No tasks are undertaken that require certified qualification such as electrical, gas or plumbing work. Such work is normally beyond the scope of Good Neighbour Schemes and their insurance cover.",
                RequestHelpFormVariant.LincolnshireVolunteersRequests_RequestSubmitter => GetLincolnshirePageIntroText(referringGroupId),
                RequestHelpFormVariant.NHSVRDemo_RequestSubmitter => "Requests made through **this form** will be available within the Sandbox area of HelpMyStreet **for demonstration and testing purposes only** and will not trigger notifications to general users of HelpMyStreet.\r\n\r\n**Please ensure you can see this message whenever you wish to submit a DEMO / test request.**",
                _ => "People across the country are helping their neighbours and community to stay safe. Whatever you need, we have people who can help."
            };
        }

        private string GetHelpRequestPageHeadingClass(RequestHelpFormVariant requestHelpFormVariant)
        {
            return requestHelpFormVariant switch
            {
                RequestHelpFormVariant.NHSVRDemo_RequestSubmitter => "sandbox-warning p-sm-md mt-sm",
                _ => ""
            };
        }

        private List<TasksViewModel> GetRequestHelpTasks(RequestHelpFormVariant requestHelpFormVariant)
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
            else if (requestHelpFormVariant == RequestHelpFormVariant.Ruddington)
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
            else if (requestHelpFormVariant == RequestHelpFormVariant.AgeUKMidMersey_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping},
                    new TasksViewModel { SupportActivity = SupportActivities.PracticalSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.CheckingIn},
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions},
                    new TasksViewModel { SupportActivity = SupportActivities.Errands},
                    new TasksViewModel { SupportActivity = SupportActivities.WellbeingPackage},
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly},
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking},
                    new TasksViewModel { SupportActivity = SupportActivities.BinDayAssistance},
                    new TasksViewModel { SupportActivity = SupportActivities.DigitalSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.InPersonBefriending},
                    new TasksViewModel { SupportActivity = SupportActivities.Covid19Help},
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.ColdWeatherArmy},
                    new TasksViewModel { SupportActivity = SupportActivities.SkillShare},
                    new TasksViewModel { SupportActivity = SupportActivities.Other},

                });                   
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.BostonGNS_Public)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping},
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly},
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions},
                    new TasksViewModel { SupportActivity = SupportActivities.PracticalSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking},
                    new TasksViewModel { SupportActivity = SupportActivities.DigitalSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.Other}
                });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.BostonGNS_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.Shopping},
                    new TasksViewModel { SupportActivity = SupportActivities.PhoneCalls_Friendly},
                    new TasksViewModel { SupportActivity = SupportActivities.CollectingPrescriptions},
                    new TasksViewModel { SupportActivity = SupportActivities.PracticalSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.DogWalking},
                    new TasksViewModel { SupportActivity = SupportActivities.DigitalSupport},
                    new TasksViewModel { SupportActivity = SupportActivities.Other},
                    new TasksViewModel { SupportActivity = SupportActivities.VolunteerSupport}
                });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.LincolnshireVolunteersRequests_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.AdvertisingRoles },
                    new TasksViewModel { SupportActivity = SupportActivities.Other },
                 });
            }
            else if (requestHelpFormVariant == RequestHelpFormVariant.NHSVRDemo_RequestSubmitter)
            {
                tasks.AddRange(new List<TasksViewModel>
                {
                    new TasksViewModel { SupportActivity = SupportActivities.NHSTransport },
                    new TasksViewModel { SupportActivity = SupportActivities.NHSCheckInAndChat },
                    new TasksViewModel { SupportActivity = SupportActivities.NHSCheckInAndChatPlus },
                    new TasksViewModel { SupportActivity = SupportActivities.NHSSteward },
                    new TasksViewModel { SupportActivity = SupportActivities.EmergencySupport },                    
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
                RequestHelpFormVariant.AgeUKSouthKentCoast_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, true),
                RequestHelpFormVariant.AgeUKFavershamAndSittingbourne_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, true),
                RequestHelpFormVariant.AgeUKNorthWestKent_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.On }, true),
                RequestHelpFormVariant.MeadowsCommunityHelpers_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, true),
                RequestHelpFormVariant.AgeConnectsCardiff_Public => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.Before, DueDateType.On }, true),
                RequestHelpFormVariant.AgeConnectsCardiff_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.On }, true),
                RequestHelpFormVariant.AgeUKMidMersey_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.On }, true),

                RequestHelpFormVariant.LincolnshireVolunteers => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.SpecificStartAndEndTimes }, false),
                RequestHelpFormVariant.ApexBankStaff_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.SpecificStartAndEndTimes }, false),
                RequestHelpFormVariant.Sandbox_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.SpecificStartAndEndTimes }, true),
                RequestHelpFormVariant.LincolnshireVolunteersRequests_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.On, DueDateType.OpenUntil },true),
                RequestHelpFormVariant.NHSVRDemo_RequestSubmitter => GetRequestHelpTimeViewModels(new List<DueDateType> { DueDateType.ASAP, DueDateType.Before, DueDateType.SpecificStartAndEndTimes }, true),
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
                RequestHelpFormVariant.VitalsForVeterans => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation }),
                RequestHelpFormVariant.LincolnshireVolunteers => GetRequestorViewModels(new List<RequestorType> { RequestorType.Organisation }),
                RequestHelpFormVariant.MeadowsCommunityHelpers_RequestSubmitter => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation }),
                RequestHelpFormVariant.AgeConnectsCardiff_RequestSubmitter => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation }),
                RequestHelpFormVariant.ApexBankStaff_RequestSubmitter => GetRequestorViewModels(new List<RequestorType> { RequestorType.Organisation }),
                RequestHelpFormVariant.AgeUKMidMersey_RequestSubmitter => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation }),
                RequestHelpFormVariant.LincolnshireVolunteersRequests_RequestSubmitter => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation }),
                RequestHelpFormVariant.NHSVRDemo_RequestSubmitter => GetRequestorViewModels(new List<RequestorType> { RequestorType.OnBehalf, RequestorType.Organisation }),
                _ => GetRequestorViewModels(new List<RequestorType> { RequestorType.Myself, RequestorType.OnBehalf }),
            };
        }

        private List<FrequencyViewModel> GetFrequencies(RequestHelpFormVariant variant)
        {
            if (variant == RequestHelpFormVariant.LincolnshireVolunteersRequests_RequestSubmitter)
            {
                return new List<FrequencyViewModel>()
                {
                    new FrequencyViewModel(Frequency.Once, new List<SupportActivities>{SupportActivities.AdvertisingRoles}),
                    new FrequencyViewModel(Frequency.Daily, new List<SupportActivities>{SupportActivities.AdvertisingRoles}),
                    new FrequencyViewModel(Frequency.Weekly, new List<SupportActivities>{SupportActivities.AdvertisingRoles}),
                    new FrequencyViewModel(Frequency.Fortnightly, new List<SupportActivities>{SupportActivities.AdvertisingRoles}),
                    new FrequencyViewModel(Frequency.EveryFourWeeks, new List<SupportActivities>{SupportActivities.AdvertisingRoles}),
                    new FrequencyViewModel(Frequency.Ongoing, new List<SupportActivities>{SupportActivities.Other})
                };
            }
            else
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
