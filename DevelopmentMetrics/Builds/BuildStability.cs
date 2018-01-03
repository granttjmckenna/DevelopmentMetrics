using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds.Metrics;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildStability
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildStability(ITellTheTime tellTheTime, IBuild build)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        public List<BuildStabilityMetric> CalculateBuildFailingRateByWeek(BuildFilter buildFilter)
        {
            if (IsClearCache(buildFilter.NumberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            return new BuildMetricCalculator(_tellTheTime, _build.GetBuilds())
                .CalculateBuildStability(
                    buildFilter,
                    new BuildStabilityMetric());
        }

        public List<BuildFailureRate> GetFailingBuildsByRate()
        {
            return new BuildStabilityMetric().GetFailingBuildsByRate(_build.GetBuilds());
        }

        public List<BuildFailureRate> GetPassingBuildsByRate()
        {
            return new BuildStabilityMetric().GetPassingBuildsByRate(_build.GetBuilds());
        }

        public List<BuildGroup> GetDistinctBuildGroups()
        {
            var results = new List<BuildGroup>();

            foreach (var buildGroup in _build.GetBuilds()
                .Select(b => new BuildGroup(b.BuildTypeId)))
            {
                if (!results.Exists(b => b.BuildTypeGroup.Equals(buildGroup.BuildTypeGroup)))
                    results.Add(buildGroup);
            }

            return results.OrderBy(b => b.BuildTypeGroup).ToList();
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}
