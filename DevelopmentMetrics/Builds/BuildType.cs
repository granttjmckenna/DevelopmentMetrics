using System;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildType
    {
        public string BuildTypeId { get; set; }

        public string BuildTypeGroup => BuildTypeId.Substring(0, BuildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase));

        public string BuildTypeGroupDisplay => Display.ConvertCamelCaseString(BuildTypeGroup);

        public BuildType(string buildTypeId)
        {
            BuildTypeId = buildTypeId;
        }
    }
}