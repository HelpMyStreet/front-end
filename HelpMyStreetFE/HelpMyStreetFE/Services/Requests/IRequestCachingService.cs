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
        Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync(IEnumerable<int> requestIds, bool waitForData, CancellationToken cancellationToken);
        Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken);
        Task<RequestSummary> RefreshCacheAsync(int requestId, CancellationToken cancellationToken);
        Task RefreshCacheForAllRequestIdsAsync(List<int> requestIds, CancellationToken cancellationToken);
    }
}
