using System;
using System.Runtime.Caching;

namespace DevelopmentMetrics.Helpers
{
    public static class CacheHelper
    {
        public static T GetObjectFromCache<T>(string itemName, int minutesToCache, Func<T> objectSettingFunction)
        {
            var cache = MemoryCache.Default;

            var cachedObject = (T)cache[itemName];

            if (cachedObject != null)
                return cachedObject;

            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(minutesToCache)
            };

            cachedObject = objectSettingFunction();

            cache.Set(itemName, cachedObject, cacheItemPolicy);

            return cachedObject;
        }

        public static void ClearObjectFromCache(string itemName)
        {
            var cache = MemoryCache.Default;

            cache.Remove(itemName);
        }
    }
}
