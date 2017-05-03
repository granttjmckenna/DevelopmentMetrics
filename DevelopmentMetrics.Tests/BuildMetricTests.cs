using System;
using System.Collections.Generic;
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

        [Test]
        public void Should_calculate_failure_percentage()
        {
            var buildMetrics = GetBuildMetricsData(10);

            var failingRate = new BuildCalculators().CalculateBuildFailingRate(buildMetrics);

            Assert.That(failingRate, Is.EqualTo(30));
        }

        [Test]
        public void Should_calculate_failure_percentage_for_project()
        {
            var buildMetrics = GetBuildMetricsData(10);

            buildMetrics[1].ProjectId = "Exclude from calculation";

            var failingRate = new BuildCalculators().CalculateProjectBuildFailingRateFor(buildMetrics, "Blah");

            Assert.That(failingRate, Is.EqualTo(33.33));
        }

        [Test]
        public void Should_calculate_failure_percentage_for_agent_name()
        {
            var buildMetrics = GetBuildMetricsData(10);

            buildMetrics[1].AgentName = "Exclude from calculation";

            var failingRate = new BuildCalculators().CalculateAgentBuildFailingRateFor(buildMetrics, "Blah");

            Assert.That(failingRate, Is.EqualTo(33.33));
        }

        [Test]
        public void Should_calculate_failure_percentage_by_project()
        {
            var buildMetrics = GetBuildMetricsData(10);

            buildMetrics[1].ProjectId = "Different project id";

            var projectBuildMetrics = new BuildCalculators().CalculateProjectBuildFailingRate(buildMetrics);

            Assert.That(projectBuildMetrics.Keys.Count, Is.EqualTo(2));
            Assert.That(projectBuildMetrics.First(c => c.Key.Equals("Blah")).Value, Is.EqualTo(33.33));
            Assert.That(projectBuildMetrics.First(c => c.Key.Equals("Different project id")).Value, Is.EqualTo(0));
        }

        private List<BuildMetric> GetBuildMetricsData(int rows)
        {
            var dummyBuildMetrics = new List<BuildMetric>();

            for (var i = 1; i <= rows; i++)
            {
                dummyBuildMetrics.Add(
                    new BuildMetric
                    {
                        ProjectId = "Blah",
                        ProjectName = "Blah",
                        BuildTypeId = "Blah_blah",
                        BuildId = i,
                        AgentName = "Blah",
                        StartDateTime = DateTime.Now,
                        FinishDateTime = DateTime.Now,
                        QueueDateTime = DateTime.Now,
                        State = "Finished",
                        Status = GetStatus(i)
                    }
                );
            }

            return dummyBuildMetrics;
        }

        private string GetStatus(int i)
        {
            return ((i % 3) == 0) ? "Failure" : "Success";
        }
    }
}
