using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds.Metrics
{
    public class BuildStabilityMetric : IBuildMetric
    {
        private List<double> Intervals { get; } = new List<double>();
        private List<Build> Builds { get; } = new List<Build>();
        private List<BuildStabilityMetric> Results { get; } = new List<BuildStabilityMetric>();

        public DateTime Date { get; private set; }
        public double FailureRate { get; private set; }
        public int RecoveryTime { get; private set; }
        public double RecoveryTimeStdDev { get; private set; }
        public int IgnoredTestCount { get; private set; }

        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public void Add(IBuild build, List<Build> builds)
        {
            Intervals.AddRange(CalculateMillisecondsBetweenBuilds(builds));
            Builds.AddRange(builds);
        }

        private List<double> CalculateMillisecondsBetweenBuilds(List<Build> builds)
        {
            var alternatingBuilds = GetAlternatingBuilds(builds);

            return CalculateMillisecondsBetweenAlternatingBuilds(alternatingBuilds);
        }

        public void Calculate()
        {
            Results.Add(new BuildStabilityMetric
            {
                Date = Date,
                FailureRate = CalculateFailureRateFor(Builds),
                RecoveryTime = CalculateAverageRecoveryTimeInHoursFor(Intervals),
                RecoveryTimeStdDev = Calculator.ConvertMillisecondsToHours(
                    Calculator.CalculateStandardDeviation(Intervals)),
                IgnoredTestCount = Builds.Sum(build => build.IgnoredTestCount)
            });

            Intervals.Clear();
            Builds.Clear();
        }

        public List<BuildStabilityMetric> GetResults()
        {
            return Results;
        }

        public List<BuildFailureRate> GetFailingBuildsByRate(List<Build> builds)
        {
            return GetFailureRatesFor(builds)
                .Where(b => b.Rate > 0.5d)
                .OrderByDescending(b => b.Rate)
                .ThenBy(b => b.BuildTypeId)
                .ToList();
        }

        public List<BuildFailureRate> GetPassingBuildsByRate(List<Build> builds)
        {
            return GetFailureRatesFor(builds)
                .Where(b => b.Rate <= 0.5d)
                .OrderByDescending(b => b.Rate)
                .ThenBy(b => b.BuildTypeId)
                .ToList();
        }

        private IEnumerable<BuildFailureRate> GetFailureRatesFor(List<Build> builds)
        {
            return from buildTypeId in builds
                    .Select(b => b.BuildTypeId)
                    .Distinct()
                   let selectedBuilds = builds
                       .Where(
                           b => b.BuildTypeId.Equals(buildTypeId, StringComparison.InvariantCultureIgnoreCase)
                                && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                       .ToList()
                   select new BuildFailureRate
                   {
                       BuildTypeId = buildTypeId,
                       Rate = CalculateBuildFailingRate(selectedBuilds)
                   };
        }

        private double CalculateBuildFailingRate(List<Build> builds)
        {
            return CalculateFailureRateFor(builds);
        }

        private double CalculateFailureRateFor(List<Build> builds)
        {
            return Calculator.Percentage(GetFailureCountFor(builds), builds.Count);
        }

        private static int GetFailureCountFor(List<Build> builds)
        {
            return builds.Count(b =>
                b.Status.Equals(BuildStatus.Failure.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        private List<Build> GetAlternatingBuilds(List<Build> builds)
        {
            var orderedBuilds = builds.OrderBy(b => b.Id).ToList();

            var results = new List<Build>();

            var isPreviousBuildSuccess = true;

            foreach (var build in orderedBuilds)
            {
                if (build.Status.Equals(BuildStatus.Failure.ToString(), StringComparison.InvariantCultureIgnoreCase) && isPreviousBuildSuccess)
                {
                    results.Add(build);
                    isPreviousBuildSuccess = false;
                }
                else if (build.Status.Equals(BuildStatus.Success.ToString(), StringComparison.InvariantCultureIgnoreCase) && !isPreviousBuildSuccess)
                {
                    results.Add(build);
                    isPreviousBuildSuccess = true;
                }
            }

            return results;
        }

        private List<double> CalculateMillisecondsBetweenAlternatingBuilds(List<Build> builds)
        {
            var doubles = new List<double>();

            for (var x = 0; x < builds.Count - 1; x += 2)
            {
                doubles.Add((builds[x + 1].FinishDateTime - builds[x].StartDateTime).TotalMilliseconds);
            }

            return doubles;
        }

        private int CalculateAverageRecoveryTimeInHoursFor(List<double> doubles)
        {
            return doubles.Any()
                ? Calculator.ConvertMillisecondsToHours(doubles.Average())
                : 0;
        }
    }
}
