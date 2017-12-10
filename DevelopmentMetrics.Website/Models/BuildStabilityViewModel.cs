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

        public BuildStabilityViewModel(List<Build> builds,ITellTheTime tellTheTime)
        {
            _tellTheTime = tellTheTime;
            _builds = builds;
        }

        public List<string> GetBuildTypeIdList()
        {
            var buildTypeIds = new BuildMetric(_tellTheTime).GetDistinctBuildTypeIdsFrom(_builds);

            var allBuildTypeIds = buildTypeIds
                .Select(buildTypeId => buildTypeId.Substring(0, buildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            return new List<string>(allBuildTypeIds.OrderBy(x => x).Distinct());
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