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
using System.Linq;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreet.Utils.Extensions;
using HelpMyStreet.Contracts.RequestService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Email;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IOptions<RequestSettings> _requestSettings;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        private readonly IGroupService _groupService;
        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IOptions<RequestSettings> requestSettings, IRequestHelpBuilder requestHelpBuilder, IGroupService groupService)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestSettings = requestSettings;
            _requestHelpBuilder = requestHelpBuilder;
            _groupService = groupService;
        }

        public async Task<BaseRequestHelpResponse<LogRequestResponse>> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, HttpContext ctx)
        {
            _logger.LogInformation($"Logging Request");
            var recipient = _requestHelpBuilder.MapRecipient(detailStage);
            var requestor = detailStage.Type == RequestorType.OnBehalf || detailStage.Type == RequestorType.Organisation ? _requestHelpBuilder.MapRequestor(detailStage) : recipient;
            var selectedTask = requestStage.Tasks.Where(x => x.IsSelected).First();
            var selectedTime = requestStage.Timeframes.Where(x => x.IsSelected).FirstOrDefault();

            bool heathCritical = false;
            var healthCriticalQuestion = requestStage.Questions.Questions.Where(a => a.ID == (int)Questions.IsHealthCritical).FirstOrDefault();
            if (healthCriticalQuestion != null && healthCriticalQuestion.Model == "true") { heathCritical = true; }

            IEnumerable<RequestHelpQuestion> questions = requestStage.Questions.Questions.Union(detailStage.Questions.Questions);

            var request = new PostNewRequestForHelpRequest
            {
                HelpRequest = new HelpRequest
                {
                    AcceptedTerms = requestStage.AgreeToTerms,
                    ConsentForContact = requestStage.AgreeToTerms,
                    OrganisationName = detailStage.Organisation ?? "",
                    RequestorType = detailStage.Type,
                    ReadPrivacyNotice = requestStage.AgreeToPrivacy,
                    CreatedByUserId = userId,
                    Recipient = recipient,
                    Requestor = requestor,
                    ReferringGroupId = referringGroupID,
                    Source = source
                },
                NewJobsRequest = new NewJobsRequest
                {
                    Jobs = new List<Job>
                    {
                        new Job
                        {
                            DueDays = selectedTime.Days,
                            Details = "",
                            HealthCritical = heathCritical,
                            SupportActivity = selectedTask.SupportActivity,
                            Questions = questions.Where(x => x.InputType != QuestionType.LabelOnly).Select(x => new Question {
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
        public async Task<OpenJobsViewModel> GetOpenJobsAsync(double distanceInMiles, int maxOtherJobsToDisplay, User user, HttpContext ctx)
        {
            if (user.PostalCode == null)
            {
                throw new Exception("Cannot identify jobs without user postcode");
            }

            var jobs = ctx.Session.GetObjectFromJson<OpenJobsViewModel>("openJobs");
            DateTime lastUpdated;
            DateTime.TryParse(ctx.Session.GetString("openJobsLastUpdated"), out lastUpdated);
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now)
            {
                var nationalSupportActivities = new List<SupportActivities>() { SupportActivities.FaceMask, SupportActivities.HomeworkSupport, SupportActivities.PhoneCalls_Anxious, SupportActivities.PhoneCalls_Friendly };
                var activitySpecificSupportDistancesInMiles = nationalSupportActivities.Where(a => user.SupportActivities.Contains(a)).ToDictionary(a => a, a => (double?)null);
                var jobsByFilterRequest = new GetJobsByFilterRequest()
                {
                    Postcode = user.PostalCode,
                    DistanceInMiles = distanceInMiles,
                    ActivitySpecificSupportDistancesInMiles = activitySpecificSupportDistancesInMiles,
                    JobStatuses = new JobStatusRequest()
                    {
                        JobStatuses = new List<JobStatuses>() { JobStatuses.Open }
                    },
                    Groups = new GroupRequest() { Groups = await _groupService.GetUserGroups(user.ID) }
                };

                var all = await _requestHelpRepository.GetJobsByFilterAsync(jobsByFilterRequest);

                var (criteriaJobs, otherJobs) = all.Split(x => user.SupportActivities.Contains(x.SupportActivity) && x.DistanceInMiles <= user.SupportRadiusMiles);

                jobs = new OpenJobsViewModel
                {
                    CriteriaJobs = criteriaJobs.OrderOpenJobsForDisplay(),
                    OtherJobs = otherJobs.OrderOpenJobsForDisplay().Take(maxOtherJobsToDisplay)
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
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now)
            {
                jobs = (await _requestHelpRepository.GetJobsAllocatedToUserAsync(userId)).OrderOpenJobsForDisplay();
                ctx.Session.SetObjectAsJson("acceptedJobs", jobs);
                ctx.Session.SetString("acceptedJobsLastUpdated", DateTime.Now.ToString());
            }

            return jobs;
        }


        public async Task<GetJobDetailsResponse> GetJobDetailsAsync(int jobId, int userId)
        {
            return await _requestHelpRepository.GetJobDetailsAsync(jobId, userId);
        }
        public async Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, HttpContext ctx)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToDoneAsync(new PutUpdateJobStatusToDoneRequest()
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
            var success = await _requestHelpRepository.UpdateJobStatusToOpenAsync(new PutUpdateJobStatusToOpenRequest()
            {
                CreatedByUserID = createdByUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(ctx);

            return success;
        }
        public async Task<bool> UpdateJobStatusToCancelledAsync(int jobID, int createdByUserId, HttpContext ctx)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToCancelledAsync(new PutUpdateJobStatusToCancelledRequest()
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

        public async Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source)
        {
            return await _requestHelpBuilder.GetSteps(requestHelpFormVariant, referringGroupID, source);
        }

        public async Task<IEnumerable<JobSummary>> GetGroupRequestsAsync(int GroupId, JobFilterRequest jobFilterRequest, HttpContext ctx)
        {
            var jobs = ctx.Session.GetObjectFromJson<IEnumerable<JobSummary>>($"group{GroupId}Jobs");
            DateTime lastUpdated;
            DateTime.TryParse(ctx.Session.GetString($"group{GroupId}JobsLastUpdated"), out lastUpdated);
            if (jobs == null || lastUpdated.AddMinutes(_requestSettings.Value.RequestsSessionExpiryInMinutes) < DateTime.Now)
            {
                var jobsByFilterRequest = new GetJobsByFilterRequest()
                {
                    ReferringGroupID = GroupId
                };

                jobs = await _requestHelpRepository.GetJobsByFilterAsync(jobsByFilterRequest);

                ctx.Session.SetObjectAsJson($"group{GroupId}Jobs", jobs);
                ctx.Session.SetString($"group{GroupId}JobsLastUpdated", DateTime.Now.ToString());
            }
            if (jobFilterRequest != null)
            {
                jobs = FilterJobs(jobs, jobFilterRequest);
            }
            return jobs;
        }

        private IEnumerable<JobSummary> FilterJobs(IEnumerable<JobSummary> jobs, JobFilterRequest jobFilterRequest)
        {
            if (jobFilterRequest.JobStatuses != null)
            {
                jobs = jobs.Where(j => jobFilterRequest.JobStatuses.Contains(j.JobStatus));
            }
            if (jobFilterRequest.SupportActivities != null)
            {
                jobs = jobs.Where(j => jobFilterRequest.SupportActivities.Contains(j.SupportActivity));
            }
            if (jobFilterRequest.MaxDistanceInMiles != null)
            {
                jobs = jobs.Where(j => j.DistanceInMiles <= jobFilterRequest.MaxDistanceInMiles);
            }
            if (jobFilterRequest.DueAfter != null)
            {
                jobs = jobs.Where(j => j.DueDate.Date >= jobFilterRequest.DueAfter?.Date);
            }
            if (jobFilterRequest.DueBefore != null)
            {
                jobs = jobs.Where(j => j.DueDate.Date <= jobFilterRequest.DueBefore?.Date);
            }
            if (jobFilterRequest.RequestedAfter != null)
            {
                jobs = jobs.Where(j => j.DateRequested.Date >= jobFilterRequest.RequestedAfter?.Date);
            }
            if (jobFilterRequest.RequestedBefore != null)
            {
                jobs = jobs.Where(j => j.DateRequested.Date <= jobFilterRequest.RequestedBefore?.Date);
            }
            return jobs;
        }
    }
 }



