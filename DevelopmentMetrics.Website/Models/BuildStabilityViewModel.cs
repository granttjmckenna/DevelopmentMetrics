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

        public List<BuildType> GetBuildGroupList()
        {
            return new BuildMetric(_tellTheTime, _build).GetDistinctBuildGroups();
        }

        public List<FailureRate> GetFailingBuildsByRate()
        {
            return new BuildMetric(_tellTheTime, _build).GetFailingBuildsByRate();
        }

        public List<FailureRate> GetPassingBuildsByRate()
        {
            return new BuildMetric(_tellTheTime, _build).GetPassingBuildsByRate();
        }

        public Dictionary<string, string> GetBuildAgentList()
        {
            var results = new Dictionary<string, string>
            {
                {"TC-Agent 1", "lon-devtcagent1"},
                {"TC-Agent 2", "lon-devtcagent2"},
                {"TC-Agent 3", "lon-devtcagent3"},
                {"TC-Agent 4", "lon-devtcagent4"},
                {"TC-Agent 5", "lon-devtcagent5"}
            };


            return results;
        }
    }
}