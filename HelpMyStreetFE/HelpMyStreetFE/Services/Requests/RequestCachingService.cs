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

        private const string CACHE_KEY_PREFIX = "request-caching-service";

        public RequestCachingService(IRequestHelpRepository requestHelpRepository, IMemDistCache<RequestSummary> memDistCache_RequestSummary, IMemDistCache<JobSummary> memDistCache_JobSummary)
        {
            _requestHelpRepository = requestHelpRepository;
            _memDistCache_RequestSummary = memDistCache_RequestSummary;
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
                var missingRequestSummaries = await _requestHelpRepository.GetRequestSummariesAsync(missingIds);

                results.AddRange(missingRequestSummaries);

                foreach (var requestSummary in missingRequestSummaries)
                {
                    _ = _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
                    {
                        return requestSummary;
                    }, GetRequestCacheKey(requestSummary.RequestID), cancellationToken);
                }
            }

            return results;
        }

        public async Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken)
        {
            return await GetRequestSummaryAsync(requestId, RefreshBehaviour.WaitForFreshData, NotInCacheBehaviour.WaitForData, cancellationToken);
        }

        public void TriggerRequestCacheRefresh(int requestId, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var requestSummary = await _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await _requestHelpRepository.GetRequestSummaryAsync(requestId);
                }, GetRequestCacheKey(requestId), cancellationToken);
            });
        }

        private async Task<RequestSummary> GetRequestSummaryAsync(int requestId, RefreshBehaviour refreshBehaviour, NotInCacheBehaviour notInCacheBehaviour, CancellationToken cancellationToken)
        {
            return await _memDistCache_RequestSummary.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestSummaryAsync(requestId);
            }, GetRequestCacheKey(requestId), refreshBehaviour, cancellationToken, notInCacheBehaviour);
        }

        private string GetRequestCacheKey(int requestId)
        {
            return $"{CACHE_KEY_PREFIX}-request-{requestId}";
        }
    }
}
