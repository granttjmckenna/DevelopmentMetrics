using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class Cache
    {
        private readonly ICacheChecker _cacheChecker;
        private readonly IBuild _build;
        private readonly ICard _card;

        public Cache(ICacheChecker cacheChecker, IBuild build, ICard card)
        {
            _cacheChecker = cacheChecker;
            _build = build;
            _card = card;
        }

        public bool IsBuildDataCached()
        {
            CacheBuildData();

            return IsDataCachedFor(Build.CacheKey);
        }

        public bool IsCardDataCached()
        {
            CacheCardData();

            return IsDataCachedFor(Card.CacheKey);
        }

        private void CacheCardData()
        {
            if (!_cacheChecker.IsDataCachedFor(Card.CacheKey))
            {
                _card.GetCards();
            }
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