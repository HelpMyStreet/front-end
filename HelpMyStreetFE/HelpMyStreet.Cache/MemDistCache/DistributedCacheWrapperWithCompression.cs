using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Caching;
using Polly.Retry;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json.Resolvers;

namespace HelpMyStreet.Cache.MemDistCache
{
    public class DistributedCacheWrapperWithCompression : IDistributedCacheWrapper
    {
        private readonly IDistributedCache _distributedCache;

        private AsyncRetryPolicy _defaultRetryPolicy;

        public DistributedCacheWrapperWithCompression(IDistributedCache distributedCache, RetryPolicy retryPolicy = null)
        {
            _distributedCache = distributedCache;

            if (retryPolicy == null)
            {
                _defaultRetryPolicy = Policy.Handle<RedisException>().RetryAsync(2);
            }
        }

        public async Task<(bool, object)> TryGetAsync<T>(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            byte[] result = null;

            await _defaultRetryPolicy.ExecuteAsync(async () =>
            {
                result = await _distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            });


            if (result == null)
            {
                return (false, null);
            }

            if (CompressionUtils.IsGZipped(result))
            {
                result = CompressionUtils.UnGZipToBytes(result);
            }

            T deserialisedResult = Utf8Json.JsonSerializer.Deserialize<T>(result);

            return (true, deserialisedResult);
        }

        public async Task PutAsync(string key, object value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            byte[] result = Utf8Json.JsonSerializer.Serialize(value, StandardResolver.AllowPrivate);

            result = CompressionUtils.Gzip(result);

            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = ttl.Timespan
            };

            await _defaultRetryPolicy.ExecuteAsync(async () =>
            {
                await _distributedCache.SetAsync(key, result, distributedCacheEntryOptions, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            });
        }
    }
}