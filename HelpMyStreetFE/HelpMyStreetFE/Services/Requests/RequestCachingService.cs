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
    public class RequestCachingService : IRequestCachingService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;

        private readonly IMemDistCache<RequestSummary> _memDistCache_RequestSummary;
        private readonly IMemDistCache<JobSummary> _memDistCache_JobSummary;

        private const string CACHE_KEY_PREFIX = "request-caching-service";

        public RequestCachingService(IRequestHelpRepository requestHelpRepository, IMemDistCache<RequestSummary> memDistCache_RequestSummary, IMemDistCache<JobSummary> memDistCache_JobSummary)
        {
            _requestHelpRepository = requestHelpRepository;
            _memDistCache_RequestSummary = memDistCache_RequestSummary;
            _memDistCache_JobSummary = memDistCache_JobSummary;
        }

        public async Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync(IEnumerable<int> requestIds, CancellationToken cancellationToken)
        {
            var results = new List<RequestSummary>();
            var missingIds = new List<int>();

            foreach (int requestId in requestIds)
            {
                var requestSummary = await GetRequestSummaryAsync(requestId, RefreshBehaviour.DontRefreshData, NotInCacheBehaviour.DontGetData, cancellationToken);
                if (requestSummary == null)
                {
                    missingIds.Add(requestId);
                }
                else
                {
                    results.Add(requestSummary);
                }
            }

            if (missingIds.Count > 0)
            {
                // To Do: Add these to the cache
                results.AddRange(await _requestHelpRepository.GetRequestSummariesAsync(missingIds));
            }

            return results;
        }

        public async Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken)
        {
            return await GetRequestSummaryAsync(requestId, RefreshBehaviour.WaitForFreshData, NotInCacheBehaviour.WaitForData, cancellationToken);
        }

        public async Task<JobSummary> GetJobSummaryAsync(int jobId, CancellationToken cancellationToken)
        {
            return await GetJobSummaryAsync(jobId, RefreshBehaviour.WaitForFreshData, NotInCacheBehaviour.WaitForData, cancellationToken);
        }

        private async Task<RequestSummary> GetRequestSummaryAsync(int requestId, RefreshBehaviour refreshBehaviour, NotInCacheBehaviour notInCacheBehaviour, CancellationToken cancellationToken)
        {
            return await _memDistCache_RequestSummary.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestSummaryAsync(requestId);
            }, $"{CACHE_KEY_PREFIX}-request-{requestId}", refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        private async Task<JobSummary> GetJobSummaryAsync(int jobId, RefreshBehaviour refreshBehaviour, NotInCacheBehaviour notInCacheBehaviour, CancellationToken cancellationToken)
        {
            return await _memDistCache_JobSummary.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetJobSummaryAsync(jobId);
            }, $"{CACHE_KEY_PREFIX}-job-{jobId}", refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        public void TriggerRequestCacheRefresh(int requestId, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var requestSummary = await _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetRequestSummaryAsync(requestId);
                }, $"{CACHE_KEY_PREFIX}-request-{requestId}", cancellationToken);

                foreach(var job in requestSummary.JobSummaries)
                {
                    _ = _memDistCache_JobSummary.RefreshDataAsync(async (cancellationToken) =>
                    {
                        return await _requestHelpRepository.GetJobSummaryAsync(job.JobID);
                    }, $"{CACHE_KEY_PREFIX}-job-{job.JobID}", cancellationToken);
                }
            });
        }

        public void TriggerJobCacheRefresh(int jobId, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var jobSummary = await _memDistCache_JobSummary.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetJobSummaryAsync(jobId);
                }, $"{CACHE_KEY_PREFIX}-job-{jobId}", cancellationToken);

                _ = _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetRequestSummaryAsync(jobSummary.RequestID);
                }, $"{CACHE_KEY_PREFIX}-request-{jobSummary.RequestID}", cancellationToken);
            });
        }
    }
}
