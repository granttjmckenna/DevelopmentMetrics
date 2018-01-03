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

            return new BuildMetricCalculator(_tellTheTime, _build)
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

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}
