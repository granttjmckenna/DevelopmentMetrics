﻿using System;
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

                var buildsForDateRange = GetBuildsForDateRange(filteredBuilds, startDate);

                foreach (var build in new BuildType().GetDistinctBuildTypeIds(filteredBuilds))
                {
                    var buildsByType = buildsForDateRange
                        .Where(b => b.BuildTypeId.Equals(build.BuildTypeId, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();

                    buildMetric.Add(_build, buildsByType);
                }

                buildMetric.Calculate();
            }

            return buildMetric;
        }

        private DateTime GetFromDate(int numberOfWeeks)
        {
            return GetStartOfWeekFor(_tellTheTime.Today()).AddDays(numberOfWeeks * -7);
        }

        private DateTime GetStartOfWeekFor(DateTime today)
        {
            var offset = (int)today.DayOfWeek * -1;

            return today.AddDays(offset);
        }

        private List<Build> GetBuildsForDateRange(List<Build> builds, DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            return builds
                .Where(b =>
                    b.StartDateTime >= startDate
                    && b.StartDateTime < endDate)
                .ToList();
        }
    }
}