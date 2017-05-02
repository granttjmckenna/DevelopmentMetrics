using System.Linq;
using DevelopmentMetrics.Models;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class BuildMetricTests
    {
        private FakeRepository _fakeRepository;

        [SetUp]
        public void SetUp()
        {
            _fakeRepository = new FakeRepository();
        }

        [Test]
        public void Should_populate_build_metric()
        {
            var buildMetrics = new RootProject(_fakeRepository).GetBuildMetrics();

            var firstBuildMetric = buildMetrics.First();

            Assert.That(firstBuildMetric.ProjectId, Is.EqualTo("Admin"));
            Assert.That(firstBuildMetric.ProjectName, Is.EqualTo("Admin"));
            Assert.That(firstBuildMetric.BuildTypeId, Is.EqualTo("not-sure"));
        }
    }
}
