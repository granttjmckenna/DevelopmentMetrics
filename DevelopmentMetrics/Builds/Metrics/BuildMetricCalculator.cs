using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds.Metrics
{
    public class BuildMetricCalculator
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly IBuild _build;

        public BuildMetricCalculator(ITellTheTime tellTheTime, IBuild build)
        {
            _tellTheTime = tellTheTime;
            _build = build;
        }

        public List<BuildThroughputMetric> CalculateBuildThroughput(BuildFilter buildFilter, IBuildMetric buildMetric)
        {
            var builds = _build.GetSuccessfulBuildStepsContaining("_01");

            var buildMetrics = (BuildThroughputMetric)Calculate(builds, buildFilter, buildMetric);

            return buildMetrics
                .GetResults()
                .OrderBy(result => result.Date)
                .ToList();
        }

        public List<BuildStabilityMetric> CalculateBuildStability(BuildFilter buildFilter, IBuildMetric buildMetric)
        {
            var builds = _build.GetBuilds();

            var buildMetrics = (BuildStabilityMetric)Calculate(builds, buildFilter, buildMetric);

            return buildMetrics
                .GetResults()
                .OrderBy(result => result.Date)
                .ToList();
        }

        public List<BuildDeploymentMetric> CalculateBuildDeployment(BuildFilter buildFilter, IBuildMetric buildMetric)
        {
            var builds = _build.GetSuccessfulBuildStepsContaining("Production");

            var buildMetrics = (BuildDeploymentMetric)Calculate(builds, buildFilter, buildMetric);

            return buildMetrics
                .GetResults()
                .OrderBy(result => result.Date)
                .ToList();
        }
        private IBuildMetric Calculate(List<Build> builds, BuildFilter buildFilter, IBuildMetric buildMetric)
        {
            var filteredBuilds = new FilterBuilds(builds).Filter(buildFilter);

            var fromDate = GetFromDate(buildFilter.NumberOfWeeks);

            for (var x = 0; x < buildFilter.NumberOfWeeks; x++)
            {
                var startDate = fromDate.AddDays(x * 7);

                buildMetric.SetDate(startDate);

                var buildsForDateRange = new FilterBuilds(filteredBuilds).GetBuildsForOneWeekFrom(startDate);

                foreach (var build in new BuildType().GetDistinctBuildTypes(buildsForDateRange))
                {
                    buildMetric.Add(_build,
                        new FilterBuilds(buildsForDateRange).GetBuildsFor(
                            new BuildGroup(build.BuildTypeId)));
                }

                buildMetric.Calculate();
            }

            return buildMetric;
        }

        private DateTime GetFromDate(int numberOfWeeks)
        {
            return GetStartOfWeek().AddDays((numberOfWeeks - 1) * -7);
        }

        private DateTime GetStartOfWeek()
        {
            var today = _tellTheTime.Today();

            var offset = (int)today.DayOfWeek * -1;

            return today.AddDays(offset);
        }
    }
}
