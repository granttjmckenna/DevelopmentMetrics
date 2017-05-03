using System;
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
            Assert.That(firstBuildMetric.BuildTypeId, Is.EqualTo("Consumer_Funnel_31ProductionSmokeTests"));
            Assert.That(firstBuildMetric.BuildId, Is.GreaterThan(0));
            Assert.That(firstBuildMetric.StartDateTime, Does.Not.EqualTo(new DateTime(0001, 01, 01, 00, 00, 00)));
            Assert.That(firstBuildMetric.FinishDateTime, Does.Not.EqualTo(new DateTime(0001, 01, 01, 00, 00, 00)));
            Assert.That(firstBuildMetric.QueueDateTime, Does.Not.EqualTo(new DateTime(0001, 01, 01, 00, 00, 00)));
            Assert.That(firstBuildMetric.State.Equals("Finished", StringComparison.CurrentCultureIgnoreCase));
            Assert.That(firstBuildMetric.Status.Equals("Success", StringComparison.CurrentCultureIgnoreCase));
            Assert.That(!string.IsNullOrWhiteSpace(firstBuildMetric.AgentName));
        }
    }
}
