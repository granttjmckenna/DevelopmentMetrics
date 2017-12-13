using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class BuildThroughputTests
    {
        private BuildThroughput _buildThroughput;
        private IBuild _build;
        private ITellTheTime _tellTheTime;

        [SetUp]
        public void Setup()
        {
            _build = Substitute.For<IBuild>();
            _tellTheTime = Substitute.For<ITellTheTime>();

            _build.GetBuilds().Returns(GetBuildDataFrom(new DateTime(2017, 01, 01), 300));
            _tellTheTime.Today().Returns(new DateTime(2017, 12, 04));
            _tellTheTime.Now().Returns(new DateTime(2017, 12, 04));

            _buildThroughput = new BuildThroughput(_build, _tellTheTime);
        }

        [Test]
        public void Should_calculate_build_interval_by_week()
        {
            var results = _buildThroughput.CalculateBuildIntervalByWeekFor(6);

            Assert.That(results.Count(), Is.EqualTo(6));
            Assert.That(results.First().Date, Is.EqualTo(new DateTime(2017, 10, 22)));
        }

        [Test]
        public void Return_all_successful_create_artifact_build_steps_from_builds_list()
        {
            var buildStepBuilds = _buildThroughput.GetSuccessfulBuildStepBuilds();

            Assert.That(buildStepBuilds.Any());
            Assert.That(buildStepBuilds.All(b => b.BuildTypeId.Contains("_01")));
            Assert.That(buildStepBuilds.All(b => b.Status.Equals(BuildStatus.Success.ToString())));
            Assert.That(buildStepBuilds.All(
                    b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Test]
        public void Return_all_builds_for_one_week()
        {
            var buildStepBuilds = _buildThroughput.GetSuccessfulBuildStepBuilds();

            var buildsForWeek = new BuildThroughput().GetBuildsForDateRange(buildStepBuilds, new DateTime(2017, 10, 01));

            Assert.That(buildsForWeek.All(b => b.StartDateTime >= new DateTime(2017, 10, 01)));
            Assert.That(buildsForWeek.All(b => b.StartDateTime < new DateTime(2017, 10, 08)));
        }

        [Test]
        public void Return_time_in_milliseconds_between_successful_builds()
        {
            var buildStepBuilds = _buildThroughput.GetSuccessfulBuildStepBuilds();

            var buildsForWeek = new BuildThroughput().GetBuildsForDateRange(buildStepBuilds, new DateTime(2017, 10, 01));

            var doubles = new BuildThroughput().GetTimeInMillisecondsBetweenBuildsFor(buildsForWeek);

            Assert.That(doubles.Count, Is.EqualTo(3));
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
}
