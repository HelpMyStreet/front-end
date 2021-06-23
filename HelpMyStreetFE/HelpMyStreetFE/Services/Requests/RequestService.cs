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
using HelpMyStreet.Utils.EqualityComparers;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;
        private readonly IRequestHelpBuilder _requestHelpBuilder;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMemDistCache<IEnumerable<JobSummary>> _memDistCache;
        private readonly IMemDistCache<IEnumerable<ShiftJob>> _memDistCache_ShiftJobs;
        private readonly IMemDistCache<IEnumerable<RequestSummary>> _memDistCache_RequestSummaries;
        private readonly IOptions<RequestSettings> _requestSettings;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAddressService _addressService;

        private IEqualityComparer<ShiftJob> _shiftJobDedupe_EqualityComparer;
        private IEqualityComparer<JobSummary> _jobSummaryJobDedupe_EqualityComparer;
        private IEqualityComparer<JobSummary> _jobSummaryJobDedupeWithDate_EqualityComparer;

        private const string CACHE_KEY_PREFIX = "request-service-jobs";

        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger, IRequestHelpBuilder requestHelpBuilder, IGroupService groupService, IUserService userService, IMemDistCache<IEnumerable<JobSummary>> memDistCache, IOptions<RequestSettings> requestSettings, IGroupMemberService groupMemberService, IAddressService addressService, IMemDistCache<IEnumerable<ShiftJob>> memDistCache_ShiftJobs, IMemDistCache<IEnumerable<RequestSummary>> memDistCache_RequestSummaries)
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
            _memDistCache_ShiftJobs = memDistCache_ShiftJobs;
            _memDistCache_RequestSummaries = memDistCache_RequestSummaries;

            _shiftJobDedupe_EqualityComparer = new JobBasicDedupe_EqualityComparer();
            _jobSummaryJobDedupe_EqualityComparer = new JobBasicDedupe_EqualityComparer();
            _jobSummaryJobDedupeWithDate_EqualityComparer = new JobBasicDedupeWithDate_EqualityComparer();
        }

        public async Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Logging Request");

            var selectedTask = requestStage.Tasks.Where(x => x.IsSelected).First();
            var selectedTime = requestStage.Timeframes.Where(x => x.IsSelected).FirstOrDefault();
            var selectedFrequency = requestStage.Frequencies.Where(x => x.IsSelected).FirstOrDefault();

            int numberOfOccurrences = 1;
            if (requestStage.Occurrences.HasValue)
            {
                numberOfOccurrences = requestStage.Occurrences.Value;
            }

            bool heathCritical = false;
            var healthCriticalQuestion = requestStage.Questions.Questions.Where(a => a.ID == (int)Questions.IsHealthCritical).FirstOrDefault();
            if (healthCriticalQuestion != null && healthCriticalQuestion.Model == "true") { heathCritical = true; }

            RequestPersonalDetails recipient = null;
            RequestPersonalDetails requestor = null;
            IEnumerable<RequestHelpQuestion> questions = requestStage.Questions.Questions;

            if (detailStage != null)
            {
                recipient = _requestHelpBuilder.MapRecipient(detailStage);
                if (detailStage.ShowRequestorFields)
                {
                    requestor = detailStage.Type == RequestorType.Myself ? recipient : _requestHelpBuilder.MapRequestor(detailStage);
                }
                questions = questions.Union(detailStage.Questions.Questions);
            }

            var request = new PostNewRequestForHelpRequest
            {
                HelpRequest = new HelpRequest
                {
                    Guid = requestStage.RequestGuid,
                    AcceptedTerms = requestStage.AgreeToPrivacyAndTerms,
                    ConsentForContact = requestStage.AgreeToPrivacyAndTerms,
                    OrganisationName = detailStage?.Organisation ?? "",
                    RequestorType = detailStage?.Type ?? RequestorType.Organisation,
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
                            DueDateType = selectedTime.DueDateType,
                            StartDate = selectedTime.StartTime.ToUTCFromUKTime(),
                            EndDate = selectedTime.EndTime.HasValue ? selectedTime.EndTime.Value.ToUTCFromUKTime() : (DateTime?)null,
                            NotBeforeDate = selectedTime.NotBeforeTime.ToUTCFromUKTime(),
                            RepeatFrequency = selectedFrequency.Frequency,
                            NumberOfRepeats = numberOfOccurrences,
                            HealthCritical = heathCritical,
                            SupportActivity = selectedTask.SupportActivity,
                            Questions = questions.Where(x => x.InputType != QuestionType.LabelOnly).Select(x => new Question {
                                Id = x.ID,
                                Answer = GetAnswerToQuestion(x),
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

        private string GetAnswerToQuestion(RequestHelpQuestion q)
        {
             return q.InputType == QuestionType.Radio ? q.AdditionalData.Where(a => a.Key == q.Model).FirstOrDefault()?.Value ?? "" : q.Model;
        }

        public async Task<IEnumerable<JobSummary>> GetOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken)
        {
            RefreshBehaviour refreshBehaviour = waitForData ? RefreshBehaviour.WaitForFreshData : RefreshBehaviour.DontWaitForFreshData;
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetOpenJobsForUserFromRepo(user, cancellationToken);
            }, $"{CACHE_KEY_PREFIX}-user-{user.ID}-open-jobs", refreshBehaviour, cancellationToken, notInCacheBehaviour);

            return jobs;
        }

        public OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<JobSummary> jobs)
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

        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            var jobs = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                var request = new GetAllJobsByFilterRequest { AllocatedToUserId = userId };
                var response = await _requestHelpRepository.GetJobsByFilterAsync(request);
                return response.JobSummaries;
            }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);

            if (jobs != null)
            {
                return jobs.OrderOpenJobsForDisplay();
            }
            throw new Exception($"Unable to get jobs for user {userId}");
        }

        public async Task<IEnumerable<RequestSummary>> GetRequestsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            var jobs = await _memDistCache_RequestSummaries.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetUserRequestsFromRepo(userId, cancellationToken);
            }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs-requests", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);

            if (jobs != null)
            {
                return jobs;
            }
            throw new Exception($"Unable to get jobs for user {userId}");
        }

        public async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            return await _memDistCache_ShiftJobs.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetOpenShiftsForUserFromRepo(user, dateFrom, dateTo, cancellationToken);
            }, $"{CACHE_KEY_PREFIX}-user-{user.ID}-open-shifts-from-{dateFrom}-to-{dateTo}", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);
        }

        public async Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            return await _memDistCache_ShiftJobs.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetUserShiftJobsByFilter(new GetUserShiftJobsByFilterRequest()
                {
                    VolunteerUserId = userId,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    JobStatusRequest = new JobStatusRequest() { JobStatuses = new List<JobStatuses>() { JobStatuses.Accepted, JobStatuses.InProgress, JobStatuses.Done } }
                });
            }, $"{CACHE_KEY_PREFIX}-user-{userId}-user-shifts-from-{dateFrom}-to-{dateTo}", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);
        }

        public async Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken)
        {
            return (await _requestHelpRepository.GetRequestSummaryAsync(requestId)).RequestSummary;
        }

        public async Task<GetRequestDetailsResponse> GetRequestDetailAsync(int requestId, int userId, CancellationToken cancellationToken)
        {
            return await _requestHelpRepository.GetRequestDetailsAsync(requestId, userId);
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            var getJobSummaryResponse = await _requestHelpRepository.GetJobSummaryAsync(jobId);

            return getJobSummaryResponse.JobSummary;
        }

        public async Task<JobDetail> GetJobAndRequestSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            var getJobSummaryResponse = await _requestHelpRepository.GetJobSummaryAsync(jobId);
            var getRequestSummaryResponse = await _requestHelpRepository.GetRequestSummaryAsync(getJobSummaryResponse.RequestID);

            //JobDetail jobDetail = (JobDetail)getJobSummaryResponse.JobSummary;
            //jobDetail.RequestSummary = getRequestSummaryResponse.RequestSummary;

            //return jobDetail;

            return new JobDetail(getJobSummaryResponse.JobSummary)
            {
                RequestSummary = getRequestSummaryResponse.RequestSummary,
                //JobSummary = getJobSummaryResponse.JobSummary,
            };
        }

        public async Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, bool adminView, CancellationToken cancellationToken)
        {
            var jobDetails = await _requestHelpRepository.GetJobDetailsAsync(jobId, userId);
            var getRequestSummaryResponse = await _requestHelpRepository.GetRequestSummaryAsync(jobDetails.RequestSummary.RequestID);

            if (jobDetails != null)
            {
                User currentVolunteer = null;
                if (jobDetails.JobSummary?.VolunteerUserID != null)
                {
                    currentVolunteer = await _userService.GetUserAsync(jobDetails.JobSummary.VolunteerUserID.Value, cancellationToken);
                }

                //JobDetail jobDetail = (JobDetail)jobDetails.JobSummary;
                //jobDetail.RequestSummary = getRequestSummaryResponse.RequestSummary;
                //jobDetail.Recipient = jobDetails.Recipient;
                //jobDetail.Requestor = jobDetails.Requestor;
                //jobDetail.JobStatusHistory = await EnrichStatusHistory(jobDetails.History, adminView, cancellationToken);
                //jobDetail.CurrentVolunteer = currentVolunteer;

                //return jobDetail;

                return new JobDetail(jobDetails.JobSummary)
                {
                    RequestSummary = getRequestSummaryResponse.RequestSummary,
                   // JobSummary = jobDetails.JobSummary,
                    Recipient = jobDetails.Recipient,
                    Requestor = jobDetails.Requestor,
                    JobStatusHistory = await EnrichStatusHistory(jobDetails.History, adminView, cancellationToken),
                    CurrentVolunteer = currentVolunteer,
                };
            }
            throw new Exception($"Failed to get job details for job {jobId} (user {userId})");
        }

        public async Task<UpdateJobStatusOutcome?> UpdateRequestStatusAsync(int requestId, JobStatuses status, int createdByUserId, CancellationToken cancellationToken)
        {
            UpdateJobStatusOutcome? outcome = status switch
            {
                JobStatuses.Done => await _requestHelpRepository.PutUpdateRequestStatusToDone(requestId, createdByUserId),
                JobStatuses.Cancelled => await _requestHelpRepository.PutUpdateRequestStatusToCancelled(requestId, createdByUserId),
                _ => throw new ArgumentException(message: $"Invalid JobStatuses value for Request: {status}", paramName: nameof(status)),
            };

            if (outcome == UpdateJobStatusOutcome.Success || outcome == UpdateJobStatusOutcome.AlreadyInThisStatus)
            {
                TriggerCacheRefresh(createdByUserId, cancellationToken);
            }

            return outcome;
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

        public async Task<IEnumerable<RequestSummary>> GetGroupRequestsAsync(string groupKey, bool waitForData, CancellationToken cancellationToken)
        {
            int groupId = (await _groupService.GetGroupIdByKey(groupKey, cancellationToken));

            return await GetGroupRequestsAsync(groupId, waitForData, cancellationToken);
        }

        public async Task<IEnumerable<RequestSummary>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            RefreshBehaviour refreshBehaviour = waitForData ? RefreshBehaviour.WaitForFreshData : RefreshBehaviour.DontWaitForFreshData;
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            return await _memDistCache_RequestSummaries.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestsByFilter(new GetRequestsByFilterRequest() { ReferringGroupID = groupId, IncludeChildGroups = true, RequestType = new RequestTypeRequest { RequestTypes = new List<RequestType> { RequestType.Task } } });
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}-requests", refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        public async Task<IEnumerable<RequestSummary>> GetAllGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            RefreshBehaviour refreshBehaviour = waitForData ? RefreshBehaviour.WaitForFreshData : RefreshBehaviour.DontWaitForFreshData;
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            return await _memDistCache_RequestSummaries.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestsByFilter(new GetRequestsByFilterRequest() { ReferringGroupID = groupId, IncludeChildGroups = true });
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}-all-requests", refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        public async Task<JobLocation> LocateJob(int jobId, int userId, CancellationToken cancellationToken)
        {
            var job = (await _requestHelpRepository.GetJobSummaryAsync(jobId)).JobSummary;

            if (job.VolunteerUserID == userId && job.JobStatus != JobStatuses.Open)
            {
                return new JobLocation
                {
                    JobSet = job.RequestType switch
                    {
                        RequestType.Task => JobSet.UserMyRequests,
                        RequestType.Shift => JobSet.UserMyShifts,
                        _ => throw new ArgumentException($"Unexpected RequestType: {job.RequestType}", nameof(job.RequestType)),
                    }
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, job.ReferringGroupID, GroupRoles.TaskAdmin, false, cancellationToken))
            {
                return new JobLocation
                {
                    JobSet = (job.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = (await _groupService.GetGroupById(job.ReferringGroupID, cancellationToken)).GroupKey,
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, job.ReferringGroupID, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                var group = await _groupService.GetGroupById(job.ReferringGroupID, cancellationToken);
                var parentGroup = await _groupService.GetGroupById(group.ParentGroupId.Value, cancellationToken);
                return new JobLocation
                {
                    JobSet = (job.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = parentGroup.GroupKey,
                };
            }
            else if (job.JobStatus == JobStatuses.Open)
            {
                return new JobLocation { JobSet = (job.RequestType.Equals(RequestType.Task) ? JobSet.UserOpenRequests_MatchingCriteria : JobSet.UserOpenShifts) };
            }

            return null;
        }

        public async Task<JobLocation> LocateRequest(int requestId, int userId, CancellationToken cancellationToken)
        {
            var request =  await GetRequestSummaryAsync(requestId, cancellationToken);

            if (request.JobBasics.Count(j => j.VolunteerUserID == userId && j.JobStatus != JobStatuses.Open) > 0)
            {
                return new JobLocation
                {
                    JobSet = request.RequestType switch
                    {
                        RequestType.Task => JobSet.UserMyRequests,
                        RequestType.Shift => JobSet.UserMyShifts,
                        _ => throw new ArgumentException($"Unexpected RequestType: {request.RequestType}", nameof(request.RequestType)),
                    }
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, request.ReferringGroupID, GroupRoles.TaskAdmin, false, cancellationToken))
            {
                return new JobLocation
                {
                    JobSet = (request.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = (await _groupService.GetGroupById(request.ReferringGroupID, cancellationToken)).GroupKey,
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, request.ReferringGroupID, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                var group = await _groupService.GetGroupById(request.ReferringGroupID, cancellationToken);
                var parentGroup = await _groupService.GetGroupById(group.ParentGroupId.Value, cancellationToken);
                return new JobLocation
                {
                    JobSet = (request.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = parentGroup.GroupKey,
                };
            }
            else if (request.JobBasics.JobStatusDictionary().ContainsKey(JobStatuses.Open))
            {
                return new JobLocation { JobSet = (request.RequestType.Equals(RequestType.Task) ? JobSet.UserOpenRequests_MatchingCriteria : JobSet.UserOpenShifts) };
            }

            return null;
        }

        private void TriggerCacheRefresh(int userId, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                _ = _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    var request = new GetAllJobsByFilterRequest { AllocatedToUserId = userId };
                    var response = await _requestHelpRepository.GetJobsByFilterAsync(request);
                    return response.JobSummaries;
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs", cancellationToken);

                _ = _memDistCache_RequestSummaries.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRequestsFromRepo(userId, cancellationToken);
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-accepted-jobs-requests", cancellationToken);


                _ = _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetOpenJobsForUserFromRepo(await _userService.GetUserAsync(userId, cancellationToken), cancellationToken);
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-open-jobs", cancellationToken);


                DateTime? dateFrom = null;
                DateTime? dateTo = null;
                _ = _memDistCache_ShiftJobs.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetOpenShiftsForUserFromRepo(await _userService.GetUserAsync(userId, cancellationToken), dateFrom, dateTo, cancellationToken);
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-open-shifts-from-{dateFrom}-to-{dateTo}", cancellationToken);


                _ = await _memDistCache_ShiftJobs.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetUserShiftJobsByFilter(new GetUserShiftJobsByFilterRequest()
                    {
                        VolunteerUserId = userId,
                        DateFrom = dateFrom,
                        DateTo = dateTo,
                        JobStatusRequest = new JobStatusRequest() { JobStatuses = new List<JobStatuses>() { JobStatuses.Accepted, JobStatuses.InProgress, JobStatuses.Done } }
                    });
                }, $"{CACHE_KEY_PREFIX}-user-{userId}-user-shifts-from-{dateFrom}-to-{dateTo}", cancellationToken);


                List<UserGroup> userGroups = await _groupMemberService.GetUserGroupRoles(userId, cancellationToken);
                if (userGroups != null)
                {
                    userGroups.Where(g => g.UserRoles.Contains(GroupRoles.TaskAdmin)).ToList().ForEach(g =>
                    {
                        if (g.TasksEnabled)
                        {
                            _ = _memDistCache_RequestSummaries.RefreshDataAsync(async (cancellationToken) =>
                            {
                                return await _requestHelpRepository.GetRequestsByFilter(new GetRequestsByFilterRequest() { ReferringGroupID = g.GroupId, IncludeChildGroups = true, RequestType = new RequestTypeRequest { RequestTypes = new List<RequestType> { RequestType.Task } } });
                            }, $"{CACHE_KEY_PREFIX}-group-{g.GroupId}-requests", cancellationToken);
                        }

                        if (g.ShiftsEnabled)
                        {
                            _ = _memDistCache_RequestSummaries.RefreshDataAsync(async (cancellationToken) =>
                            {
                                var getShiftRequestsByFilterRequest = new GetShiftRequestsByFilterRequest
                                {
                                    ReferringGroupID = g.GroupId,
                                    IncludeChildGroups = true,
                                    DateFrom = dateFrom,
                                    DateTo = dateTo,
                                };

                                return await _requestHelpRepository.GetShiftRequestsByFilter(getShiftRequestsByFilterRequest);
                            }, $"{CACHE_KEY_PREFIX}-group-{g.GroupId}-shifts-from-{dateFrom}-to-{dateTo}", cancellationToken);
                        }

                        _ = _memDistCache_RequestSummaries.RefreshDataAsync(async (cancellationToken) =>
                        {
                            return await _requestHelpRepository.GetRequestsByFilter(new GetRequestsByFilterRequest() { ReferringGroupID = g.GroupId, IncludeChildGroups = true });
                        }, $"{CACHE_KEY_PREFIX}-group-{g.GroupId}-all-requests", cancellationToken);
                    });
                }
            });
        }

        private async Task<IEnumerable<RequestSummary>> GetUserRequestsFromRepo(int userId, CancellationToken cancellationToken)
        {
            var userJobs = await GetJobsForUserAsync(userId, true, cancellationToken);
            var requestIDs = userJobs.Select(j => j.RequestID).Distinct().ToList();
            return await _requestHelpRepository.GetRequestSummariesAsync(requestIDs);
        }

        private async Task<IEnumerable<JobSummary>> GetOpenJobsForUserFromRepo(User user, CancellationToken cancellationToken)
        {
            if (user.PostalCode == null)
            {
                throw new Exception("Cannot get open jobs for user without postcode");
            }

            var activitySpecificSupportDistancesInMiles = _requestSettings.Value.NationalSupportActivities
                .Where(a => user.SupportActivities.Contains(a)).ToDictionary(a => a, a => (double?)null);
            var jobsByFilterRequest = new GetAllJobsByFilterRequest()
            {
                Postcode = user.PostalCode,
                DistanceInMiles = Math.Max(_requestSettings.Value.OpenRequestsRadius, user.SupportRadiusMiles ?? 0),
                ActivitySpecificSupportDistancesInMiles = activitySpecificSupportDistancesInMiles,
                JobStatuses = new JobStatusRequest()
                {
                    JobStatuses = new List<JobStatuses>() { JobStatuses.Open }
                },
                Groups = new GroupRequest() { Groups = await _groupMemberService.GetUserGroups(user.ID) },
                RequestType = new RequestTypeRequest { RequestTypes = new List<RequestType> { RequestType.Task } },
            };

            var allJobs = (await _requestHelpRepository.GetJobsByFilterAsync(jobsByFilterRequest)).JobSummaries;
            var dedupedJobs = allJobs.Distinct(_jobSummaryJobDedupe_EqualityComparer);
            var userJobs = await GetJobsForUserAsync(user.ID, true, cancellationToken);
            var notMyJobs = dedupedJobs.Where(s => !userJobs.Contains(s, _jobSummaryJobDedupeWithDate_EqualityComparer));

            return notMyJobs;
        }

        private async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserFromRepo(User user, DateTime? dateFrom, DateTime? dateTo, CancellationToken canellationToken)
        {
            var locations = (await _addressService.GetLocationDetailsForUser(user, canellationToken)).Select(l => l.Location).ToList();

            var getOpenShiftJobsByFilterRequest = new GetOpenShiftJobsByFilterRequest
            {
                ExcludeSiblingsOfJobsAllocatedToUserID = user.ID,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Groups = new GroupRequest { Groups = await _groupMemberService.GetUserGroups(user.ID) },
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
            
            NotInCacheBehaviour notInCacheBehaviour = waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;

            return await _memDistCache_RequestSummaries.GetCachedDataAsync(async (cancellationToken) =>
            {
                var getShiftRequestsByFilterRequest = new GetShiftRequestsByFilterRequest
                {
                    ReferringGroupID = groupId,
                    IncludeChildGroups = true,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                };

                return await _requestHelpRepository.GetShiftRequestsByFilter(getShiftRequestsByFilterRequest);
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}-shifts-from-{dateFrom}-to-{dateTo}", RefreshBehaviour.DontWaitForFreshData, cancellationToken, notInCacheBehaviour);
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



