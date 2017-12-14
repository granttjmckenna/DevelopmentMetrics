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
    public class BuildDeploymentTests
    {
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
                        b.BuildTypeId.Contains("Production")
                        && b.Status.Equals(BuildStatus.Success.ToString())
                        && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                    .ToList());

            _tellTheTime.Today().Returns(new DateTime(2017, 12, 04));
            _tellTheTime.Now().Returns(new DateTime(2017, 12, 04));

        }

        [Test]
        public void Return_matching_build_step_for_production_build_step()
        {
            var productionBuild = _build.GetSuccessfulBuildStepsContaining("Production").First();

            var buildStep = GetMatchingBuildStep(productionBuild);

            Assert.That(buildStep.BuildTypeId, Is.EqualTo(productionBuild.BuildTypeId));
            Assert.That(buildStep.Number, Is.EqualTo(productionBuild.Number));
            Assert.That(buildStep.State, Is.EqualTo("Finished"));
            Assert.That(buildStep.Status, Is.EqualTo("Success"));
        }

        [Test]
        public void Return_duration_in_milliseconds_between_production_and_build_step()
        {
            var productionBuild = _build.GetSuccessfulBuildStepsContaining("Production").First();

            var buildStep = GetMatchingBuildStep(productionBuild);

            var duration = (productionBuild.FinishDateTime - buildStep.StartDateTime).TotalMilliseconds;

            Assert.That(duration, Is.EqualTo(60000d));
        }

        [Test]
        public void Return_list_of_lead_time_in_milliseconds_between_production_and_build_step()
        {
            var leadTimes = (from productionBuild in
                                 _build.GetSuccessfulBuildStepsContaining("Production")
                             let buildStep = GetMatchingBuildStep(productionBuild)
                             select (productionBuild.FinishDateTime - buildStep.StartDateTime).TotalMilliseconds)
                             .ToList();

            Assert.That(leadTimes.Count, Is.EqualTo(200));
        }

        private Build GetMatchingBuildStep(Build productionBuild)
        {
            var buildStep = _build.GetSuccessfulBuildStepsContaining("01")
                .First(b =>
                    b.Number == productionBuild.Number &&
                    b.BuildTypeId.StartsWith(new BuildGroup(b.BuildTypeId).BuildTypeGroup, StringComparison.InvariantCultureIgnoreCase) &&
                    b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase) &&
                    b.Status.Equals(BuildStatus.Success.ToString()));

            return buildStep;
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
                        Status = GetStatus(i),
                        Number = "999"
                    }
                );
            }

            return dummyBuilds;
        }

        private string GetBuildStep(int i)
        {
            return (i % 3) == 0 ? $"blah_blah_{i}" : $"blah_blah_Production";
        }

        private string GetStatus(int i)
        {
            return ((i % 3) == 0) ? BuildStatus.Failure.ToString() : BuildStatus.Success.ToString();
        }

    }
}
