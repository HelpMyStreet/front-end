using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.RequestHelp.Stages.Detail;
using HelpMyStreetFE.Models.RequestHelp.Stages.Request;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestService
    {
        Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetRequestsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);

        Task<IEnumerable<JobSummary>> GetOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<IEnumerable<JobSummary>>> GetGroupedOpenJobsForUserFromRepo(User user, CancellationToken cancellationToken);
        Task<IEnumerable<JobSummary>> FilterAndDedupeOpenJobsForUser(IEnumerable<JobSummary> allJobs, User user, CancellationToken cancellationToken);

        Task<IEnumerable<RequestSummary>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetGroupRequestsAsync(string groupKey, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetAllGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken);
        Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken);
        Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken);
        Task<GetRequestDetailsResponse> GetRequestDetailAsync(int requestId, int userId, CancellationToken cancellationToken);
        Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobAndRequestSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, bool adminView, CancellationToken cancellationToken);
        Task<UpdateJobStatusOutcome?> UpdateRequestStatusAsync(int requestId, JobStatuses status, int createdByUserId, CancellationToken cancellationToken);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken);
        Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpJourney requestHelpJourney, int referringGroupID, string source);
        OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<IEnumerable<JobSummary>> jobs);
        Task<JobLocation> LocateJob(int jobId, int userId, CancellationToken cancellationToken);
        Task<JobLocation> LocateRequest(int requestId, int userId, CancellationToken cancellationToken);

        Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(int groupId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(string groupKey, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
    }
}