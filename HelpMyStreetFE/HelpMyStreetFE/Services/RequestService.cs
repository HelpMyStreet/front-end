using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using System.Linq;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreetFE.Models.Account;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Contracts.RequestService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using HelpMyStreetFE.Models.RequestHelp.Stages;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IOptions<RequestSettings> _requestSettings;
        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IOptions<RequestSettings> requestSettings)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestSettings = requestSettings;
        }
        //public async Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpViewModel viewModel, int userId, HttpContext ctx)
        //{
        //    _logger.LogInformation($"Logging Request");
        //    var request = new PostNewRequestForHelpRequest
        //    {
        //        HelpRequest = new HelpRequest {                                     
        //            AcceptedTerms = viewModel.HelpRequest.AcceptedTerms,
        //            OtherDetails = viewModel.HelpRequest.OtherDetails,
        //            ConsentForContact = viewModel.HelpRequest.ConsentForContact,
        //            SpecialCommunicationNeeds = viewModel.HelpRequest.SpecialCommunicationNeeds,
        //            ForRequestor = viewModel.HelpRequest.ForRequestor,
        //            ReadPrivacyNotice = viewModel.HelpRequest.ReadPrivacyNotice,
        //            CreatedByUserId = userId,
        //            Recipient = new RequestPersonalDetails
        //            {
        //                FirstName = viewModel.HelpRequest.Recipient.Firstname,
        //                LastName = viewModel.HelpRequest.Recipient.Lastname,
        //                MobileNumber = viewModel.HelpRequest.Recipient.Mobile,
        //                OtherNumber = viewModel.HelpRequest.Recipient.AltNumber,
        //                EmailAddress = viewModel.HelpRequest.Recipient.Email,
        //                Address = new Address
        //                {
        //                    AddressLine1 = viewModel.HelpRequest.Recipient.Address.Addressline1,
        //                    AddressLine2 = viewModel.HelpRequest.Recipient.Address.Addressline2,
        //                    Locality = viewModel.HelpRequest.Recipient.Address.Locality,
        //                    Postcode = PostcodeFormatter.FormatPostcode(viewModel.HelpRequest.Recipient.Address.Postcode),
        //                }
        //            },
        //            Requestor = new RequestPersonalDetails
        //            {
        //                FirstName = viewModel.HelpRequest.Requestor.Firstname,
        //                LastName = viewModel.HelpRequest.Requestor.Lastname,
        //                MobileNumber = viewModel.HelpRequest.Requestor.Mobile,
        //                OtherNumber = viewModel.HelpRequest.Requestor.AltNumber,
        //                EmailAddress = viewModel.HelpRequest.Requestor.Email,
        //                Address = new Address
        //                {
        //                    AddressLine1 = viewModel.HelpRequest.Requestor.Address.Addressline1,
        //                    AddressLine2 = viewModel.HelpRequest.Requestor.Address.Addressline2,
        //                    Locality = viewModel.HelpRequest.Requestor.Address.Locality,
        //                    Postcode = PostcodeFormatter.FormatPostcode(viewModel.HelpRequest.Requestor.Address.Postcode),
        //                }
        //            }
        //        },
        //        NewJobsRequest = new NewJobsRequest
        //        {
        //            Jobs = new List<Job>
        //            {
        //                new Job
        //                {
        //                    DueDays = viewModel.JobRequest.DueDays,
        //                    Details = viewModel.JobRequest.Details,
        //                    HealthCritical = viewModel.JobRequest.HealthCritical,
        //                    SupportActivity = (SupportActivities)Enum.Parse(typeof(SupportActivities), viewModel.JobRequest.SupportActivity),
        //                }
        //            }
        //        }
        //    };

        //    var response = await _requestHelpRepository.PostNewRequestForHelpAsync(request);
        //    if (response.HasContent & response.IsSuccessful)
        //        TriggerCacheRefresh(ctx);


        //    return response;
        //}
        public async Task<OpenJobsViewModel> GetOpenJobsAsync(double distanceInMiles, User user, HttpContext ctx)
        {               
                var jobs = ctx.Session.GetObjectFromJson<OpenJobsViewModel>("openJobs");
                DateTime lastUpdated;
                DateTime.TryParse(ctx.Session.GetString("openJobsLastUpdated"), out lastUpdated);
                if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now)                
                {
                    var all = await _requestHelpRepository.GetJobsByFilterAsync(user.PostalCode, distanceInMiles);

                    var (criteriaJobs, otherJobs) = all.Split(x => user.SupportActivities.Contains(x.SupportActivity) && x.DistanceInMiles < user.SupportRadiusMiles);

                    jobs = new OpenJobsViewModel
                    {
                        CriteriaJobs = criteriaJobs.OrderOpenJobsForDisplay(),
                        OtherJobs = otherJobs.OrderOpenJobsForDisplay()
                    };
                    ctx.Session.SetObjectAsJson("openJobs", jobs);
                    ctx.Session.SetString("openJobsLastUpdated", DateTime.Now.ToString());
                }
                                           
            return jobs;          
        }
  
        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, HttpContext ctx)
        {                           
            var jobs = ctx.Session.GetObjectFromJson<IEnumerable<JobSummary>>("acceptedJobs");                        
            DateTime lastUpdated;
            DateTime.TryParse(ctx.Session.GetString("acceptedJobsLastUpdated"), out lastUpdated);
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now) {
                jobs = (await _requestHelpRepository.GetJobsAllocatedToUserAsync(userId)).OrderOpenJobsForDisplay();           
                ctx.Session.SetObjectAsJson("acceptedJobs", jobs);
                ctx.Session.SetString("acceptedJobsLastUpdated", DateTime.Now.ToString());
            }

            return jobs;
    }


        public async Task<IDictionary<int, RequestContactInformation>> GetContactInformationForRequests(IEnumerable<int> ids)
        {
            List<GetJobDetailsResponse> details = new List<GetJobDetailsResponse>();

            foreach (var id in ids) {
                details.Add(await _requestHelpRepository.GetJobDetailsAsync(id));
            }

            return details.Aggregate(new Dictionary<int, RequestContactInformation>(), (acc, cur) =>
            {
                acc[cur.JobID] = new RequestContactInformation
                {
                    ForRequestor = cur.ForRequestor,
                    JobID = cur.JobID,
                    Recipient = cur.Recipient,
                    Requestor = cur.Requestor
                };

                return acc;
            });
        }

        public async Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId)
        {
            return await _requestHelpRepository.GetJobDetailsAsync(jobId);
        }
        public async Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, HttpContext ctx)
        {
            var success =  await _requestHelpRepository.UpdateJobStatusToDoneAsync(new PutUpdateJobStatusToDoneRequest()
            {
                JobID = jobID,
                CreatedByUserID = createdByUserId
            });

            if (success)
                TriggerCacheRefresh(ctx);

            return success;
        }
        public async Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId, HttpContext ctx)
        {
            var success =  await _requestHelpRepository.UpdateJobStatusToOpenAsync(new PutUpdateJobStatusToOpenRequest()
            {
                CreatedByUserID = createdByUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(ctx);

            return success;
        }
        public async Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, HttpContext ctx)
        {
            
            var success = await _requestHelpRepository.UpdateJobStatusToInProgressAsync(new PutUpdateJobStatusToInProgressRequest()
            {
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId,
                JobID = jobID
            });

            if (success)
             TriggerCacheRefresh(ctx);
            
            return success;
        }


        private void TriggerCacheRefresh(HttpContext ctx)
        {
            int triggerSessionMinutes = (_requestSettings.Value.RequestsSessionExpiryInMinutes + 1) * -1;            
            ctx.Session.SetString("acceptedJobsLastUpdated", DateTime.Now.AddMinutes(triggerSessionMinutes).ToString());
            ctx.Session.SetString("openJobsLastUpdated", DateTime.Now.AddMinutes(triggerSessionMinutes).ToString());
        }


        public RequestHelpNewViewModel GetRequestHelpSteps()
        {
            return new RequestHelpNewViewModel
            {

                CurrentStepIndex = 0,
                Steps = new List<IRequestHelpStageViewModel>
                {
                    new RequestHelpRequestStageViewModel
                    {
                        Tasks = GetRequestHelpTasks(),
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
                }

            };
        }
        private List<TasksViewModel> GetRequestHelpTasks()
        {
            return new List<TasksViewModel>
                {
                    new TasksViewModel
                    {
                        ID = 1,
                        SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Shopping,
                        Questions = new List<RequestHelpQuestion>
                        {
                            new RequestHelpQuestion
                            {
                                ID=1,
                                Label = "Question 1",
                                InputType = InputType.Textarea
                                },
                            new RequestHelpQuestion
                            {
                                ID = 2,
                                Label = "Question 2",
                                InputType = InputType.Number
                            }
                        }
                    },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.FaceMask },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.CheckingIn },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Errands },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.MealPreparation },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.PhoneCalls_Anxious },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.HomeworkSupport },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Other },                
                };
            }
        }
    }



