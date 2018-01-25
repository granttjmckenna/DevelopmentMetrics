using System.Collections.Generic;
using DevelopmentMetrics.Builds;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class BuildTypeTests
    {
        [Test]
        public void Return_distinct_list_of_build_types()
        {
            var builds = GetBuilds();

            var list = new BuildType().GetDistinctBuildTypeIds(builds);

            Assert.That(list.Count, Is.EqualTo(1));
        }

        private List<Build> GetBuilds()
        {
            return new List<Build>
            {
                new Build
                {
                    BuildTypeId = "EHLInsight_10Test"
                }
            };
        }
    }
}
