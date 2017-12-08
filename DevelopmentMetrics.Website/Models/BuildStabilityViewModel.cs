using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildStabilityViewModel
    {
        private readonly List<string> _buildTypeIds;

        public BuildStabilityViewModel(List<string> buildTypeIds)
        {
            _buildTypeIds = buildTypeIds;
        }

        public List<string> GetBuildTypeIdList()
        {
            var allBuildTypeIds = _buildTypeIds
                .Select(buildTypeId => buildTypeId.Substring(0, buildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            return new List<string>(allBuildTypeIds.OrderBy(x => x).Distinct());
        }
    }
}