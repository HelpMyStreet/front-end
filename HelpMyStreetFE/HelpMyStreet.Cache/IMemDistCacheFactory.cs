using System;

namespace HelpMyStreet.Cache
{
    public interface IMemDistCacheFactory<T>
    {
        /// <summary>
        /// Get cache
        /// </summary>
        /// <param name="howLongToKeepStaleData">How long to keep stale data in the cache</param>
        /// <param name="defaultWhenDataIsStaleDelegate">When the data should be considered stale (can be overriden when adding an item to the cache)</param> 
        /// <returns></returns>
        IMemDistCache<T> GetCache(TimeSpan howLongToKeepStaleData, Func<DateTimeOffset, DateTimeOffset> defaultWhenDataIsStaleDelegate);
    }
}