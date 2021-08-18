using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestCachingService : IRequestCachingService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestCachingService> _logger;

        private readonly IMemDistCache<RequestSummary> _memDistCache_RequestSummary;

        private const string CACHE_KEY_PREFIX = "request-caching-service";

        public RequestCachingService(IRequestHelpRepository requestHelpRepository, ILogger<RequestCachingService> logger, IMemDistCache<RequestSummary> memDistCache_RequestSummary)
        {
            _requestHelpRepository = requestHelpRepository ?? throw new ArgumentNullException(nameof(requestHelpRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memDistCache_RequestSummary = memDistCache_RequestSummary ?? throw new ArgumentNullException(nameof(memDistCache_RequestSummary));
        }

        /// <summary>
        /// Returns RequestSummaries from cache if possible, or from the repo
        /// </summary>
        /// <param name="requestIds">Requests to fetch</param>
        /// <param name="waitForData">If true, RequestSummaries will be fetched from the repo before returning.  If false, an empty List will be returned whilst any missing RequestSummaries are added to the cache in a separate thread</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
                    results.AddRange(await RefreshCacheAsync(missingIds, cancellationToken));
                }
                else
                {
#pragma warning disable CS4014
                    Task.Factory.StartNew(async () => await RefreshCacheAsync(missingIds, cancellationToken));
#pragma warning restore CS4014
                    
                    return new List<RequestSummary>();
                }
            }

            return results;
        }

        /// <summary>
        /// Returns a RequestSummary, from cache if possible or from the repo if not
        /// </summary>
        /// <param name="requestId">Request to fetch</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RequestSummary> GetRequestSummaryAsync(int requestId, CancellationToken cancellationToken)
        {
            return await GetRequestSummaryAsync(requestId, RefreshBehaviour.DontWaitForFreshData, NotInCacheBehaviour.WaitForData, cancellationToken);
        }

        /// <summary>
        /// Fetches the latest RequestSummary from the repo, and updates the cache
        /// </summary>
        /// <param name="requestId">Request to fetch</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RequestSummary> RefreshCacheAsync(int requestId, CancellationToken cancellationToken)
        {
            return await _memDistCache_RequestSummary.RefreshDataAsync(async (cancellationToken) =>
            {
                return await _requestHelpRepository.GetRequestSummaryAsync(requestId);
            }, GetRequestCacheKey(requestId), cancellationToken);
        }

        /// <summary>
        /// Fetches RequestSummaries from the repo, and updates the cache
        /// </summary>
        /// <param name="requestIds">Requests to fetch</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RequestSummary>> RefreshCacheAsync(IEnumerable<int> requestIds, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Refreshing cache for RequestIDs {string.Join(',', requestIds)}");

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
