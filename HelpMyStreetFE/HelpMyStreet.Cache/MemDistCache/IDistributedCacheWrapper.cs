using System.Threading;
using System.Threading.Tasks;
using Polly.Caching;

namespace HelpMyStreet.Cache.MemDistCache
{
    public interface IDistributedCacheWrapper
    {
        Task<(bool, object)> TryGetAsync<T>(string key, CancellationToken cancellationToken, bool continueOnCapturedContext);
        Task PutAsync(string key, object value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext);
    }
}