using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Models
{
    public class BuildCalculators
    {
        public static double CalculateBuildFailureRate(List<BuildMetric> buildMetrics)
        {
            Func<BuildMetric, bool> predicate =
                b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase);

            return CalculateBuildFailureRate(buildMetrics, predicate);
        }

        public static double CalculateBuildFailureRate(List<BuildMetric> buildMetrics, Func<BuildMetric, bool> predicate)
        {
            var filteredBuildMetrics = buildMetrics.Where(predicate);

            return CalculateFailingRate(filteredBuildMetrics);
        }

        public Dictionary<string, double> CalculateProjectBuildFailingRate(List<BuildMetric> buildMetrics)
        {
            var projectBuildMetrics = new Dictionary<string, double>();

            foreach (var buildMetric in buildMetrics)
            {
                if (projectBuildMetrics.ContainsKey(buildMetric.ProjectId))
                    continue;

                var failureRate = CalculateBuildFailureRate(buildMetrics,
                    b => b.ProjectId.Equals(buildMetric.ProjectId, StringComparison.InvariantCultureIgnoreCase));

                projectBuildMetrics.Add(buildMetric.ProjectId, failureRate);
            }

            return projectBuildMetrics;
        }

        private static double CalculateFailingRate(IEnumerable<BuildMetric> buildMetrics)
        {
            var metrics = buildMetrics
                .Where(b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var total = metrics.Count;

            var failing = metrics.Count(b => b.Status.Equals(Helpers.BuildStatus.Failure.ToString(), StringComparison.CurrentCultureIgnoreCase));

            var failingRate = CalculateFailingRate(failing, total);

            return failingRate;
        }

        public Dictionary<string, double> CalculateBuildFailingRateByMonthFrom(DateTime fromDate,
            List<BuildMetric> buildMetrics)
        {
            var results = new Dictionary<string, double>();

            for (var i = 0; i < 12; i++)
            {
                var queryDate = fromDate.AddMonths(i);

                var monthBuildMetrics =
                    buildMetrics
                        .Where(b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)
                                    && b.StartDateTime.Month.Equals(queryDate.Month)
                                    && b.StartDateTime.Year.Equals(queryDate.Year))
                        .ToList();

                var total = monthBuildMetrics.Count();

                if (total == 0)
                    continue;

                var failing =
                    monthBuildMetrics.Count(b => b.Status.Equals(Helpers.BuildStatus.Failure.ToString(), StringComparison.CurrentCultureIgnoreCase));

                var failingRate = CalculateFailingRate(failing, total);

                results.Add(queryDate.ToString("MMM-yyyy"), failingRate);
            }

            return results;
        }

        public Dictionary<string, double> CalculateBuildFailingRateByMonth(List<BuildMetric> buildMetrics)
        {
            var firstBuildStart =
                buildMetrics
                    .OrderBy(b => b.StartDateTime)
                    .First(b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                    .StartDateTime;

            return CalculateBuildFailingRateByMonthFrom(firstBuildStart, buildMetrics);
        }

        private static double CalculateFailingRate(int failing, int total)
        {
            return (total == 0)
                ? 0
                : Math.Round((double)(100 * failing) / total, 2);
        }

        public static List<BuildMetric> GetFirstFailingBuildsByProject(List<BuildMetric> buildMetrics)
        {
            var failingBuilds =
                buildMetrics.Where(b => b.Status.Equals(Helpers.BuildStatus.Failure.ToString(), StringComparison.InvariantCultureIgnoreCase));

            return failingBuilds
                .Where(failingBuild => PreviousBuildSucceeded(buildMetrics, failingBuild))
                .ToList();
        }

        private static bool PreviousBuildSucceeded(List<BuildMetric> buildMetrics, BuildMetric failingBuild)
        {
            var previousBuild =
                buildMetrics.LastOrDefault(
                    b => b.ProjectId.Equals(failingBuild.ProjectId, StringComparison.InvariantCultureIgnoreCase)
                         && b.BuildId < failingBuild.BuildId);

            return previousBuild == null ||
                   previousBuild.Status.Equals(Helpers.BuildStatus.Success.ToString(),
                       StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
