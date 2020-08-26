using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
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
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreetFE.Models.Account;
using Microsoft.Extensions.Options;
using HelpMyStreetFE.Models.Email;

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMemDistCache<IEnumerable<JobSummary>> _memDistCache;
        private readonly IOptions<RequestSettings> _requestSettings;

        private const string CACHE_KEY_PREFIX = "request-service-jobs";

        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IRequestHelpBuilder requestHelpBuilder, IGroupService groupService, IUserService userService, IMemDistCache<IEnumerable<JobSummary>> memDistCache, IOptions<RequestSettings> requestSettings)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestHelpBuilder = requestHelpBuilder;
            _groupService = groupService;
            _userService = userService;
            _memDistCache = memDistCache;
            _requestSettings = requestSettings;
        }

        public async Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken)
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
            if (response != null && userId != 0)
                TriggerCacheRefresh(userId, cancellationToken);

            return response;
        }

        public async Task<OpenJobsViewModel> GetOpenJobsAsync(User user, CancellationToken cancellationToken)
        {
            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetOpenJobsForUserFromRepo(user);
            }, $"{CACHE_KEY_PREFIX}-user-{user.ID}-open-jobs", RefreshBehaviour.WaitForFreshData, cancellationToken);

            var (criteriaJobs, otherJobs) = jobs.Split(x => user.SupportActivities.Contains(x.SupportActivity) && x.DistanceInMiles <= user.SupportRadiusMiles);

            return new OpenJobsViewModel
            {
                CriteriaJobs = criteriaJobs.OrderOpenJobsForDisplay(),
                OtherJobs = otherJobs.OrderOpenJobsForDisplay().Take(_requestSettings.Value.MaxNonCriteriaOpenJobsToDisplay)
            };
        }

        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, CancellationToken cancellationToken)
        {
            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { UserID = userId });
            }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", RefreshBehaviour.WaitForFreshData, cancellationToken);

            return jobs.OrderOpenJobsForDisplay();
        }


        public async Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, CancellationToken cancellationToken)
        {
            var getJobDetailsResponse = await _requestHelpRepository.GetJobDetailsAsync(jobId, userId);
            var getStatusHistoryResponse = await _requestHelpRepository.GetJobStatusHistoryAsync(jobId);

            User currentVolunteer = null;
            if (getJobDetailsResponse?.JobSummary?.VolunteerUserID != null)
            {
                currentVolunteer = await _userService.GetUserAsync(getJobDetailsResponse.JobSummary.VolunteerUserID.Value);
            }

            if (getJobDetailsResponse != null && getStatusHistoryResponse != null)
            {
                return new JobDetail()
                {
                    JobSummary = getJobDetailsResponse.JobSummary,
                    Recipient = getJobDetailsResponse.Recipient,
                    Requestor = getJobDetailsResponse.Requestor,
                    JobStatusHistory = getStatusHistoryResponse.History,
                    CurrentVolunteer = currentVolunteer,
                };
            }
            return null;
        }

        public async Task<bool> UpdateJobStatusToDoneAsync(int jobID, int createdByUserId, CancellationToken cancellationToken)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToDoneAsync(new PutUpdateJobStatusToDoneRequest()
            {
                JobID = jobID,
                CreatedByUserID = createdByUserId
            });

            if (success)
                TriggerCacheRefresh(createdByUserId, cancellationToken);

            return success;
        }
        public async Task<bool> UpdateJobStatusToOpenAsync(int jobID, int createdByUserId, CancellationToken cancellationToken)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToOpenAsync(new PutUpdateJobStatusToOpenRequest()
            {
                CreatedByUserID = createdByUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(createdByUserId, cancellationToken);

            return success;
        }
        public async Task<bool> UpdateJobStatusToCancelledAsync(int jobID, int createdByUserId, CancellationToken cancellationToken)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToCancelledAsync(new PutUpdateJobStatusToCancelledRequest()
            {
                CreatedByUserID = createdByUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(createdByUserId, cancellationToken);

            return success;
        }
        public async Task<bool> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, CancellationToken cancellationToken)
        {
            var success = await _requestHelpRepository.UpdateJobStatusToInProgressAsync(new PutUpdateJobStatusToInProgressRequest()
            {
                CreatedByUserID = createdByUserId,
                VolunteerUserID = volunteerUserId,
                JobID = jobID
            });

            if (success)
                TriggerCacheRefresh(createdByUserId, cancellationToken);

            return success;
        }

        public async Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source)
        {
            return await _requestHelpBuilder.GetSteps(requestHelpFormVariant, referringGroupID, source);
        }

        public async Task<IEnumerable<JobSummary>> GetGroupRequestsAsync(int groupId, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { ReferringGroupID = groupId });
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}", RefreshBehaviour.WaitForFreshData, cancellationToken);
        }

        public IEnumerable<JobSummary> FilterJobs(IEnumerable<JobSummary> jobs, JobFilterRequest jfr)
        {
            return jobs.Where( 
                j =>    (jfr.JobStatuses == null        || jfr.JobStatuses.Contains(j.JobStatus))
                    &&  (jfr.SupportActivities == null  || jfr.SupportActivities.Contains(j.SupportActivity))
                    &&  (jfr.MaxDistanceInMiles == null || j.DistanceInMiles <= jfr.MaxDistanceInMiles)
                    &&  (jfr.DueInNextXDays == null     || j.DueDate.Date <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    &&  (jfr.DueAfter == null           || j.DueDate.Date >= jfr.DueAfter?.Date)
                    &&  (jfr.DueBefore == null          || j.DueDate.Date <= jfr.DueBefore?.Date)
                    &&  (jfr.RequestedAfter == null     || j.DateRequested.Date >= jfr.RequestedAfter?.Date)
                    &&  (jfr.RequestedBefore != null)   || j.DateRequested.Date <= jfr.RequestedBefore?.Date);
        }

        private void TriggerCacheRefresh(int userId, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { UserID = userId });
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", cancellationToken);


                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetOpenJobsForUserFromRepo(await _userService.GetUserAsync(userId));
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-open-jobs", cancellationToken);


                List<UserGroup> userGroups = await _groupService.GetUserGroupRoles(userId);
                if (userGroups != null)
                {
                    userGroups.Where(g => g.UserRoles.Contains(GroupRoles.TaskAdmin)).ToList().ForEach(async g => {
                        await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                        {
                            return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { ReferringGroupID = g.GroupId });
                        }, $"{CACHE_KEY_PREFIX}-group-{g.GroupId}", cancellationToken);
                    });
                }
            });
        }

        private async Task<IEnumerable<JobSummary>> GetOpenJobsForUserFromRepo(User user)
        {
            if (user.PostalCode == null)
            {
                return null;
            }

            var nationalSupportActivities = new List<SupportActivities>() { SupportActivities.FaceMask, SupportActivities.HomeworkSupport, SupportActivities.PhoneCalls_Anxious, SupportActivities.PhoneCalls_Friendly, SupportActivities.CommunityConnector };
            var activitySpecificSupportDistancesInMiles = nationalSupportActivities.Where(a => user.SupportActivities.Contains(a)).ToDictionary(a => a, a => (double?)null);
            var jobsByFilterRequest = new GetJobsByFilterRequest()
            {
                Postcode = user.PostalCode,
                DistanceInMiles = _requestSettings.Value.OpenRequestsRadius,
                ActivitySpecificSupportDistancesInMiles = activitySpecificSupportDistancesInMiles,
                JobStatuses = new JobStatusRequest()
                {
                    JobStatuses = new List<JobStatuses>() { JobStatuses.Open }
                },
                Groups = new GroupRequest() { Groups = await _groupService.GetUserGroups(user.ID) }
            };

            return await _requestHelpRepository.GetJobsByFilterAsync(jobsByFilterRequest);
        }
    }
}



