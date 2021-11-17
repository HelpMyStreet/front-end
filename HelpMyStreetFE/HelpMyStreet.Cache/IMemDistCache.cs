using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreet.Cache
{
    public interface IMemDistCache<T>
    {
        /// <summary>
        /// Get data from cache. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that returns data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="refreshBehaviour">Action to take when data is stale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="notInCacheBehaviour">Action to take when data is not in cache</param>
        /// <param name="whenDataIsStaleDelegate">When the data should be considered stale</param> 
        /// <returns></returns>
        Task<T> GetCachedDataAsync(Func<CancellationToken, Task<T>> dataGetter,
                                   string key,
                                   RefreshBehaviour refreshBehaviour,
                                   CancellationToken cancellationToken,
                                   NotInCacheBehaviour notInCacheBehaviour = NotInCacheBehaviour.WaitForData,
                                   Func<DateTimeOffset, DateTimeOffset> whenDataIsStaleDelegate = null);


        /// <summary>
        /// Get data from cache. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that returns data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="refreshBehaviour">Action to take when data is stale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="notInCacheBehaviour">Action to take when data is not in cache</param>
        /// <param name="whenDataIsStaleDelegate">When the data should be considered stale</param> 
        /// <returns></returns>
        T GetCachedData(Func<CancellationToken, T> dataGetter,
                        string key,
                        RefreshBehaviour refreshBehaviour,
                        CancellationToken cancellationToken,
                        NotInCacheBehaviour notInCacheBehaviour = NotInCacheBehaviour.WaitForData,
                        Func<DateTimeOffset, DateTimeOffset> whenDataIsStaleDelegate = null);


        /// <summary>
        /// Refresh data in cache. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that returns data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="whenDataIsStaleDelegate">When the data should be considered stale</param> 
        /// <returns></returns>
        Task<T> RefreshDataAsync(Func<CancellationToken, Task<T>> dataGetter,
                                 string key,
                                 CancellationToken cancellationToken,
                                 Func<DateTimeOffset, DateTimeOffset> whenDataIsStaleDelegate = null);
    }
}