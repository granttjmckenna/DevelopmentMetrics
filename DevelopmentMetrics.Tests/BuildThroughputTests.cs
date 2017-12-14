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
            var builds = GetBuildDataFrom(new DateTime(2017, 01, 01), 300);

            _build = Substitute.For<IBuild>();
            _tellTheTime = Substitute.For<ITellTheTime>();

            _build.GetBuilds().Returns(builds);
            _build.GetSuccessfulBuildStepsContaining(Arg.Any<string>()).Returns(
                builds
                .Where(b =>
                    b.BuildTypeId.Contains("_01")
                    && b.Status.Equals(BuildStatus.Success.ToString())
                    && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                .ToList());

            _tellTheTime.Today().Returns(new DateTime(2017, 12, 04));
            _tellTheTime.Now().Returns(new DateTime(2017, 12, 04));

            _buildThroughput = new BuildThroughput(_build, _tellTheTime);
        }

        [Test]
        public void Should_calculate_build_interval_by_week()
        {
            var results = _buildThroughput.CalculateBuildIntervalByWeekFor(
                new BuildFilter(6, "blah", "blah")
                );

            Assert.That(results.Count(), Is.EqualTo(6));
            Assert.That(results.First().Date, Is.EqualTo(new DateTime(2017, 10, 22)));
        }

        [Test]
        public void Return_build_time_in_milliseconds()
        {
            var builds = GetBuildDataFrom(new DateTime(2017, 01, 01), 20);

            var successfulBuilds = builds.Where(b => b.Status.Equals(BuildStatus.Success.ToString())).ToList();

            var doubles = GetBuildTimeInMillisecondsFor(successfulBuilds);

            Assert.That(doubles.Count, Is.EqualTo(14));
            Assert.That(doubles.All(ms => ms == 60000d));
        }

        private List<double> GetBuildTimeInMillisecondsFor(List<Build> successfulBuilds)
        {
            return successfulBuilds
                .Select(build => (build.FinishDateTime - build.StartDateTime).TotalMilliseconds)
                .ToList();
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
