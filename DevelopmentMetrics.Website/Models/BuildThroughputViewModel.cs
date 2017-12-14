using System.Collections.Generic;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildThroughputViewModel
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildThroughputViewModel(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        public Dictionary<string, string> GetBuildGroupList()
        {
            return new BuildChartMenu(_tellTheTime, _build).GetBuildGroupList();
        }

        public Dictionary<string, string> GetAgentsList()
        {
            return new BuildChartMenu(_tellTheTime, _build).GetBuildAgentList();
        }

        public Dictionary<string, string> GetBuildWeeksList()
        {
            return new BuildChartMenu(_tellTheTime, _build).GetBuildWeeksList();
        }
    }
}