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

        public RequestCachingService(IRequestHelpRepository requestHelpRepository, IMemDistCache<RequestSummary> memDistCache_RequestSummary)
        {
            _requestHelpRepository = requestHelpRepository;
            _memDistCache_RequestSummary = memDistCache_RequestSummary;
        }

        public async Task<IEnumerable<RequestSummary>> GetRequestSummariesAsync(IEnumerable<int> requestIds, bool waitForData, CancellationToken cancellationToken)
        {
            var results = new List<RequestSummary>();
            var missingIds = new List<int>();

            foreach (int requestId in requestIds.Distinct())
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
                if (waitForData)
                {
                    results.AddRange(await AddRequestSummariesToCache(missingIds, cancellationToken));
                }
                else
                {
#pragma warning disable CS4014
                    Task.Factory.StartNew(async () => await AddRequestSummariesToCache(missingIds, cancellationToken));
#pragma warning restore CS4014
                    
                    return new List<RequestSummary>();
                }
            }

            return results;
        }

        public async Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken)
        {
            return await GetRequestSummaryAsync(requestId, RefreshBehaviour.WaitForFreshData, NotInCacheBehaviour.WaitForData, cancellationToken);
        }

        public async Task RefreshCacheAsync(int requestId, CancellationToken cancellationToken)
        {
            await _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestSummaryAsync(requestId);
            }, GetRequestCacheKey(requestId), cancellationToken);
        }

        private async Task<IEnumerable<RequestSummary>> AddRequestSummariesToCache(IEnumerable<int> requestIds, CancellationToken cancellationToken)
        {
            var requestSummaries = await _requestHelpRepository.GetRequestSummariesAsync(requestIds);

            foreach (var requestSummary in requestSummaries)
            {
                _ = _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
                {
                    return requestSummary;
                }, GetRequestCacheKey(requestSummary.RequestID), cancellationToken);
            }

            return requestSummaries;
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
