using System.Runtime.Caching;

namespace DevelopmentMetrics.Helpers
{
    public interface ICacheChecker
    {
        bool IsDataCachedFor(string itemName);
    }

    public class CacheChecker : ICacheChecker
    {
        public bool IsDataCachedFor(string itemName)
        {
            var cache = MemoryCache.Default;

            return cache.Contains(itemName);
        }
    }
}