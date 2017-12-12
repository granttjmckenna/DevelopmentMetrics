using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildStabilityViewModel
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly IBuild _build;

        public BuildStabilityViewModel(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        public List<BuildType> GetBuildTypeIdList()
        {
            var builds = _build.GetBuilds();

            var buildTypes = new BuildMetric(_tellTheTime, _build).GetDistinctBuildTypeIdsFrom(builds);

            return buildTypes
                .Select(b => b.BuildTypeGroup)
                .Distinct()
                .Select(buildTypeGroup => buildTypes
                    .First(b => b.BuildTypeGroup.Equals(buildTypeGroup)))
                .ToList();
        }

        public List<FailureRate> GetTopFiveFailingBuildsByRate()
        {
            return new BuildMetric(_tellTheTime, _build).GetFailingBuildsByRate();
        }

        public List<FailureRate> GetTopFivePassingBuildsByRate()
        {
            return new BuildMetric(_tellTheTime, _build).GetPassingBuildsByRate();
        }
    }
}