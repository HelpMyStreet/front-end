using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Repositories;

namespace HelpMyStreetFE.Services.Requests
{
    public class JobCachingService : IJobCachingService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly IRequestCachingService _requestCachingService;

        private readonly IMemDistCache<int> _memDistCache_int;

        private const string CACHE_KEY_PREFIX = "job-caching-service";

        public JobCachingService(IRequestHelpRepository requestHelpRepository, IRequestCachingService requestCachingService, IMemDistCache<int> memDistCache_int)
        {
            _requestHelpRepository = requestHelpRepository;
            _requestCachingService = requestCachingService;

            _memDistCache_int = memDistCache_int;
        }

        public async Task<int> GetRequestId(int jobId, CancellationToken cancellationToken)
        {
            return await _memDistCache_int.GetCachedDataAsync(async (cancellationToken) =>
            {
                var jobSummary = await _requestHelpRepository.GetJobSummaryAsync(jobId);
                return jobSummary.RequestID;
            }, GetJobCacheKey(jobId), RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            var requestId = await GetRequestId(jobId, cancellationToken);

            var requestSummary = await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken);

            return requestSummary.JobSummaries.FirstOrDefault(j => j.JobID.Equals(jobId));
        }

        public async Task<JobBasic> GetJobBasicAsync(int jobId, CancellationToken cancellationToken)
        {
            var requestId = await GetRequestId(jobId, cancellationToken);

            var requestSummary = await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken);

            return requestSummary.JobBasics.FirstOrDefault(j => j.JobID.Equals(jobId));
        }

        public async Task RefreshCacheAsync(int jobId, CancellationToken cancellationToken)
        {
            var requestId = await GetRequestId(jobId, cancellationToken);

            await _requestCachingService.RefreshCacheAsync(requestId, cancellationToken);
        }

        private string GetJobCacheKey(int jobId)
        {
            return $"{CACHE_KEY_PREFIX}-job-{jobId}";
        }
    }
}
