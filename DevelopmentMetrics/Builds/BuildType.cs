using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Builds
{
    public class BuildType
    {
        public string BuildTypeId { get; set; }

        public BuildGroup BuildGroup => new BuildGroup(BuildTypeId);

        public BuildType(string buildTypeId)
        {
            BuildTypeId = buildTypeId;
        }

        public BuildType() { }

        public List<BuildType> GetDistinctBuildTypeIds(List<Build> builds)
        {
            return builds
                .OrderBy(b => b.BuildTypeId)
                .Select(b => b.BuildTypeId)
                .Distinct()
                .Select(buildTypeId => new BuildType(buildTypeId))
                .ToList();
        }
    }
}