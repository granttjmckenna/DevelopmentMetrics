using System.Threading;
using DevelopmentMetrics.Builds;
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
        private IBuild _build;

        [SetUp]
        public void Setup()
        {
            _cacheChecker = Substitute.For<ICacheChecker>();
            _build = Substitute.For<IBuild>();

            _cache = new Cache(_cacheChecker, _build);
        }

        [Test]
        public void Return_true_when_data_cached_for_cache_key()
        {
            _cacheChecker.IsDataCachedFor(Arg.Any<string>()).Returns(true);

            var result = _cache.IsBuildDataCached();

            Assert.True(result);
        }
    }
}
