using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class Cache
    {
        private readonly ICacheChecker _cacheChecker;
        private readonly IBuild _build;

        public Cache(ICacheChecker cacheChecker, IBuild build)
        {
            _cacheChecker = cacheChecker;
            _build = build;
        }

        public bool IsBuildDataCached()
        {
            CacheBuildData();

            return IsDataCachedFor(Build.CacheKey);
        }

        private void CacheBuildData()
        {
            if (!_cacheChecker.IsDataCachedFor(Build.CacheKey))
            {
                _build.GetBuilds();
            }
        }


        private bool IsDataCachedFor(string cacheKey)
        {
            var isCached = _cacheChecker.IsDataCachedFor(cacheKey);

            while (!isCached)
            {
                isCached = _cacheChecker.IsDataCachedFor(cacheKey);
            }

            return true;
        }
    }
}