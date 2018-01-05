using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Website.Tests.Models
{
    [TestFixture]
    public class CacheTest
    {
        private Cache _cache;
        private ICacheChecker _cacheChecker;

        [SetUp]
        public void Setup()
        {
            _cacheChecker = Substitute.For<ICacheChecker>();
            var build = Substitute.For<IBuild>();
            var card = Substitute.For<ICard>();

            _cache = new Cache(_cacheChecker, build, card);
        }

        [Test]
        public void Return_true_when_build_data_cached_for_cache_key()
        {
            _cacheChecker.IsDataCachedFor(Arg.Any<string>()).Returns(true);

            var result = _cache.IsBuildDataCached();

            Assert.True(result);
        }

        [Test]
        public void Return_true_when_card_data_cached_for_cache_key()
        {
            _cacheChecker.IsDataCachedFor(Arg.Any<string>()).Returns(true);

            var result = _cache.IsCardDataCached();

            Assert.True(result);
        }
    }
}
