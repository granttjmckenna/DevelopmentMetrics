﻿using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds.Metrics;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildThroughput
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildThroughput(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        public List<BuildThroughputMetric> CalculateBuildThroughputByWeekFor(BuildFilter buildFilter)
        {
            if (IsClearCache(buildFilter.NumberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            return new BuildMetricCalculator(_tellTheTime, _build)
                .CalculateBuildThroughput(
                    buildFilter,
                    new BuildThroughputMetric());
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}