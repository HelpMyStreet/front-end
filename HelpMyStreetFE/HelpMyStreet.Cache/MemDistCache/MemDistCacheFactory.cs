using Microsoft.Extensions.Internal;
using Polly.Caching;
using System;
using HelpMyStreet.Utils.Utils;

namespace HelpMyStreet.Cache.MemDistCache
{
    public class MemDistCacheFactory<T> : IMemDistCacheFactory<T>
    {
        private readonly ISyncCacheProvider _pollySyncCacheProvider;
        private readonly IDistributedCacheWrapper _distributedCacheWrapper;
        private readonly ISystemClock _mockableDateTime;
        private readonly ILoggerWrapper<MemDistCache<T>> _logger;

        public MemDistCacheFactory(ISyncCacheProvider pollySyncCacheProvider, IDistributedCacheWrapper distributedCacheWrapper, ISystemClock mockableDateTime, ILoggerWrapper<MemDistCache<T>> logger)
        {
            _pollySyncCacheProvider = pollySyncCacheProvider;
            _distributedCacheWrapper = distributedCacheWrapper;
            _mockableDateTime = mockableDateTime;
            _logger = logger;
        }

        /// <inheritdoc />>
        public IMemDistCache<T> GetCache(TimeSpan howLongToKeepStaleData, Func<DateTimeOffset, DateTimeOffset> whenDataIsStaleDelegate)
        {
            MemDistCache<T> memDistCache = new MemDistCache<T>(_pollySyncCacheProvider, _distributedCacheWrapper, _mockableDateTime, howLongToKeepStaleData, whenDataIsStaleDelegate, _logger);
            return memDistCache;
        }
    }
}

