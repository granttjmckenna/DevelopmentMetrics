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

            var failingRate = Math.Round((double) (100 * failing) / total, 2);

            return failingRate;
        }
    }
}
