using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildThroughput
    {
        private readonly IBuild _build;

        public BuildThroughput()
        {
            //to be deleted later
        }

        public BuildThroughput(IBuild build)
        {
            _build = build;
        }

        public List<double> GetTimeInMillisecondsBetweenBuildsFor(List<Build> builds)
        {
            var results = new List<double>();

            for (var x = builds.Count - 1; x > 0; x--)
            {
                results.Add((builds[x].StartDateTime - builds[x - 1].StartDateTime).TotalMilliseconds);
            }

            return results;
        }

        public List<Build> GetSuccessfulBuildStepBuilds()
        {
            return _build.GetBuilds()
                .Where(
                    b => b.BuildTypeId.Contains("_01")
                         && b.Status.Equals(BuildStatus.Success.ToString())
                         && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        public List<Build> GetBuildsForDateRange(List<Build> builds, DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            return builds
                .Where(b =>
                    b.StartDateTime >= startDate
                    && b.StartDateTime < endDate)
                .ToList();
        }
    }
}