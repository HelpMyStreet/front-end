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
        Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken);
        Task<JobBasic> GetJobBasicAsync(int jobId, CancellationToken cancellationToken);
        Task<int> GetRequestId(int jobId, CancellationToken cancellationToken);
        Task TriggerCacheRefresh(int jobId, CancellationToken cancellationToken);
    }
}
