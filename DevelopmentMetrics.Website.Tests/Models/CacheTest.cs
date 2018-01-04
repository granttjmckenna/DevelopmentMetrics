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

            _cache = new Cache(_cacheChecker);
        }

        [Test]
        public void Return_false_when_data_NOT_cached_for_cache_key()
        {
            var result = _cache.IsDataCachedFor("cache key");

            Assert.False(result);
        }

        [Test]
        public void Return_true_when_data_cached_for_cache_key()
        {
            _cacheChecker.IsDataCachedFor(Arg.Any<string>()).Returns(true);

            var result = _cache.IsDataCachedFor("cache key");

            Assert.True(result);
        }
    }
}
