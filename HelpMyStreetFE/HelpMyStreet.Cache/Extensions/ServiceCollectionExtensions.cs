using HelpMyStreet.Cache.MemCache;
using HelpMyStreet.Cache.MemDistCache;
using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace HelpMyStreet.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add MemDistCache
        /// </summary>
        public static IServiceCollection AddMemDistCache(this IServiceCollection serviceCollection, string applicationName, string redisConnectionString)
        {
            serviceCollection.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = applicationName;
            });

            serviceCollection.TryAddSingleton<IDistributedCacheWrapper, DistributedCacheWrapperWithCompression>();
            serviceCollection.TryAddSingleton(typeof(IMemDistCacheFactory<>), typeof(MemDistCacheFactory<>));
            serviceCollection.AddShared();

            return serviceCollection;
        }

        /// <summary>
        /// Add in-memory implementation of MemDistCache
        /// </summary>
        public static IServiceCollection AddMemCache(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<Polly.Caching.ISyncCacheProvider, Polly.Caching.Memory.MemoryCacheProvider>();
            serviceCollection.TryAddSingleton(typeof(IMemDistCacheFactory<>), typeof(MemCacheFactory<>));
            serviceCollection.AddShared();

            return serviceCollection;
        }

        private static IServiceCollection AddShared(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMemoryCache();
            serviceCollection.AddLogging();
            serviceCollection.TryAddSingleton<Polly.Caching.ISyncCacheProvider, Polly.Caching.Memory.MemoryCacheProvider>();
            serviceCollection.TryAddSingleton<ISystemClock, MockableDateTime>();
            serviceCollection.TryAddSingleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>));

            return serviceCollection;
        }
    }
}
