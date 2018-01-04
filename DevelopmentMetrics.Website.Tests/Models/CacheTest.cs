using DevelopmentMetrics.Website.Models;
using NUnit.Framework;

namespace DevelopmentMetrics.Website.Tests.Models
{
    [TestFixture]
    public class CacheTest
    {
        [Test]
        public void Return_false_when_data_NOT_cached_for_cache_key()
        {
            var result = new Cache().IsDataCachedFor("cache key");

            Assert.False(result);
        }
    }
}
