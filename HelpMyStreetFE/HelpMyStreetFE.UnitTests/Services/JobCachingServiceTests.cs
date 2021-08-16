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
using Moq;
using NUnit.Framework;

namespace HelpMyStreetFE.UnitTests.Services
{
    class JobCachingServiceTests
    {
        private Mock<IRequestHelpRepository> _repo;
        private Mock<IRequestCachingService> _requestCachingService;
        private Mock<IMemDistCache<int>> _cache;

        private JobCachingService _classUnderTest;

        private IEnumerable<RequestSummary> _requestSummaries;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _repo = new Mock<IRequestHelpRepository>();
            _requestCachingService = new Mock<IRequestCachingService>();
            _cache = new Mock<IMemDistCache<int>>();

            _requestSummaries = new List<RequestSummary> {
                new RequestSummary {
                    RequestID = 1,
                    JobSummaries = new List<JobSummary> { new JobSummary { JobID = 11 }, new JobSummary {JobID = 12 }, new JobSummary {JobID = 13 } },
                    ShiftJobs = new List<ShiftJob>{ new ShiftJob { JobID = 14 } } ,
                },
                new RequestSummary { 
                    RequestID = 2, 
                    JobSummaries = new List<JobSummary> { new JobSummary { JobID = 21 }, new JobSummary {JobID = 22 }, new JobSummary {JobID = 23 } },
                    ShiftJobs = new List<ShiftJob>(),
                },
                new RequestSummary {
                    RequestID = 3,
                    JobSummaries = new List<JobSummary>(),
                    ShiftJobs = new List<ShiftJob> { new ShiftJob { JobID = 31 }, new ShiftJob { JobID = 32 }, new ShiftJob { JobID = 33 } },
                },
                new RequestSummary {
                    RequestID = 9,
                    JobSummaries = new List<JobSummary>(),
                    ShiftJobs = new List<ShiftJob> { new ShiftJob { JobID = 97 }, new ShiftJob { JobID = 98 }, new ShiftJob { JobID = 99 } },
                },
            };

            _cancellationToken = new CancellationToken();

            _requestCachingService.Setup(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<int> requestIds, bool waitForData, CancellationToken cancellationToken) =>
                {
                    var a = requestIds.Count();
                    return _requestSummaries;
                });

            _requestCachingService.Setup(x => x.GetRequestSummaryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(_requestSummaries.First());

            _cache.Setup(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<RefreshBehaviour>(), It.IsAny<CancellationToken>(), It.IsAny<NotInCacheBehaviour>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()))
                .ReturnsAsync((Func<CancellationToken, Task<int>> dataGetter, string key, RefreshBehaviour refreshBehaviour, CancellationToken cancellationToken, NotInCacheBehaviour notInCacheBehaviour, Func<DateTimeOffset, DateTimeOffset> whenDataIsStaleDelegate) => {
                    var requestId = key.Reverse().ToArray().ElementAt(1);
                    if (requestId == '9')
                    {
                        // Cache does not yet contain RequestId 9
                        return default;
                    }
                    else
                    {
                        return (int)Char.GetNumericValue(requestId);
                    }
                });

            _repo.Setup(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>())).ReturnsAsync((IEnumerable<int> jobIds) => {
                var result = new Dictionary<int, int>();

                foreach (var jobId in jobIds)
                {
                    result.Add(jobId, jobId % 10);
                }

                return result;
            });

            _classUnderTest = new JobCachingService(_repo.Object, _requestCachingService.Object, _cache.Object);
        }

        [Test]
        public async Task GetJobSummariesAsync_WhenInvoked_GetsFromCache()
        {
            var jobIds = new List<int> { 11, 12, 31 };

            var result = await _classUnderTest.GetJobSummariesAsync(jobIds, _cancellationToken);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(11, result.First().JobID);
            Assert.AreEqual(12, result.ElementAt(1).JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Never);
            _requestCachingService.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>(), _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
        }

        [Test]
        public async Task GetJobSummaryAsync_WhenInvoked_GetsFromCache()
        {
            var jobId = 13;

            var result = await _classUnderTest.GetJobSummaryAsync(jobId, _cancellationToken);

            Assert.AreEqual(jobId, result.JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), "job-caching-service-job-13", RefreshBehaviour.DontWaitForFreshData, _cancellationToken, NotInCacheBehaviour.WaitForData, null), Times.Once);
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Never);
            _requestCachingService.Verify(x => x.GetRequestSummaryAsync(1, _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
        }

        [Test]
        public async Task GetShiftJobsAsync_WhenInvoked_GetsFromCache()
        {
            var jobIds = new List<int> { 11, 12, 31 };

            var result = await _classUnderTest.GetShiftJobsAsync(jobIds, _cancellationToken);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(31, result.First().JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Never);
            _requestCachingService.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>(), _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
        }

        [Test]
        public async Task GetShiftJobAsync_WhenInvoked_GetsFromCache()
        {
            var jobId = 14;

            var result = await _classUnderTest.GetShiftJobAsync(jobId, _cancellationToken);

            Assert.AreEqual(jobId, result.JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), "job-caching-service-job-14", RefreshBehaviour.DontWaitForFreshData, _cancellationToken, NotInCacheBehaviour.WaitForData, null), Times.Once);
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Never);
            _requestCachingService.Verify(x => x.GetRequestSummaryAsync(1, _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
        }

        [Test]
        public async Task GetJobBasicsAsync_WhenInvoked_GetsFromCache()
        {
            var jobIds = new List<int> { 11, 12, 31 };

            var result = await _classUnderTest.GetJobBasicsAsync(jobIds, _cancellationToken);

            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(11, result.First().JobID);
            Assert.AreEqual(12, result.ElementAt(1).JobID);
            Assert.AreEqual(31, result.ElementAt(2).JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(3));
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Never);
            _requestCachingService.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>(), _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
        }

        [Test]
        public async Task GetJobBasicAsync_WhenInvoked_GetsFromCache()
        {
            var jobId = 14;

            var result = await _classUnderTest.GetShiftJobAsync(jobId, _cancellationToken);

            Assert.AreEqual(jobId, result.JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), "job-caching-service-job-14", RefreshBehaviour.DontWaitForFreshData, _cancellationToken, NotInCacheBehaviour.WaitForData, null), Times.Once);
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Never);
            _requestCachingService.Verify(x => x.GetRequestSummaryAsync(1, _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Never);
        }

        [Test]
        public async Task GetJobBasicsAsync_WhenInvokedWithMissingJobIds_GetsFromCacheAndRepo()
        {
            var jobIds = new List<int> { 11, 12, 31, 97, 98, 99 };

            var result = await _classUnderTest.GetJobBasicsAsync(jobIds, _cancellationToken);

            Assert.AreEqual(6, result.Count());
            Assert.AreEqual(11, result.First().JobID);
            Assert.AreEqual(12, result.ElementAt(1).JobID);
            Assert.AreEqual(31, result.ElementAt(2).JobID);
            Assert.AreEqual(97, result.ElementAt(3).JobID);
            Assert.AreEqual(98, result.ElementAt(4).JobID);
            Assert.AreEqual(99, result.ElementAt(5).JobID);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), RefreshBehaviour.DontRefreshData, _cancellationToken, NotInCacheBehaviour.DontGetData, null), Times.Exactly(6));
            _repo.Verify(x => x.GetRequestIDs(It.IsAny<IEnumerable<int>>()), Times.Once);
            _requestCachingService.Verify(x => x.GetRequestSummariesAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<bool>(), _cancellationToken), Times.Once);
            _cache.Verify(x => x.RefreshDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Func<DateTimeOffset, DateTimeOffset>>()), Times.Exactly(3));
        }

        [Test]
        public async Task RefreshCacheAsync_WhenInvoked_TriggersRequestCachingServiceCacheRefresh()
        {
            var jobId = 88;

            await _classUnderTest.RefreshCacheAsync(jobId, _cancellationToken);

            _cache.Verify(x => x.GetCachedDataAsync(It.IsAny<Func<CancellationToken, Task<int>>>(), "job-caching-service-job-88", RefreshBehaviour.DontWaitForFreshData, _cancellationToken, NotInCacheBehaviour.WaitForData, null), Times.Once);
            _requestCachingService.Verify(x => x.RefreshCacheAsync(8, _cancellationToken), Times.Once);
        }
    }
}
