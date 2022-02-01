using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.ReportService;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestService
    {
        Task<Chart> GetChart(Charts chart, int groupId, DateTime dateFrom, DateTime dateTo);
        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
        Task<IEnumerable<JobBasic>> GetAllJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobSummary>> GetJobsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetRequestsForUserAsync(int userId, bool waitForData, CancellationToken cancellationToken);

        Task<IEnumerable<IEnumerable<JobSummary>>> GetDedupedOpenJobsForUserFromRepo(User user, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<JobSummary>> FilterAndDedupeOpenJobsForUser(IEnumerable<JobSummary> allJobs, User user, CancellationToken cancellationToken);

        Task<IEnumerable<RequestSummary>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetAllGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken);

        Task<GetRequestDetailsResponse> GetRequestDetailAsync(int requestId, int userId, CancellationToken cancellationToken);

        Task<JobDetail> GetJobAndRequestSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobDetail> GetJobDetailsAsync(int jobId, int userId, bool adminView, CancellationToken cancellationToken);
        OpenJobsViewModel SplitOpenJobs(User user, IEnumerable<IEnumerable<JobSummary>> jobs);

        Task<IEnumerable<ShiftJob>> GetOpenShiftsForUserAsync(User user, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<ShiftJob>> GetShiftsForUserAsync(int userId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<IEnumerable<RequestSummary>> GetGroupShiftRequestsAsync(int groupId, DateTime? dateFrom, DateTime? dateTo, bool waitForData, CancellationToken cancellationToken);
        Task<bool> LogViewLocationEvent(int userId, int requestId, int jobId);
    }
}