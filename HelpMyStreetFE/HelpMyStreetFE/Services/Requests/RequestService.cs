using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreet.Utils.Extensions;
using System.Threading;
using HelpMyStreetFE.Services.Groups;
using HelpMyStreetFE.Services.Users;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreet.Utils.EqualityComparers;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestService : IRequestService
    {
        private readonly IRequestListCachingService _requestListCachingService;
        private readonly IRequestCachingService _requestCachingService;
        private readonly IJobCachingService _jobCachingService;
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly IUserService _userService;

        private readonly IEqualityComparer<ShiftJob> _shiftJobDedupe_EqualityComparer;
        private readonly IEqualityComparer<JobSummary> _jobSummaryJobDedupe_EqualityComparer;
        private readonly IEqualityComparer<JobSummary> _jobSummaryJobDedupeWithDate_EqualityComparer;

        public RequestService(
            IRequestHelpRepository requestHelpRepository,
            IUserService userService,
            IRequestCachingService requestCachingService,
            IJobCachingService jobCachingService, 
            IRequestListCachingService requestListCachingService)
        {
            _requestCachingService = requestCachingService;
            _jobCachingService = jobCachingService;
            _requestHelpRepository = requestHelpRepository;
            _userService = userService;
            _shiftJobDedupe_EqualityComparer = new JobBasicDedupe_EqualityComparer();
            _jobSummaryJobDedupe_EqualityComparer = new JobBasicDedupe_EqualityComparer();
            _jobSummaryJobDedupeWithDate_EqualityComparer = new JobBasicDedupeWithDate_EqualityComparer();
            _requestListCachingService = requestListCachingService;
        }


        public OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<IEnumerable<JobSummary>> jobs)
        {
            if (jobs == null)
            {
                return null;
            }

            var (criteriaJobs, otherJobs) = jobs.Split(x => user.SupportActivities.Contains(x.First().SupportActivity) && x.First().DistanceInMiles <= user.SupportRadiusMiles);

            return new OpenJobsViewModel
            {
                CriteriaJobs = criteriaJobs,
                OtherJobs = otherJobs,
            };
        }

        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);
            var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

            if (userRequests != null)
            {
                var userJobs = userRequests.SelectMany(r => r.JobSummaries).Where(j => j.VolunteerUserID.Equals(userId));
                return userJobs;//.OrderOpenJobsForDisplay();
            }
            throw new Exception($"Unable to get jobs for user {userId}");
        }

        // My Requests tab
        public async Task<IEnumerable<RequestSummary>> GetRequestsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);
            var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

            if (userRequests != null)
            {
                // Check all request still include a job allocated to the user, and filter out shifts
                var userTaskRequests = userRequests.Where(r => r.RequestType.Equals(RequestType.Task) && r.JobBasics.Exists(j => j.VolunteerUserID.Equals(userId)));
                return userTaskRequests;
            }
            throw new Exception($"Unable to get requests for user {userId}");
        }

        public async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            return await GetOpenShiftsForUserFromRepo(user, dateFrom, dateTo, waitForData, cancellationToken);
        }

        public async Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);
            var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

            if (userRequests != null)
            {
                var userJobs = userRequests.SelectMany(r => r.ShiftJobs).Where(j => j.VolunteerUserID.Equals(userId));
                return userJobs;
            }
            throw new Exception($"Unable to get shifts for user {userId}");
        }

        public async Task<GetRequestDetailsResponse> GetRequestDetailAsync(int requestId, int userId, CancellationToken cancellationToken)
        {
            return await _requestHelpRepository.GetRequestDetailsAsync(requestId, userId);
        }

        public async Task<JobDetail> GetJobAndRequestSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            var jobBasic = await _jobCachingService.GetJobBasicAsync(jobId, cancellationToken);
            var requestSummary = await _requestCachingService.GetRequestSummaryAsync(jobBasic.RequestID, cancellationToken);

            if (jobBasic.RequestType.Equals(RequestType.Task))
            {
                var jobSummary = await _jobCachingService.GetJobSummaryAsync(jobId, cancellationToken);

                return new JobDetail(jobSummary)
                {
                    RequestSummary = requestSummary
                };
            }
            else
            {
                return new JobDetail(jobBasic)
                {
                    RequestSummary = requestSummary
                };
            }
        }

        public async Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, bool adminView, CancellationToken cancellationToken)
        {
            var jobDetails = await _requestHelpRepository.GetJobDetailsAsync(jobId, userId);
            // RequestSummary from GetJobDetailsAsync contains only one job
            var requestSummary = await _requestCachingService.GetRequestSummaryAsync(jobDetails.JobSummary.RequestID, cancellationToken);

            if (jobDetails != null)
            {
                User currentVolunteer = null;
                if (jobDetails.JobSummary?.VolunteerUserID != null)
                {
                    currentVolunteer = await _userService.GetUserAsync(jobDetails.JobSummary.VolunteerUserID.Value, cancellationToken);
                }

                return new JobDetail(jobDetails.JobSummary)
                {
                    RequestSummary = requestSummary,
                    Recipient = jobDetails.Recipient,
                    Requestor = jobDetails.Requestor,
                    JobStatusHistory = await EnrichStatusHistory(jobDetails.History, adminView, cancellationToken),
                    CurrentVolunteer = currentVolunteer,
                };
            }
            throw new Exception($"Failed to get job details for job {jobId} (user {userId})");
        }

        public async Task<IEnumerable<RequestSummary>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            var groupRequestIDs = await _requestListCachingService.GetGroupRequestsAsync(groupId, waitForData, cancellationToken);
            var groupRequests = await GetAllGroupRequestsAsync(groupId, waitForData, cancellationToken);
            var groupTaskRequests = groupRequests.Where(r => r.RequestType.Equals(RequestType.Task));

            return groupTaskRequests;
        }

        public async Task<IEnumerable<RequestSummary>> GetAllGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            var groupRequestIDs = await _requestListCachingService.GetGroupRequestsAsync(groupId, waitForData, cancellationToken);
            var groupRequests = await _requestCachingService.GetRequestSummariesAsync(groupRequestIDs, waitForData, cancellationToken);

            return groupRequests;
        }

        public async Task<IEnumerable<JobSummary>> FilterAndDedupeOpenJobsForUser(IEnumerable<JobSummary> allJobs, User user, CancellationToken cancellationToken)
        {
            var dedupedJobs = allJobs.Distinct(_jobSummaryJobDedupeWithDate_EqualityComparer);
            var userJobs = await GetJobsForUserAsync(user.ID, true, cancellationToken);
            var notMyJobs = dedupedJobs.Where(s => !userJobs.Contains(s, _jobSummaryJobDedupeWithDate_EqualityComparer));

            return notMyJobs;
        }

        public async Task<IEnumerable<IEnumerable<JobSummary>>> GetGroupedOpenJobsForUserFromRepo(User user, bool waitForData, CancellationToken cancellationToken)
        {
            var openJobIDs = await _requestListCachingService.GetUserOpenJobsAsync(user, waitForData, cancellationToken);
            var openJobs = await _jobCachingService.GetJobSummariesAsync(openJobIDs, cancellationToken);

            // Check jobs are all still open
            var stillOpenJobs = openJobs.Where(j => j.JobStatus.Equals(JobStatuses.Open));

            // Exclude duplicates of jobs the user has accepted
            var userJobs = await GetJobsForUserAsync(user.ID, true, cancellationToken);
            var notMyJobs = stillOpenJobs.Where(j => !userJobs.Contains(j, _jobSummaryJobDedupeWithDate_EqualityComparer));

            var groupedJobs = notMyJobs.GroupBy(j => j, _jobSummaryJobDedupe_EqualityComparer).Select(g => g.AsEnumerable());

            return groupedJobs;
        }

        private async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserFromRepo(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            var openJobIDs = await _requestListCachingService.GetUserOpenJobsAsync(user, waitForData, cancellationToken);
            var openJobs = await _jobCachingService.GetShiftJobsAsync(openJobIDs, cancellationToken);

            // Check jobs are all still open
            var stillOpenJobs = openJobs.Where(j => j.JobStatus.Equals(JobStatuses.Open));

            // Exclude duplicates of jobs the user has accepted
            var userJobs = await GetShiftsForUserAsync(user.ID, null, null, true, cancellationToken);
            var notMyJobs = stillOpenJobs.Where(j => !userJobs.Contains(j, _shiftJobDedupe_EqualityComparer));

            return notMyJobs;
        }

        public async Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(int groupId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            var groupRequests = await GetAllGroupRequestsAsync(groupId, waitForData, cancellationToken);
            var groupShiftRequests = groupRequests.Where(r => r.RequestType.Equals(RequestType.Shift));

            if (dateFrom != null)
            {
                groupShiftRequests = groupShiftRequests.Where(r => r.Shift.EndDate >= dateFrom);
            }

            if (dateTo != null)
            {
                groupShiftRequests = groupShiftRequests.Where(r => r.Shift.StartDate <= dateTo);
            }

            return groupShiftRequests;
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

        public async Task<IEnumerable<JobBasic>> GetUserCompletedJobs(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);
            var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

            if (userRequests != null)
            {
                var userJobs = userRequests.SelectMany(r => r.JobBasics).Where(j => j.VolunteerUserID.Equals(userId));
                return userJobs;
            }
            throw new Exception($"Unable to get completed jobs for user {userId}");
        }
    }
}



