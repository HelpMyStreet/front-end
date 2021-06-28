using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestCachingService
    {
        Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync(IEnumerable<int> requestIds, CancellationToken cancellationToken);
        Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken);
        Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken);

        void TriggerRequestCacheRefresh(int requestId, CancellationToken cancellationToken);
        void TriggerJobCacheRefresh(int jobId, CancellationToken cancellationToken);
    }
}
