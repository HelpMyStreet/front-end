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
        private readonly IMemDistCache<IEnumerable<JobHeader>> _memDistCache;
        private readonly IOptions<RequestSettings> _requestSettings;

        private const string CACHE_KEY_PREFIX = "request-service-jobs";

        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IRequestHelpBuilder requestHelpBuilder, IGroupService groupService, IUserService userService, IMemDistCache<IEnumerable<JobHeader>> memDistCache, IOptions<RequestSettings> requestSettings)
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

        public async Task<IEnumerable<JobHeader>> GetOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken)
        {
            RefreshBehaviour refreshBehaviour = waitForData ? RefreshBehaviour.WaitForFreshData : RefreshBehaviour.DontWaitForFreshData;
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetOpenJobsForUserFromRepo(user);
            }, $"{CACHE_KEY_PREFIX}-user-{user.ID}-open-jobs", refreshBehaviour, cancellationToken, notInCacheBehaviour);

            return jobs;
        }

        public OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<JobHeader> jobs)
        {
            if (jobs == null)
            {
                return null;
            }

            var (criteriaJobs, otherJobs) = jobs.Split(x => user.SupportActivities.Contains(x.SupportActivity) && x.DistanceInMiles <= user.SupportRadiusMiles);

            return new OpenJobsViewModel
            {
                CriteriaJobs = criteriaJobs.OrderOpenJobsForDisplay(),
                OtherJobs = otherJobs.OrderOpenJobsForDisplay()
            };
        }

        public async Task<IEnumerable<JobHeader>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            RefreshBehaviour refreshBehaviour = waitForData ? RefreshBehaviour.WaitForFreshData : RefreshBehaviour.DontWaitForFreshData;
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { UserID = userId });
            }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", refreshBehaviour, cancellationToken, notInCacheBehaviour);

            return jobs?.OrderOpenJobsForDisplay();
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            return await _requestHelpRepository.GetJobSummaryAsync(jobId);
        }

        public async Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, CancellationToken cancellationToken)
        {
            var jobDetails = await _requestHelpRepository.GetJobDetailsAsync(jobId, userId);

            if (jobDetails != null)
            {
                User currentVolunteer = null;
                if (jobDetails.JobSummary?.VolunteerUserID != null)
                {
                    currentVolunteer = await _userService.GetUserAsync(jobDetails.JobSummary.VolunteerUserID.Value, cancellationToken);
                }

                return new JobDetail()
                {
                    JobSummary = jobDetails.JobSummary,
                    Recipient = jobDetails.Recipient,
                    Requestor = jobDetails.Requestor,
                    JobStatusHistory = jobDetails.History,
                    CurrentVolunteer = currentVolunteer,
                };
            }
            return null;
        }

        public async Task<bool> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken)
        {
            bool success = status switch
            {
                JobStatuses.InProgress => await _requestHelpRepository.UpdateJobStatusToInProgressAsync(jobID, createdByUserId, volunteerUserId.Value),
                JobStatuses.Done => await _requestHelpRepository.UpdateJobStatusToDoneAsync(jobID, createdByUserId),
                JobStatuses.Cancelled => await _requestHelpRepository.UpdateJobStatusToCancelledAsync(jobID, createdByUserId),
                JobStatuses.Open => await _requestHelpRepository.UpdateJobStatusToOpenAsync(jobID, createdByUserId),
                _ => throw new ArgumentException(message: $"Invalid JobStatuses value: {status}", paramName: nameof(status)),
            };

            if (success)
            {
                TriggerCacheRefresh(createdByUserId, cancellationToken);
            }

            return success;
        }

        public async Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpFormVariant requestHelpFormVariant, int referringGroupID, string source)
        {
            return await _requestHelpBuilder.GetSteps(requestHelpFormVariant, referringGroupID, source);
        }
        public async Task<IEnumerable<JobHeader>> GetGroupRequestsAsync(string groupKey, bool waitForData, CancellationToken cancellationToken)
        {
            int groupId = (await _groupService.GetGroupIdByKey(groupKey, cancellationToken));

            return await GetGroupRequestsAsync(groupId, waitForData, cancellationToken);
        }

        public async Task<IEnumerable<JobHeader>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            RefreshBehaviour refreshBehaviour = waitForData ? RefreshBehaviour.WaitForFreshData : RefreshBehaviour.DontWaitForFreshData;
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { ReferringGroupID = groupId });
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}", refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        public IEnumerable<JobHeader> FilterJobs(IEnumerable<JobHeader> jobs, JobFilterRequest jfr)
        {
            return jobs.Where(
                j => (jfr.JobStatuses == null || jfr.JobStatuses.Contains(j.JobStatus))
                    && (jfr.SupportActivities == null || jfr.SupportActivities.Contains(j.SupportActivity))
                    && (jfr.MaxDistanceInMiles == null || j.DistanceInMiles <= jfr.MaxDistanceInMiles)
                    && (jfr.DueInNextXDays == null || j.DueDate.Date <= DateTime.Now.Date.AddDays(jfr.DueInNextXDays.Value))
                    && (jfr.DueAfter == null || j.DueDate.Date >= jfr.DueAfter?.Date)
                    && (jfr.DueBefore == null || j.DueDate.Date <= jfr.DueBefore?.Date)
                    && (jfr.RequestedAfter == null || j.DateRequested.Date >= jfr.RequestedAfter?.Date)
                    && (jfr.RequestedBefore == null) || j.DateRequested.Date <= jfr.RequestedBefore?.Date);
        }

        private void TriggerCacheRefresh(int userId, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                _ = _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { UserID = userId });
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", cancellationToken);


                _ = _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetOpenJobsForUserFromRepo(await _userService.GetUserAsync(userId, cancellationToken));
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-open-jobs", cancellationToken);


                List<UserGroup> userGroups = await _groupService.GetUserGroupRoles(userId, cancellationToken);
                if (userGroups != null)
                {
                    userGroups.Where(g => g.UserRoles.Contains(GroupRoles.TaskAdmin)).ToList().ForEach(g =>
                    {
                        _ = _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                        {
                            return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { ReferringGroupID = g.GroupId });
                        }, $"{CACHE_KEY_PREFIX}-group-{g.GroupId}", cancellationToken);
                    });
                }
            });
        }

        private async Task<IEnumerable<JobHeader>> GetOpenJobsForUserFromRepo(User user)
        {
            if (user.PostalCode == null)
            {
                throw new Exception("Cannot get open jobs for user without postcode");
            }

            var activitySpecificSupportDistancesInMiles = _requestSettings.Value.NationalSupportActivities
                .Where(a => user.SupportActivities.Contains(a)).ToDictionary(a => a, a => (double?)null);
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



