using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildStabilityViewModel
    {
        private readonly List<Build> _builds;
        private readonly ITellTheTime _tellTheTime;

        public BuildStabilityViewModel(List<Build> builds, ITellTheTime tellTheTime)
        {
            _tellTheTime = tellTheTime;
            _builds = builds;
        }

        public List<BuildType> GetBuildTypeIdList()
        {
            var buildTypes = new BuildMetric(_tellTheTime).GetDistinctBuildTypeIdsFrom(_builds);

            return buildTypes
                .Select(b => b.BuildTypeGroup)
                .Distinct()
                .Select(buildTypeGroup => buildTypes
                    .First(b => b.BuildTypeGroup.Equals(buildTypeGroup)))
                .ToList();
        }

        public List<FailureRate> GetTopFiveFailingBuildsByRate()
        {
            return new BuildMetric(_tellTheTime).GetTopFiveFailingBuildsByRate(_builds);
        }

        public List<FailureRate> GetTopFivePassingBuildsByRate()
        {
            return new BuildMetric(_tellTheTime).GetTopFivePassingBuildsByRate(_builds);
        }
    }
}