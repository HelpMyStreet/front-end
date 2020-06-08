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
using HelpMyStreetFE.Models.RequestHelp.Stages.Review;

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
                    Postcode = PostcodeFormatter.FormatPostcode(detailStage.Recipient.Postcode),
                }
            };
        }

        public async Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int userId, HttpContext ctx)
        {
            _logger.LogInformation($"Logging Request");
            var recipient = MapRecipient(detailStage);
            var requestor = detailStage.Type == RequestorType.OnBehalf ? MapRequestor(detailStage) : recipient;
            var selectedTask = requestStage.Tasks.Where(x => x.IsSelected).First();
            var selectedTime = requestStage.Timeframes.Where(x => x.IsSelected).FirstOrDefault();
            var request = new PostNewRequestForHelpRequest
            {
                HelpRequest = new HelpRequest
                {
                    AcceptedTerms = requestStage.AgreeToTerms,
                    OtherDetails = detailStage.OtherDetails,
                    ConsentForContact = requestStage.AgreeToTerms,
                    SpecialCommunicationNeeds = detailStage.CommunicationNeeds,
                    RequestorType = detailStage.Type,
                    ForRequestor = detailStage.Type == RequestorType.Myself ? true : false,
                    ReadPrivacyNotice = requestStage.AgreeToPrivacy,
                    CreatedByUserId = userId,
                    Recipient = recipient,
                    Requestor = requestor,
                },
                NewJobsRequest = new NewJobsRequest
                {
                    Jobs = new List<Job>
                    {
                        new Job
                        {
                            DueDays = selectedTime.Days,
                            Details = "",
                            HealthCritical = requestStage.IsHealthCritical.HasValue ? requestStage.IsHealthCritical.Value : false,
                            SupportActivity = selectedTask.SupportActivity,
                            Questions = selectedTask.Questions.Select(x => new Question {
                                Id = x.ID,
                                Answer = x.InputType == QuestionType.Radio ? x.AdditionalData.Where(a => a.Key == x.Model).FirstOrDefault()?.Value ?? "" : x.Model,
                                Name = x.Label,
                                Required = x.Required,
                                AddtitonalData = x.AdditionalData,
                                Type  = x.InputType}).ToList()
                        }
                    }
                }
            };


            var response = await _requestHelpRepository.PostNewRequestForHelpAsync(request);
               if (response.HasContent & response.IsSuccessful)
                    TriggerCacheRefresh(ctx);


               return response;
        }
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


        public async Task<RequestHelpViewModel> GetRequestHelpSteps(string source)
        {
            
            return new RequestHelpViewModel
            {

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
                                IconDark = "question-mark.svg",
                                IconLight = "question-mark.svg",
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
        }
        private async Task<List<TasksViewModel>> GetRequestHelpTasks(string source)
        {
            var tasks = new List<TasksViewModel>();
            if (source == "v4v")
                tasks.Add(new TasksViewModel { ID = 11, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.WellbeingPackage });

            tasks.AddRange(new List<TasksViewModel>
            {
                    new TasksViewModel { ID = 1,SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Shopping },
                    new TasksViewModel { ID = 2, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.FaceMask },
                    new TasksViewModel { ID = 3, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.CheckingIn },
                    new TasksViewModel { ID = 4, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.CollectingPrescriptions },
                    new TasksViewModel { ID = 5, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Errands },
                    new TasksViewModel { ID = 6, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.MealPreparation },
                    new TasksViewModel { ID = 7, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.PhoneCalls_Friendly },
                    new TasksViewModel { ID = 8, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.PhoneCalls_Anxious },
                    new TasksViewModel { ID = 9, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.HomeworkSupport },
                    new TasksViewModel { ID = 10, SupportActivity = HelpMyStreet.Utils.Enums.SupportActivities.Other },
             });
            
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
                AdditionalData = x.AddtitonalData                
            }).ToList()) ;

            return tasks;
         }

    
    }

    }



