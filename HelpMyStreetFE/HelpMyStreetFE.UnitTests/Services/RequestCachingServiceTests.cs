﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Cache.Models;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace HelpMyStreetFE.UnitTests.Services
{
    class RequestCachingServiceTests
    {
        private Mock<IRequestHelpRepository> _repo;
        private Mock<IMemDistCache<RequestSummary>> _cache;
        private Mock<ILogger<RequestCachingService>> _logger;

        private RequestCachingService _classUnderTest;

        private Dictionary<string, CachedItemWrapper<RequestSummary>> _requestSummaries;
        private CancellationToken _cancellationToken;

        private int _waitForBackgroundThreadToCompleteMs = 100;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IRequestHelpRepository>();
            _cache = new Mock<IMemDistCache<RequestSummary>>();
            _logger = new Mock<ILogger<RequestCachingService>>();

            _requestSummaries = new Dictionary<string, CachedItemWrapper<RequestSummary>> {
                { "request-caching-service-request-1", new CachedItemWrapper<RequestSummary>(new RequestSummary{ RequestID = 1}, DateTime.MaxValue) },
                { "request-caching-service-request-2", new CachedItemWrapper<RequestSummary>(new RequestSummary{ RequestID = 2}, DateTime.MaxValue) },
                { "request-caching-service-request-3", new CachedItemWrapper<RequestSummary>(new RequestSummary{ RequestID = 3}, DateTime.MaxValue) },
                { "request-caching-service-stale-request-98", new CachedItemWrapper<RequestSummary>(new RequestSummary{ RequestID = 98}, DateTime.MinValue) },
                { "request-caching-service-stale-request-99", new CachedItemWrapper<RequestSummary>(new RequestSummary{ RequestID = 99}, DateTime.MinValue) },
            };

            _cancellationToken = new CancellationToken();

            _cache.Setup(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(),
                                                   It.IsAny<string>(),
                                                   It.IsAny<RefreshBehaviour>(),
                                                   It.IsAny<CancellationToken>(),
                                                   It.IsAny<NotInCacheBehaviour>(),
                                                   null))
                                    .ReturnsAsync((Func<CancellationToken, Task<RequestSummary>> dataGetter,
                                                   string key,
                                                   RefreshBehaviour refreshBehaviour,
                                                   CancellationToken cancellationToken,
                                                   NotInCacheBehaviour notInCacheBehaviour,
                                                   Func<DateTimeOffset, DateTimeOffset> whenDataIsStaleDelegate)
                                                        => _requestSummaries.ContainsKey(key) ? _requestSummaries[key] : new CachedItemWrapper<RequestSummary>(null, DateTime.MinValue));

            _repo.Setup(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()))
                                    .ReturnsAsync((IEnumerable<int> requestIDs) => requestIDs.Select(r => new RequestSummary { RequestID = r }));

            _classUnderTest = new RequestCachingService(_repo.Object, _logger.Object, _cache.Object);
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenInvoked_DontWaitForData_GetsFromCache()
        {
            var ids = new List<int> { 1, 1, 2, 3 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, false, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenInvoked_WaitForData_GetsFromCache()
        {
            var ids = new List<int> { 1, 1, 2, 3 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, true, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenStale_WaitForData_GetsFromCacheAndTriggersBackgroundRefresh()
        {
            var ids = new List<int> { 1, 1, 2, 3, 98, 99 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, true, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(5));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Exactly(2));
            Assert.AreEqual(5, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenRequestIsMissing_DontWaitForData_ReturnsExmptyList()
        {
            var ids = new List<int> { 1, 1, 2, 3, 4, 5, 6, 6 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, false, _cancellationToken);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            _cache.Verify(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(6));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Exactly(3));
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenRequestIsMissing_WaitForData_ReturnsFromCacheAndRepo()
        {
            var ids = new List<int> { 1, 1, 2, 3, 4, 5, 6, 6 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, true, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(6));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Exactly(3));
            Assert.AreEqual(6, result.Count());
        }

        [Test]
        public async Task GetRequestSummaryAsync_WhenInvoked_GetsFromCache()
        {
            var result = await _classUnderTest.GetRequestSummaryAsync(2, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataInWrapperAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), "request-caching-service-request-2", RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.WaitForData, null), Times.Once);
            Assert.AreEqual(2, result.RequestID);
        }

        [Test]
        public async Task RefreshCacheAsync_WhenInvoked_RefreshesCache()
        {
            await _classUnderTest.RefreshCacheAsync(1, _cancellationToken);

            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), "request-caching-service-request-1", _cancellationToken, null), Times.Once);
        }

        [Test]
        public async Task RefreshCacheAsync_IEnumerable_WhenInvoked_RefreshesCache()
        {
            var ids = new List<int> { 7, 8, 9 };

            await _classUnderTest.RefreshCacheAsync(ids, _cancellationToken);

            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), _cancellationToken, It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Exactly(3));
        }
    }
}
