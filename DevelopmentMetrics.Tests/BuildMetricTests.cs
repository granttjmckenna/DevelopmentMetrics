using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class BuildMetricTests
    {




        //[Test]
        //public void Should_return_first_failing_build_by_project()
        //{
        //    var buildMetrics = GetBuildMetricsData(10);

        //    //add consecutive failing builds to dummy data with one for a separate project
        //    buildMetrics[1].Status = Helpers.BuildStatus.Failure.ToString();
        //    buildMetrics[2].Status = Helpers.BuildStatus.Failure.ToString();
        //    buildMetrics[3].Status = Helpers.BuildStatus.Failure.ToString();
        //    buildMetrics[3].ProjectId = "Different project id";

        //    var failingBuilds = BuildCalculators.GetFirstFailingBuildsByProject(buildMetrics);

        //    Assert.That(failingBuilds.Count, Is.EqualTo(4));
        //}

        //[Test]
        //public void Should_return_failing_build_and_milliseconds_to_next_succeeding_build()
        //{
        //    var buildMetrics = GetBuildMetricsData(10);

        //    var results = GetFailingBuildResults(buildMetrics);

        //    Assert.That(results.Count, Is.EqualTo(3));
        //    Assert.That(results.All(b => b.MillisecondsUntilBuildSucceeded > 0));
        //}

        //[Test]
        //public void Should_return_average_in_milliseconds_between_failing_and_succeeding_builds_by_month()
        //{
        //    var buildMetrics = GetBuildMetricsData(350);

        //    var results = GetAverageMillisecondsBetweenFailingAndSucceedingBuildsByMonthFrom(new DateTime(2017, 1, 1), buildMetrics);

        //    Assert.That(results.Keys.Count, Is.EqualTo(12));
        //}

        //[Test]
        //public void Should_return_average_in_hours_between_failing_and_succeeding_builds_by_month()
        //{
        //    var buildMetrics = GetBuildMetricsData(350);

        //    var results = GetAverageHoursBetweenFailingAndSucceedingBuildsByMonthFrom(new DateTime(2017, 1, 1),
        //        buildMetrics);

        //    Assert.That(results.Keys.Count, Is.EqualTo(12));
        //}

        //private Dictionary<string, double> GetAverageHoursBetweenFailingAndSucceedingBuildsByMonthFrom(DateTime fromDate, List<BuildMetric> buildMetrics)
        //{
        //    var failingBuildsInMilliseconds =
        //        GetAverageMillisecondsBetweenFailingAndSucceedingBuildsByMonthFrom(fromDate, buildMetrics);

        //    var millisecondsPerHour = 3600000;

        //    return
        //        failingBuildsInMilliseconds.ToDictionary<KeyValuePair<string, long>, string, double>(
        //            failingBuild => failingBuild.Key, failingBuild => failingBuild.Value / millisecondsPerHour);
        //}


        //private Dictionary<string, double> GetStandardDeviationBetweenFailingAndSucceedingBuildsByMonthFrom(
        //    DateTime fromDate, List<BuildMetric> buildMetrics)
        //{
        //    var results = new Dictionary<string, double>();

        //    var failingBuildMetrics = GetFailingBuildResults(buildMetrics);

        //    for (var i = 0; i < 12; i++)
        //    {
        //        var whereDate = fromDate.AddMonths(i);

        //        var values = failingBuildMetrics.Where(
        //                b =>
        //                    b.FailingBuild.StartDateTime.Month.Equals(whereDate.Month) &&
        //                    b.FailingBuild.StartDateTime.Year.Equals(whereDate.Year))
        //            .Select(b => b.MillisecondsUntilBuildSucceeded)
        //            .ToList();

        //        var standardDeviation = CalculateStandardDeviation(values);

        //        results.Add(whereDate.ToString("MMM-yyyy"), standardDeviation);
        //    }

        //    return results;
        //}

        //private double CalculateStandardDeviation(List<double> values)
        //{
        //    if (!values.Any())
        //        return 0;

        //    var average = values.Average();

        //    var sumOf = values.Sum(d => Math.Pow(d - average, 2));

        //    return Math.Sqrt(sumOf / (values.Count - 1));
        //}

        //private Dictionary<string, long> GetAverageMillisecondsBetweenFailingAndSucceedingBuildsByMonthFrom(DateTime fromDate, List<BuildMetric> buildMetrics)
        //{
        //    var results = new Dictionary<string, long>();

        //    var failingBuildMetrics = GetFailingBuildResults(buildMetrics);

        //    for (var i = 0; i < 12; i++)
        //    {
        //        var whereDate = fromDate.AddMonths(i);

        //        var average =
        //            failingBuildMetrics.Where(
        //                    b =>
        //                        b.FailingBuild.StartDateTime.Month.Equals(whereDate.Month) &&
        //                        b.FailingBuild.StartDateTime.Year.Equals(whereDate.Year))
        //                .Average(b => b.MillisecondsUntilBuildSucceeded);

        //        results.Add(whereDate.ToString("MMM-yyyy"), (long)average);
        //    }

        //    return results;
        //}

        //private List<FailingBuildResults> GetFailingBuildResults(List<BuildMetric> buildMetrics)
        //{
        //    return
        //        BuildCalculators.GetFirstFailingBuildsByProject(buildMetrics)
        //            .Select(failingBuild => new FailingBuildResults
        //            {
        //                FailingBuild = failingBuild,
        //                MillisecondsUntilBuildSucceeded = GetMillisecondsUntilBuildSucceeded(buildMetrics, failingBuild)
        //            })
        //            .ToList();
        //}

        //private double GetMillisecondsUntilBuildSucceeded(List<BuildMetric> buildMetrics, BuildMetric failingBuild)
        //{
        //    var firstSucceedingBuild = buildMetrics
        //        .FirstOrDefault(b => b.Status.Equals("Success", StringComparison.InvariantCultureIgnoreCase)
        //                             &&
        //                             b.ProjectId.Equals(failingBuild.ProjectId,
        //                                 StringComparison.InvariantCultureIgnoreCase)
        //                             && b.BuildId > failingBuild.BuildId);

        //    return firstSucceedingBuild?.FinishDateTime.Subtract(failingBuild.FinishDateTime).TotalMilliseconds ?? DateTime.Now.Subtract(failingBuild.FinishDateTime).TotalMilliseconds;
        //}

        //private List<BuildMetric> GetBuildMetricsData(int rows)
        //{
        //    var dummyBuildMetrics = new List<BuildMetric>();

        //    for (var i = 1; i <= rows; i++)
        //    {
        //        var baseDateTime = new DateTime(2017, 1, 1, 12, 0, 0);

        //        dummyBuildMetrics.Add(
        //            new BuildMetric
        //            {
        //                ProjectId = "Blah",
        //                ProjectName = "Blah",
        //                BuildTypeId = "Blah_blah",
        //                BuildId = i,
        //                AgentName = "Blah",
        //                StartDateTime = baseDateTime.AddDays(i).AddMinutes(1),
        //                FinishDateTime = baseDateTime.AddDays(i).AddMinutes(2),
        //                QueueDateTime = baseDateTime.AddDays(i),
        //                State = "Finished",
        //                Status = GetStatus(i)
        //            }
        //        );
        //    }

        //    return dummyBuildMetrics;
        //}

        //private string GetStatus(int i)
        //{
        //    return ((i % 3) == 0) ? Helpers.BuildStatus.Failure.ToString() : Helpers.BuildStatus.Success.ToString();
        //}
    }

    internal class FailingBuildResults
    {
        public BuildMetric FailingBuild { get; set; }

        public double MillisecondsUntilBuildSucceeded { get; set; }
    }
}
