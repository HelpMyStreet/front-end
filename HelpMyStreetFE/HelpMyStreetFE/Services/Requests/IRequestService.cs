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
        Task<IEnumerable<JobHeader>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobHeader>> GetOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobHeader>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobHeader>> GetGroupRequestsAsync(string groupKey, bool waitForData, CancellationToken cancellationToken);
        Task<LogRequestResponse> LogRequestAsync(RequestHelpRequestStageViewModel requestStage, RequestHelpDetailStageViewModel detailStage, int referringGroupID, string source, int userId, CancellationToken cancellationToken);
        Task<GetRequestDetailsResponse> GetRequestDetailAsync(int requestId, int userId, CancellationToken cancellationToken);
        Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobAndRequestSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, CancellationToken cancellationToken);
        Task<UpdateJobStatusOutcome?> UpdateJobStatusAsync(int jobID, JobStatuses status, int createdByUserId, int? volunteerUserId, CancellationToken cancellationToken);
        Task<RequestHelpViewModel> GetRequestHelpSteps(RequestHelpJourney requestHelpJourney, int referringGroupID, string source);
        OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<JobHeader> jobs);
        Task<JobLocation> LocateJob(int jobId, int userId, CancellationToken cancellationToken);

        Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(int groupId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(string groupKey, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
    }
}