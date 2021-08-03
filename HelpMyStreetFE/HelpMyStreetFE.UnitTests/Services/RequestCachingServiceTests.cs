using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
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

        private Dictionary<string, RequestSummary> _requestSummaries;
        private CancellationToken _cancellationToken;

        private int _waitForBackgroundThreadToCompleteMs = 100;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IRequestHelpRepository>();
            _cache = new Mock<IMemDistCache<RequestSummary>>();
            _logger = new Mock<ILogger<RequestCachingService>>();

            _requestSummaries = new Dictionary<string, RequestSummary> {
                { "request-caching-service-request-1", new RequestSummary{ RequestID = 1} },
                { "request-caching-service-request-2", new RequestSummary{ RequestID = 2} },
                { "request-caching-service-request-3", new RequestSummary{ RequestID = 3} },
            };

            _cancellationToken = new CancellationToken();

            _cache.Setup(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(),
                                                   It.IsAny<string>(),
                                                   It.IsAny<RefreshBehaviour>(),
                                                   It.IsAny<CancellationToken>(),
                                                   It.IsAny<NotInCacheBehaviour>()))
                                    .ReturnsAsync((Func<CancellationToken, Task<RequestSummary>> dataGetter,
                                                   string key,
                                                   RefreshBehaviour refreshBehaviour,
                                                   CancellationToken cancellationToken,
                                                   NotInCacheBehaviour notInCacheBehaviour)
                                                        => _requestSummaries.ContainsKey(key) ? _requestSummaries[key] : null);

            _repo.Setup(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()))
                                    .ReturnsAsync((IEnumerable<int> requestIDs) => requestIDs.Select(r => new RequestSummary { RequestID = r }));

            _classUnderTest = new RequestCachingService(_repo.Object, _logger.Object, _cache.Object);
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenInvoked_DontWaitForData_GetsFromCache()
        {
            var ids = new List<int> { 1, 1, 2, 3 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, false, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenInvoked_WaitForData_GetsFromCache()
        {
            var ids = new List<int> { 1, 1, 2, 3 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, true, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenRequestIsMissing_DontWaitForData_ReturnsExmptyList()
        {
            var ids = new List<int> { 1, 1, 2, 3, 4, 5, 6, 6 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, false, _cancellationToken);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData), Times.Exactly(6));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task GetRequestSummariesAsync_WhenRequestIsMissing_WaitForData_ReturnsFromCacheAndRepo()
        {
            var ids = new List<int> { 1, 1, 2, 3, 4, 5, 6, 6 };

            var result = await _classUnderTest.GetRequestSummariesAsync(ids, true, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData), Times.Exactly(6));
            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            Assert.AreEqual(6, result.Count());
        }

        [Test]
        public async Task GetRequestSummaryAsync_WhenInvoked_GetsFromCache()
        {
            var result = await _classUnderTest.GetRequestSummaryAsync(2, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), "request-caching-service-request-2", RefreshBehaviour.DontWaitForFreshData, _cancellationToken, NotInCacheBehaviour.WaitForData), Times.Once);
            Assert.AreEqual(2, result.RequestID);
        }

        [Test]
        public async Task RefreshCacheAsync_WhenInvoked_RefreshesCache()
        {
            await _classUnderTest.RefreshCacheAsync(1, _cancellationToken);

            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), "request-caching-service-request-1", _cancellationToken), Times.Once);
        }

        [Test]
        public async Task RefreshCacheAsync_IEnumerable_WhenInvoked_RefreshesCache()
        {
            var ids = new List<int> { 7, 8, 9 };

            await _classUnderTest.RefreshCacheAsync(ids, _cancellationToken);

            _repo.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<RequestSummary>>>(), It.IsAny<string>(), _cancellationToken), Times.Exactly(3));
        }
    }
}
