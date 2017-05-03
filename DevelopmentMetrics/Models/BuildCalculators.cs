using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Models
{
    public class BuildCalculators
    {
        public double CalculateBuildFailingRate(List<BuildMetric> buildMetrics)
        {
            return CalculateFailingRate(buildMetrics);
        }

        public Dictionary<string, double> CalculateProjectBuildFailingRate(List<BuildMetric> buildMetrics)
        {
            var projectBuildMetrics = new Dictionary<string, double>();

            foreach (var buildMetric in buildMetrics)
            {
                if (projectBuildMetrics.ContainsKey(buildMetric.ProjectId))
                    continue;

                var failureRate = new BuildCalculators().CalculateProjectBuildFailingRateFor(buildMetrics,
                    buildMetric.ProjectId);

                projectBuildMetrics.Add(buildMetric.ProjectId, failureRate);
            }

            return projectBuildMetrics;
        }

        public double CalculateProjectBuildFailingRateFor(List<BuildMetric> buildMetrics, string projectId)
        {
            var projectBuildMetrics = buildMetrics
                .Where(b => b.ProjectId.Equals(projectId, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            return CalculateFailingRate(projectBuildMetrics);
        }

        public double CalculateAgentBuildFailingRateFor(List<BuildMetric> buildMetrics, string agentName)
        {
            var projectBuildMetrics = buildMetrics
                .Where(b => b.AgentName.Equals(agentName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            return CalculateFailingRate(projectBuildMetrics);
        }

        private static double CalculateFailingRate(IReadOnlyCollection<BuildMetric> buildMetrics)
        {
            var total = buildMetrics.Count;

            var failing =
                buildMetrics.Count(
                    b =>
                        b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase) &&
                        b.Status.Equals("Failure", StringComparison.CurrentCultureIgnoreCase));

            var failingRate = CalculateFailingRate(failing, total);

            return failingRate;
        }

        public Dictionary<string, double> CalculateBuildFailingRateByMonthFrom(DateTime fromDate, List<BuildMetric> buildMetrics)
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

                var failing =
                    monthBuildMetrics.Count(b => b.Status.Equals("Failure", StringComparison.CurrentCultureIgnoreCase));

                var failingRate = CalculateFailingRate(failing, total);

                results.Add(queryDate.ToString("MMM-yyyy"), failingRate);
            }

            return results;
        }

        private static double CalculateFailingRate(int failing, int total)
        {
            return (total == 0)
                ? 0
                : Math.Round((double)(100 * failing) / total, 2);
        }
    }
}
