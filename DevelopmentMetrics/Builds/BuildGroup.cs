using System;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildGroup
    {
        public string BuildTypeGroup { get; }

        public string BuildTypeGroupDisplay => Display.ConvertCamelCaseString(BuildTypeGroup);

        public BuildGroup(string buildTypeId)
        {
            BuildTypeGroup = buildTypeId.Substring(0, buildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}