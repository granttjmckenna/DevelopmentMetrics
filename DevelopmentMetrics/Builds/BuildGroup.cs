using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildGroup
    {
        private readonly IBuild _build;
        public string BuildTypeGroup { get; }

        public string BuildTypeGroupDisplay => Display.ConvertCamelCaseString(BuildTypeGroup);

        public BuildGroup(IBuild build)
        {
            _build = build;
        }

        public BuildGroup(string buildTypeId)
        {
            BuildTypeGroup = buildTypeId.Substring(0, buildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase));
        }

        public List<BuildGroup> GetDistinctBuildGroups()
        {
            var results = new List<BuildGroup>();

            foreach (var buildGroup in _build.GetBuilds()
                .Select(b => new BuildGroup(b.BuildTypeId)))
            {
                if (!results.Exists(b => b.BuildTypeGroup.Equals(buildGroup.BuildTypeGroup)))
                    results.Add(buildGroup);
            }

            return results.OrderBy(b => b.BuildTypeGroup).ToList();
        }
    }
}