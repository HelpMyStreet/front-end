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
using HelpMyStreetFE.Services.Users;
using HelpMyStreet.Utils.EqualityComparers;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.ReportService;
using HelpMyStreetFE.Helpers;

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

        public async Task<bool> LogViewLocationEvent(int userId, int requestId, int jobId)
        {
            var logRequest = new LogRequestEventRequest()
            {
                JobID = jobId,
                RequestID = requestId,
                UserID = userId,
                RequestEventRequest = new RequestEventRequest()
                {
                    RequestEvent = RequestEvent.ShowFullPostCode
                }
            };

            var result = await _requestHelpRepository.LogEventRequest(logRequest);

            if (result != null)
            {
                return result.Success;
            }
            else
            {
                throw new Exception("Error when logging new event.");
            }
        }

        public async Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);

            if (userRequestIDs != null)
            {
                var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

                var userJobs = userRequests?.SelectMany(r => r.JobSummaries).Where(j => j.VolunteerUserID.Equals(userId));
                return userJobs;
            }
            return null;
        }

        public async Task<IEnumerable<JobBasic>> GetAllJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);

            if (userRequestIDs != null)
            {
                var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

                var userJobs = userRequests?.SelectMany(r => r.JobBasics).Where(j => j.VolunteerUserID.Equals(userId));
                return userJobs;
            }
            return null;
        }

        // My Requests tab
        public async Task<IEnumerable<RequestSummary>> GetRequestsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);

            if (userRequestIDs != null)
            {
                var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);

                // Check all request still include a job allocated to the user, and filter out shifts
                var userTaskRequests = userRequests?.Where(r => r.RequestType.Equals(RequestType.Task) && r.JobBasics.Exists(j => j.VolunteerUserID.Equals(userId)));
                return userTaskRequests;
            }
            return null;
        }

        public async Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            var openJobIDs = await _requestListCachingService.GetUserOpenJobsAsync(user, waitForData, cancellationToken);

            if (openJobIDs != null)
            {
                var openJobs = await _jobCachingService.GetShiftJobsAsync(openJobIDs, cancellationToken);

                // Check jobs are all still open
                var stillOpenJobs = openJobs?.Where(j => j.JobStatus.Equals(JobStatuses.Open));

                // Exclude duplicates of jobs the user has accepted
                var userJobs = await GetShiftsForUserAsync(user.ID, null, null, true, cancellationToken);
                var notMyJobs = stillOpenJobs?.Where(j => !userJobs.Contains(j, _shiftJobDedupe_EqualityComparer));

                var dedupedJobs = notMyJobs?.GroupBy(j => j, _shiftJobDedupe_EqualityComparer).Select(g => g.First());

                return dedupedJobs;
            }
            return null;
        }

        public async Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            var userRequestIDs = await _requestListCachingService.GetUserRequestsAsync(userId, waitForData, cancellationToken);

            if (userRequestIDs != null)
            {
                var userRequests = await _requestCachingService.GetRequestSummariesAsync(userRequestIDs, waitForData, cancellationToken);
                var userJobs = userRequests?.SelectMany(r => r.ShiftJobs).Where(j => j.VolunteerUserID.Equals(userId));
                return userJobs;
            }
            return null;
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

            if (groupRequestIDs == null)
            {
                if (waitForData) throw new Exception($"Unable to get list of group requests for group {groupId}");
                return null;
            }

            var groupRequests = await GetAllGroupRequestsAsync(groupId, waitForData, cancellationToken);
            var groupTaskRequests = groupRequests?.Where(r => r.RequestType.Equals(RequestType.Task));

            return groupTaskRequests;
        }

        public async Task<IEnumerable<RequestSummary>> GetAllGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            var groupRequestIDs = await _requestListCachingService.GetGroupRequestsAsync(groupId, waitForData, cancellationToken);

            if (groupRequestIDs != null)
            {
                var groupRequests = await _requestCachingService.GetRequestSummariesAsync(groupRequestIDs, waitForData, cancellationToken);
                return groupRequests;
            }
            return null;
        }

        public async Task<IEnumerable<JobSummary>> FilterAndDedupeOpenJobsForUser(IEnumerable<JobSummary> allJobs, User user, CancellationToken cancellationToken)
        {
            if (allJobs == null)
            {
                return null;
            }

            var dedupedJobs = allJobs.Distinct(_jobSummaryJobDedupeWithDate_EqualityComparer);
            var userJobs = await GetJobsForUserAsync(user.ID, true, cancellationToken);
            var notMyJobs = dedupedJobs.Where(s => !userJobs.Contains(s, _jobSummaryJobDedupeWithDate_EqualityComparer));

            return notMyJobs;
        }

        public async Task<IEnumerable<IEnumerable<JobSummary>>> GetDedupedOpenJobsForUserFromRepo(User user, bool waitForData, CancellationToken cancellationToken)
        {
            var openJobIDs = await _requestListCachingService.GetUserOpenJobsAsync(user, waitForData, cancellationToken);

            if (openJobIDs != null)
            {
                var openJobs = await _jobCachingService.GetJobSummariesAsync(openJobIDs, cancellationToken);

                // Check jobs are all still open
                var stillOpenJobs = openJobs?.Where(j => j.JobStatus.Equals(JobStatuses.Open));

                // Exclude duplicates of jobs the user has accepted
                var userJobs = await GetJobsForUserAsync(user.ID, true, cancellationToken);
                var notMyJobs = stillOpenJobs?.Where(j => !userJobs.Contains(j, _jobSummaryJobDedupeWithDate_EqualityComparer));

                var groupedJobs = notMyJobs?.GroupBy(j => j, _jobSummaryJobDedupe_EqualityComparer).Select(g => g.AsEnumerable());

                return groupedJobs;
            }
            return null;
        }

        public async Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(int groupId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken)
        {
            var groupRequests = await GetAllGroupRequestsAsync(groupId, waitForData, cancellationToken);

            var groupShiftRequests = groupRequests?.Where(r => r.RequestType.Equals(RequestType.Shift));

            if (dateFrom != null)
            {
                groupShiftRequests = groupShiftRequests?.Where(r => r.Shift.EndDate >= dateFrom);
            }

            if (dateTo != null)
            {
                groupShiftRequests = groupShiftRequests?.Where(r => r.Shift.StartDate <= dateTo);
            }

            return groupShiftRequests;
        }

        private async Task<List<EnrichedStatusHistory>> EnrichStatusHistory(List<StatusHistory> history, bool ShowNames, CancellationToken cancellationToken)
        {
            List<EnrichedStatusHistory> eHist = history
                .Where(x=> x.JobStatusChangeReasonCode == JobStatusChangeReasonCodes.UserChange)
                .OrderBy(o => o.StatusDate)
                .ThenBy(o=> o.JobStatus.UsualOrderOfProgression())
                .Select(h => new EnrichedStatusHistory(h)).ToList();


            int latestCreatedById = -1;

            for (int i = 0; i < eHist.Count; i++)
            {
                switch (eHist[i].StatusHistory.JobStatus)
                {
                    case JobStatuses.New:
                        eHist[i].JobStatusDescription = "Created";
                        break;
                    case JobStatuses.Open:
                        if (latestCreatedById > 0)
                        {
                            eHist[i].JobStatusDescription = "Released";
                            eHist[i].StatusHistory.CreatedByUserID = latestCreatedById;
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

                if ((eHist[i].StatusHistory.CreatedByUserID ?? -1) > 0)
                {
                    latestCreatedById = eHist[i].StatusHistory.CreatedByUserID.Value;

                    if (ShowNames)
                    {
                        eHist[i].ChangeMadeByUser = await _userService.GetUserAsync(eHist[i].StatusHistory.CreatedByUserID.Value, cancellationToken);
                    }
                }
            }

            return eHist;
        }

        public async Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId)
        {
            return await _requestHelpRepository.GetNewsTickerMessages(groupId);
        }

        public async Task<Chart> GetChart(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo)
        {
            return await _requestHelpRepository.GetChart(chart, groupId, dateFrom, dateTo);
        }
    }
}



