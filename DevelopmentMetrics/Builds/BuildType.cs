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

        public List<BuildType> GetDistinctBuildTypes(List<Build> builds)
        {
            var buildTypes = new List<BuildType>();

            foreach (var build in builds)
            {
                var buildType = new BuildType(build.BuildTypeId);

                if (buildTypes.All(b => b.BuildGroup.BuildTypeGroup != buildType.BuildGroup.BuildTypeGroup))
                {
                    buildTypes.Add(buildType);
                }
            }

            return buildTypes.ToList();
        }
    }
}