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

        private readonly IMemDistCache<int> _memDistCache_RequestIdLookup;

        private const string CACHE_KEY_PREFIX = "job-caching-service";

        public JobCachingService(IRequestHelpRepository requestHelpRepository, IRequestCachingService requestCachingService, IMemDistCache<int> memDistCache_int)
        {
            _requestHelpRepository = requestHelpRepository;
            _requestCachingService = requestCachingService;

            _memDistCache_RequestIdLookup = memDistCache_int;
        }

        public async Task<IEnumerable<JobSummary>> GetJobSummariesAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken)
        {
            var requests = await GetRequestSummariesAsync(jobIds, cancellationToken);

            var allJobs = requests.SelectMany(r => r.JobSummaries);

            return allJobs.Where(j => jobIds.Contains(j.JobID));
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            var requestId = await GetRequestId(jobId, cancellationToken);

            var requestSummary = await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken);

            return requestSummary.JobSummaries.FirstOrDefault(j => j.JobID.Equals(jobId));
        }

        public async Task<IEnumerable<ShiftJob>> GetShiftJobsAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken)
        {
            var requests = await GetRequestSummariesAsync(jobIds, cancellationToken);

            var allJobs = requests.SelectMany(r => r.ShiftJobs);

            return allJobs.Where(j => jobIds.Contains(j.JobID));
        }

        public async Task<ShiftJob> GetShiftJobAsync(int jobId, CancellationToken cancellationToken)
        {
            var requestId = await GetRequestId(jobId, cancellationToken);

            var requestSummary = await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken);

            return requestSummary.ShiftJobs.FirstOrDefault(j => j.JobID.Equals(jobId));
        }

        public async Task<IEnumerable<JobBasic>> GetJobBasicsAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken)
        {
            var requests = await GetRequestSummariesAsync(jobIds, cancellationToken);

            var allJobs = requests.SelectMany(r => r.JobBasics);

            return allJobs.Where(j => jobIds.Contains(j.JobID));
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

        private async Task<int> GetRequestId(int jobId, CancellationToken cancellationToken)
        {
            var requestId = await GetRequestId(jobId, RefreshBehaviour.DontWaitForFreshData, NotInCacheBehaviour.WaitForData, cancellationToken);

            if (requestId == default)
            {
                throw new Exception($"Failed to get RequestId for job {jobId}");
            }

            return requestId;
        }

        private async Task<int> GetRequestId(int jobId, RefreshBehaviour refreshBehaviour, NotInCacheBehaviour notInCacheBehaviour, CancellationToken cancellationToken)
        {
            return await _memDistCache_RequestIdLookup.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestId(jobId);
            }, GetJobCacheKey(jobId), refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        private async Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken)
        {
            var requestIds = new List<int>();
            var missingJobIds = new List<int>();

            foreach (int jobId in jobIds)
            {
                var requestId = await GetRequestId(jobId, RefreshBehaviour.DontRefreshData, NotInCacheBehaviour.DontGetData, cancellationToken);

                if (requestId == default)
                {
                    missingJobIds.Add(jobId);
                }
                else
                {
                    requestIds.Add(requestId);
                }
            }

            if (missingJobIds.Count > 0)
            {
                var missingRequestIdDictionary = await RefreshRequestIdLookupCacheAsync(missingJobIds, cancellationToken);
                requestIds.AddRange(missingRequestIdDictionary.Select(a => a.Value));
            }

            return await _requestCachingService.GetRequestSummariesAsync(requestIds, true, cancellationToken);
        }

        private async Task<Dictionary<int, int>> RefreshRequestIdLookupCacheAsync(IEnumerable<int> jobIds, CancellationToken cancellationToken)
        {
            var missingIds = await _requestHelpRepository.GetRequestIDs(jobIds);

            foreach (var item in missingIds)
            {
                _ = _memDistCache_RequestIdLookup.RefreshDataAsync(async (cancellationToken) =>
                {
                    return item.Value;
                }, GetJobCacheKey(item.Key), cancellationToken);
            }

            return missingIds;
        }

        private string GetJobCacheKey(int jobId)
        {
            return $"{CACHE_KEY_PREFIX}-job-{jobId}";
        }
    }
}
