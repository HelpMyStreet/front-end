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
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMemDistCache<IEnumerable<JobHeader>> _memDistCache;
        //private readonly IMemDistCache<IEnumerable<ShiftJob>> _memDistCache_ShiftJobs;
        private readonly IOptions<RequestSettings> _requestSettings;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAddressService _addressService;

        private IEqualityComparer<ShiftJob> _shiftJobDedupe_EqualityComparer;

        private const string CACHE_KEY_PREFIX = "request-service-jobs";

        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IRequestHelpBuilder requestHelpBuilder, IGroupService groupService, IUserService userService, IMemDistCache<IEnumerable<JobHeader>> memDistCache, IOptions<RequestSettings> requestSettings, IGroupMemberService groupMemberService, IAddressService addressService)//, IMemDistCache<IEnumerable<ShiftJob>> memDistCache_ShiftJobs)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
            _requestHelpBuilder = requestHelpBuilder;
            _groupService = groupService;
            _userService = userService;
            _memDistCache = memDistCache;
            _requestSettings = requestSettings;
            _groupMemberService = groupMemberService;
            _addressService = addressService;
            //_memDistCache_ShiftJobs = memDistCache_ShiftJobs;

            _shiftJobDedupe_EqualityComparer = new ShiftJobDedupe_EqualityComparer();
        }

        public async Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Logging Request");
            var recipient = _requestHelpBuilder.MapRecipient(detailStage);

            RequestPersonalDetails requestor = null;
            if (detailStage.ShowRequestorFields)
            {
                requestor = detailStage.Type == RequestorType.Myself ? recipient : _requestHelpBuilder.MapRequestor(detailStage);
            }

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
                    Guid = Guid.NewGuid(),
                    AcceptedTerms = requestStage.AgreeToPrivacyAndTerms,
                    ConsentForContact = requestStage.AgreeToPrivacyAndTerms,
                    OrganisationName = detailStage.Organisation ?? "",
                    RequestorType = detailStage.Type,
                    ReadPrivacyNotice = requestStage.AgreeToPrivacyAndTerms,
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
                            DueDateType = selectedTime.OnDate ? DueDateType.On : DueDateType.Before,
                            DueDays = selectedTime.OnDate ? Convert.ToInt32((selectedTime.Date.Date - DateTime.Now.Date).TotalDays) : selectedTime.Days,
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
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetJobsByFilterAsync(new GetJobsByFilterRequest() { UserID = userId });
            }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);

            if (jobs != null)
            {
                return jobs.OrderOpenJobsForDisplay();
            }
            throw new Exception($"Unable to get jobs for user {userId}");
        }

        public async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            return await GetOpenShiftsForUserFromRepo(user, dateFrom, dateTo, cancellationToken);
        }

        public async Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            //TODO: Add caching?

            //NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            //var jobs = await _memDistCache_ShiftJobs.GetCachedDataAsync(async (cancellationToken) =>
            //{


            return await _requestHelpRepository.GetUserShiftJobsByFilter(new GetUserShiftJobsByFilterRequest()
            {
                VolunteerUserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                JobStatusRequest = new JobStatusRequest() { JobStatuses = new List<JobStatuses>() { JobStatuses.Accepted, JobStatuses.InProgress, JobStatuses.Done } }
            });
            
            
            //}, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);

            //return jobs?.OrderOpenJobsForDisplay();
        }

        public async Task<GetRequestDetailsResponse> GetRequestDetailAsync(int requestId, int userId, CancellationToken cancellationToken)
        {
            return await _requestHelpRepository.GetRequestDetailsAsync(requestId, userId);
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            return (await GetJobAndRequestSummaryAsync(jobId, cancellationToken)).JobSummary;
        }

        public async Task<JobDetail> GetJobAndRequestSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            var getJobSummaryResponse = await _requestHelpRepository.GetJobSummaryAsync(jobId);

            return new JobDetail()
            {
                RequestSummary = getJobSummaryResponse.RequestSummary,
                JobSummary = getJobSummaryResponse.JobSummary,
            };
        }

        public async Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, bool adminView, CancellationToken cancellationToken)
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
                    RequestSummary = jobDetails.RequestSummary,
                    JobSummary = jobDetails.JobSummary,
                    Recipient = jobDetails.Recipient,
                    Requestor = jobDetails.Requestor,
                    JobStatusHistory = await EnrichStatusHistory(jobDetails.History, adminView, cancellationToken),
                    CurrentVolunteer = currentVolunteer,
                };
            }
            throw new Exception($"Failed to get job details for job {jobId} (user {userId})");
        }

        public async Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken)
        {
            UpdateJobStatusOutcome? outcome = status switch
            {
                JobStatuses.Accepted => await _requestHelpRepository.UpdateJobStatusToAcceptedAsync(jobID, createdByUserId, volunteerUserId.Value),
                JobStatuses.InProgress => await UpdateJobStatusToInProgressAsync(jobID, createdByUserId, volunteerUserId.Value, cancellationToken),
                JobStatuses.Done => await _requestHelpRepository.UpdateJobStatusToDoneAsync(jobID, createdByUserId),
                JobStatuses.Cancelled => await _requestHelpRepository.UpdateJobStatusToCancelledAsync(jobID, createdByUserId),
                JobStatuses.Open => await _requestHelpRepository.UpdateJobStatusToOpenAsync(jobID, createdByUserId),
                JobStatuses.New => await _requestHelpRepository.UpdateJobStatusToNewAsync(jobID, createdByUserId),
                _ => throw new ArgumentException(message: $"Invalid JobStatuses value: {status}", paramName: nameof(status)),
            };

            if (outcome == UpdateJobStatusOutcome.Success || outcome == UpdateJobStatusOutcome.AlreadyInThisStatus)
            {
                TriggerCacheRefresh(createdByUserId, cancellationToken);
            }

            return outcome;
        }

        private async Task<UpdateJobStatusOutcome?> UpdateJobStatusToInProgressAsync(int jobID, int createdByUserId, int volunteerUserId, CancellationToken cancellationToken)
        {
            var job = await GetJobSummaryAsync(jobID, cancellationToken);

            return job.RequestType switch
            {
                RequestType.Shift => await _requestHelpRepository.PutUpdateShiftStatusToAccepted(job.RequestID, job.SupportActivity, createdByUserId, volunteerUserId),
                RequestType.Task => await _requestHelpRepository.UpdateJobStatusToInProgressAsync(jobID, createdByUserId, volunteerUserId),
                _ => throw new ArgumentException(message: $"Invalid RequestType value: {job.RequestType}", paramName: nameof(job.RequestType)),
            };
        }


        public async Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpJourney requestHelpJourney, int referringGroupID, string source)
        {
            return await _requestHelpBuilder.GetSteps(requestHelpJourney, referringGroupID, source);
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

        public async Task<JobLocation> LocateJob(int jobId, int userId, CancellationToken cancellationToken)
        {
            var job = (await _requestHelpRepository.GetJobSummaryAsync(jobId)).JobSummary;

            if (job.VolunteerUserID == userId && job.JobStatus != JobStatuses.Open)
            {
                return new JobLocation
                {
                    JobSet = job.JobStatus switch
                    {
                        JobStatuses.InProgress => JobSet.UserAcceptedRequests,
                        JobStatuses.Done => JobSet.UserCompletedRequests,
                        JobStatuses.Cancelled => JobSet.UserCompletedRequests,
                        _ => throw new ArgumentException($"Unexpected JobStatuses value: {job.JobStatus}", nameof(job.JobStatus)),
                    }
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, job.ReferringGroupID, GroupRoles.TaskAdmin, cancellationToken))
            {
                return new JobLocation
                {
                    JobSet = JobSet.GroupRequests,
                    GroupKey = (await _groupService.GetGroupById(job.ReferringGroupID, cancellationToken)).GroupKey,
                };
            }
            else if (job.JobStatus == JobStatuses.Open)
            {
                return new JobLocation { JobSet = JobSet.UserOpenRequests_MatchingCriteria };
            }

            return null;
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


                List<UserGroup> userGroups = await _groupMemberService.GetUserGroupRoles(userId, cancellationToken);
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
                DistanceInMiles = Math.Max(_requestSettings.Value.OpenRequestsRadius, user.SupportRadiusMiles ?? 0),
                ActivitySpecificSupportDistancesInMiles = activitySpecificSupportDistancesInMiles,
                JobStatuses = new JobStatusRequest()
                {
                    JobStatuses = new List<JobStatuses>() { JobStatuses.Open }
                },
                Groups = new GroupRequest() { Groups = await _groupMemberService.GetUserGroups(user.ID) }
            };

            return await _requestHelpRepository.GetJobsByFilterAsync(jobsByFilterRequest);
        }

        private async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserFromRepo(User user, DateTime? dateFrom, DateTime? dateTo, CancellationToken canellationToken)
        {
            var locations = (await _addressService.GetLocationsForUser(user, canellationToken)).Select(l => l.Location).ToList();

            var getOpenShiftJobsByFilterRequest = new GetOpenShiftJobsByFilterRequest
            {
                ExcludeSiblingsOfJobsAllocatedToUserID = user.ID,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Groups = new GroupRequest { Groups = new List<int>() },
                Locations = new LocationsRequest { Locations = locations },
                SupportActivities = new SupportActivityRequest { SupportActivities = new List<SupportActivities>() }
            };
            var allShifts = await _requestHelpRepository.GetOpenShiftJobsByFilter(getOpenShiftJobsByFilterRequest);
            var dedupedShifts = allShifts.Distinct(_shiftJobDedupe_EqualityComparer);

            var userShifts = await GetShiftsForUserAsync(user.ID, null, null, true, canellationToken);
            var notMyShifts = dedupedShifts.Where(s => !userShifts.Contains(s, _shiftJobDedupe_EqualityComparer));

            return notMyShifts;
        }

        public async Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(int groupId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            //TODO: Add caching?

            var getShiftRequestsByFilterRequest = new GetShiftRequestsByFilterRequest
            {
                ReferringGroupID = groupId,
                DateFrom = dateFrom,
                DateTo = dateTo,
            };

            return await _requestHelpRepository.GetShiftRequestsByFilter(getShiftRequestsByFilterRequest);
        }

        public async Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(string groupKey, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            int groupId = (await _groupService.GetGroupIdByKey(groupKey, cancellationToken));

            return await GetGroupShiftRequestsAsync(groupId, dateFrom, dateTo, waitForData, cancellationToken);
        }

        private async Task<List<EnrichedStatusHistory>> EnrichStatusHistory(List<StatusHistory> history, bool ShowNames, CancellationToken cancellationToken)
        {
            List<EnrichedStatusHistory> eHist = history.Select(h => new EnrichedStatusHistory(h)).ToList();

            int latestVolunteerId = -1;

            for (int i = 0; i < eHist.Count; i++)
            {
                switch (eHist[i].StatusHistory.JobStatus)
                {
                    case JobStatuses.New:
                        eHist[i].JobStatusDescription = "Created";
                        break;
                    case JobStatuses.Open:
                        if (latestVolunteerId > 0)
                        {
                            eHist[i].JobStatusDescription = "Released";
                            eHist[i].StatusHistory.VolunteerUserID = latestVolunteerId;
                        }
                        else if (i == 0)
                        {
                            eHist[i].JobStatusDescription = "Created";
                        }
                        else
                        {
                            eHist[i].JobStatusDescription = "Approved";
                        }
                        break;
                    default:
                        eHist[i].JobStatusDescription = eHist[i].StatusHistory.JobStatus.FriendlyName();
                        break;
                }

                if ((eHist[i].StatusHistory.VolunteerUserID ?? -1) > 0)
                {
                    latestVolunteerId = eHist[i].StatusHistory.VolunteerUserID.Value;

                    if (ShowNames)
                    {
                        eHist[i].VolunteerUser = await _userService.GetUserAsync(eHist[i].StatusHistory.VolunteerUserID.Value, cancellationToken);
                    }
                }
            }

            return eHist;
        }
    }
}



