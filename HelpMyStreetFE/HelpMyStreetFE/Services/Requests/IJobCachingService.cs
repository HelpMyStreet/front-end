using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IJobCachingService
    {
        Task<IEnumerable<JobSummary>> GetJobSummariesAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken);
        Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken);

        Task<IEnumerable<ShiftJob>> GetShiftJobsAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken);
        Task<ShiftJob> GetShiftJobAsync(int jobId, CancellationToken cancellationToken);

        Task<IEnumerable<JobBasic>> GetJobBasicsAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken);
        Task<JobBasic> GetJobBasicAsync(int jobId, CancellationToken cancellationToken);

        Task<int> GetRequestId(int jobId, CancellationToken cancellationToken);

        Task RefreshCacheAsync(int jobId, CancellationToken cancellationToken);
    }
}
