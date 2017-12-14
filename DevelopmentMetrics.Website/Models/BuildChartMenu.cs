using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildChartMenu
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildChartMenu(ITellTheTime tellTheTime, IBuild build)
        {
            _tellTheTime = tellTheTime;
            _build = build;
        }

        public Dictionary<string, string> GetBuildGroupList()
        {
            return new BuildStability(_tellTheTime, _build)
                .GetDistinctBuildGroups()
                .ToDictionary(b => b.BuildTypeGroupDisplay, b => b.BuildTypeGroup);
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

        public Dictionary<string, string> GetBuildWeeksList()
        {
            var results = new Dictionary<string, string>
            {
                {"6 weeks (default)", "6"},
                {"7 weeks", "7"},
                {"8 weeks", "8"},
                {"9 weeks", "9"},
                {"10 weeks", "10"},
                {"15 weeks", "15"},
                {"20 weeks", "20"},
                {"All", "-2"}
            };

            return results;
        }

    }
}