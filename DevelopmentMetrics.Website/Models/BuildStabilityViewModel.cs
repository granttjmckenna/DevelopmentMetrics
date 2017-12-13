using System.Collections.Generic;
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

        public List<BuildGroup> GetBuildGroupList()
        {
            return new BuildStability(_tellTheTime, _build).GetDistinctBuildGroups();
        }

        public List<BuildFailureRate> GetFailingBuildsByRate()
        {
            return new BuildStability(_tellTheTime, _build).GetFailingBuildsByRate();
        }

        public List<BuildFailureRate> GetPassingBuildsByRate()
        {
            return new BuildStability(_tellTheTime, _build).GetPassingBuildsByRate();
        }
    }
}