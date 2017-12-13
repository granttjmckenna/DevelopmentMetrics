using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class BuildThroughputTests
    {
        [Test]
        public void Return_all_successful_create_artifact_build_steps_from_builds_list()
        {
            var builds = GetBuildDataFrom(new DateTime(2017, 01, 01), 300);

            var buildStepBuilds = new BuildThroughput().GetSuccessfulBuildStepBuildsFrom(builds);

            Assert.That(buildStepBuilds.Any());
            Assert.That(buildStepBuilds.All(b => b.BuildTypeId.Contains("_01")));
            Assert.That(buildStepBuilds.All(b => b.Status.Equals(BuildStatus.Success.ToString())));
            Assert.That(buildStepBuilds.All(
                    b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Test]
        public void Return_all_builds_for_one_week()
        {
            var builds = GetBuildDataFrom(new DateTime(2017, 01, 01), 300);

            var buildStepBuilds = new BuildThroughput().GetSuccessfulBuildStepBuildsFrom(builds);

            var buildsForWeek = new BuildThroughput().GetBuildsForDateRange(buildStepBuilds, new DateTime(2017, 10, 01));

            Assert.That(buildsForWeek.All(b => b.StartDateTime >= new DateTime(2017, 10, 01)));
            Assert.That(buildsForWeek.All(b => b.StartDateTime < new DateTime(2017, 10, 08)));
        }

        private List<Build> GetBuildDataFrom(DateTime fromDate, int rows)
        {
            var dummyBuilds = new List<Build>();

            for (var i = 1; i <= rows; i++)
            {
                dummyBuilds.Add(
                    new Build
                    {
                        BuildTypeId = GetBuildStep(i),
                        Id = i,
                        AgentName = "Blah",
                        StartDateTime = fromDate.AddDays(i).AddMinutes(1),
                        FinishDateTime = fromDate.AddDays(i).AddMinutes(2),
                        QueueDateTime = fromDate.AddDays(i),
                        State = "Finished",
                        Status = GetStatus(i)
                    }
                );
            }

            return dummyBuilds;
        }

        private string GetBuildStep(int i)
        {
            return (i % 3) == 0 ? $"blah_blah_{i}" : $"blah_blah_01";
        }

        private string GetStatus(int i)
        {
            return ((i % 3) == 0) ? BuildStatus.Failure.ToString() : BuildStatus.Success.ToString();
        }
    }

    public class BuildThroughput
    {
        public List<Build> GetSuccessfulBuildStepBuildsFrom(List<Build> builds)
        {
            return builds
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
