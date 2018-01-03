using System.Collections.Generic;
using DevelopmentMetrics.Builds.Metrics;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildDeployment
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildDeployment(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }
        public List<BuildDeploymentMetric> CalculateBuildDeploymentIntervalByWeekFor(BuildFilter buildFilter)
        {
            if (IsClearCache(buildFilter.NumberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

           return new BuildMetricCalculator(_tellTheTime, _build)
                .CalculateBuildDeployment(
                    buildFilter,
                    new BuildDeploymentMetric());
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}