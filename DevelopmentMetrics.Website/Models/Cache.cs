using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class Cache
    {
        private readonly ICacheChecker _cacheChecker;

        public Cache(ICacheChecker cacheChecker)
        {
            _cacheChecker = cacheChecker;
        }

        public bool IsDataCachedFor(string cacheKey)
        {
            var isCached = _cacheChecker.IsDataCachedFor(cacheKey);


            return isCached;
        }
    }
}