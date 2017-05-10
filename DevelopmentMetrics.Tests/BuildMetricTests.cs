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

            var failingRate = BuildCalculators.CalculateBuildFailureRate(buildMetrics);

            Assert.That(failingRate, Is.EqualTo(30));
        }

        [Test]
        public void Should_calculate_failure_percentage_for_project()
        {
            var buildMetrics = GetBuildMetricsData(10);

            buildMetrics[1].ProjectId = "Exclude from calculation";

            var failingRate = BuildCalculators.CalculateBuildFailureRate(buildMetrics,
                b => b.ProjectId.Equals("Blah", StringComparison.CurrentCultureIgnoreCase));

            Assert.That(failingRate, Is.EqualTo(33.33));
        }

        [Test]
        public void Should_calculate_failure_percentage_for_agent_name()
        {
            var buildMetrics = GetBuildMetricsData(10);

            buildMetrics[1].AgentName = "Exclude from calculation";

            var failingRate = BuildCalculators.CalculateBuildFailureRate(buildMetrics,
                b => b.AgentName.Equals("Blah", StringComparison.CurrentCultureIgnoreCase));

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

        [TestCase("2017-01-01")]
        [TestCase("2016-06-01")]
        [TestCase("2017-06-01")]
        public void Should_calculate_failure_percentage_by_month_for_one_year(string fromDate)
        {
            var buildMetrics = GetBuildMetricsData(360);

            var monthlyBuildMetrics = new BuildCalculators().CalculateBuildFailingRateByMonthFrom(DateTime.Parse(fromDate), buildMetrics);

            Assert.That(monthlyBuildMetrics.Keys.Count, Is.EqualTo(12));
        }

        [Test]
        public void Should_return_first_failing_buil_by_project()
        {
            var buildMetrics = GetBuildMetricsData(10);

            //add consecutive failing builds to dummy data with one for a separate project
            buildMetrics[1].Status = Helpers.BuildStatus.Failure.ToString();
            buildMetrics[2].Status = Helpers.BuildStatus.Failure.ToString();
            buildMetrics[3].Status = Helpers.BuildStatus.Failure.ToString();
            buildMetrics[3].ProjectId = "Different project id";

            var failingBuilds = BuildCalculators.GetFirstFailingBuildsByProject(buildMetrics);

            Assert.That(failingBuilds.Count, Is.EqualTo(4));
        }

        [Test]
        public void Should_return_failing_build_and_milliseconds_to_next_succeeding_build()
        {
            var buildMetrics = GetBuildMetricsData(10);

            var results = GetFailingBuildResults(buildMetrics);

            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.All(b => b.MillisecondsUntilBuildSucceeded > 0));
        }

        [Test]
        public void Should_return_average_in_milliseconds_between_failing_and_succeeding_builds_by_month()
        {
            var buildMetrics = GetBuildMetricsData(350);

            var results = GetAverageMillisecondsBetweenFailingAndSucceedingBuildsByMonthFrom(new DateTime(2017, 1, 1), buildMetrics);

            Assert.That(results.Keys.Count, Is.EqualTo(12));
        }

        private Dictionary<string, long> GetAverageMillisecondsBetweenFailingAndSucceedingBuildsByMonthFrom(DateTime fromDate, List<BuildMetric> buildMetrics)
        {
            var results = new Dictionary<string, long>();

            var failingBuildMetrics = GetFailingBuildResults(buildMetrics);

            for (var i = 0; i < 12; i++)
            {
                var whereDate = fromDate.AddMonths(i);

                var average =
                    failingBuildMetrics.Where(
                            b =>
                                b.FailingBuild.StartDateTime.Month.Equals(whereDate.Month) &&
                                b.FailingBuild.StartDateTime.Year.Equals(whereDate.Year))
                        .Average(b => b.MillisecondsUntilBuildSucceeded);

                results.Add(whereDate.ToString("MMM-yyyy"), (long)average);
            }

            return results;
        }

        private List<FailingBuildResults> GetFailingBuildResults(List<BuildMetric> buildMetrics)
        {
            return
                BuildCalculators.GetFirstFailingBuildsByProject(buildMetrics)
                    .Select(failingBuild => new FailingBuildResults
                    {
                        FailingBuild = failingBuild,
                        MillisecondsUntilBuildSucceeded = GetMillisecondsUntilBuildSucceeded(buildMetrics, failingBuild)
                    })
                    .ToList();
        }

        private double GetMillisecondsUntilBuildSucceeded(List<BuildMetric> buildMetrics, BuildMetric failingBuild)
        {
            var firstSucceedingBuild = buildMetrics
                .FirstOrDefault(b => b.Status.Equals("Success", StringComparison.InvariantCultureIgnoreCase)
                                     &&
                                     b.ProjectId.Equals(failingBuild.ProjectId,
                                         StringComparison.InvariantCultureIgnoreCase)
                                     && b.BuildId > failingBuild.BuildId);

            return firstSucceedingBuild?.FinishDateTime.Subtract(failingBuild.FinishDateTime).TotalMilliseconds ?? DateTime.Now.Subtract(failingBuild.FinishDateTime).TotalMilliseconds;
        }

        private List<BuildMetric> GetBuildMetricsData(int rows)
        {
            var dummyBuildMetrics = new List<BuildMetric>();

            for (var i = 1; i <= rows; i++)
            {
                var baseDateTime = new DateTime(2017, 1, 1, 12, 0, 0);

                dummyBuildMetrics.Add(
                    new BuildMetric
                    {
                        ProjectId = "Blah",
                        ProjectName = "Blah",
                        BuildTypeId = "Blah_blah",
                        BuildId = i,
                        AgentName = "Blah",
                        StartDateTime = baseDateTime.AddDays(i).AddMinutes(1),
                        FinishDateTime = baseDateTime.AddDays(i).AddMinutes(2),
                        QueueDateTime = baseDateTime.AddDays(i),
                        State = "Finished",
                        Status = GetStatus(i)
                    }
                );
            }

            return dummyBuildMetrics;
        }

        private string GetStatus(int i)
        {
            return ((i % 3) == 0) ? Helpers.BuildStatus.Failure.ToString() : Helpers.BuildStatus.Success.ToString();
        }
    }

    internal class FailingBuildResults
    {
        public BuildMetric FailingBuild { get; set; }

        public double MillisecondsUntilBuildSucceeded { get; set; }
    }
}
