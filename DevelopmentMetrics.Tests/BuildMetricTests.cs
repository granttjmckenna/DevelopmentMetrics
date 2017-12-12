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
    public class BuildMetricTests
    {
        private ITellTheTime _tellTheTime;
        private IBuild _build;

        [SetUp]
        public void Setup()
        {
            _tellTheTime = Substitute.For<ITellTheTime>();
            _build = Substitute.For<IBuild>();

            _tellTheTime.Today().Returns(new DateTime(2017, 12, 04));
            _tellTheTime.Now().Returns(new DateTime(2017, 12, 04));
        }

        [Test]
        public void Return_all_failing_builds()
        {
            var builds = new List<Build>
            {
                new Build
                {
                    Id = 1,
                    BuildTypeId = "passing build type id",
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    Status = "Success",
                    State = "Finished"
                },
                new Build
                {
                    Id = 2,
                    BuildTypeId = "passing build type id",
                    StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                    Status = "Success",
                    State = "Finished"
                }
            };

            builds.AddRange(GetBuilds("failing build type id"));

            _build.GetBuilds().Returns(builds);

            var failingBuilds = new BuildMetric(_tellTheTime, _build).GetFailingBuildsByRate();

            Assert.That(failingBuilds.All(b => b.BuildTypeId.Equals("failing build type id")));
        }

        [Test]
        public void Return_all_passing_builds()
        {
            var builds = new List<Build>
            {
                new Build
                {
                    Id = 1,
                    BuildTypeId = "passing build type id",
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    Status = "Success",
                    State = "Finished"
                },
                new Build
                {
                    Id = 2,
                    BuildTypeId = "passing build type id",
                    StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                    Status = "Success",
                    State = "Finished"
                }
            };

            builds.AddRange(GetBuilds("passing build type id"));

            _build.GetBuilds().Returns(builds);

            var failingBuilds = new BuildMetric(_tellTheTime, _build).GetPassingBuildsByRate();

            Assert.That(failingBuilds.All(b => b.BuildTypeId.Equals("passing build type id")));
        }
        
        [Test]
        public void Should_calculate_failure_percentage_by_week()
        {
            var builds = GetBuildDataFrom(new DateTime(2017, 1, 1), 365);

            _build.GetBuilds().Returns(builds);

            var results =
                new BuildMetric(_tellTheTime, _build).CalculateBuildFailingRateByWeekFor(new BuildFilter(6, "All", "All"));

            Assert.That(results.Count(), Is.EqualTo(6));
            Assert.That(results.First().Date, Is.EqualTo(new DateTime(2017, 10, 22)));
        }

        [Test]
        public void Return_nearest_previous_Sunday_to_date()
        {
            var today = new DateTime(2017, 12, 04);

            var startOfWeek = GetStartOfWeekFor(today);

            Assert.That(startOfWeek, Is.EqualTo(new DateTime(2017, 12, 03)));
        }

        [Test]
        public void Return_milliseconds_between_failing_and_next_succeeding_build()
        {
            var builds = new List<Build>
            {
                new Build
                {
                    Id = 999,
                    BuildTypeId = "blah blah",
                    StartDateTime = new DateTime(2017, 11, 27, 15, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 27, 15, 5, 30),
                    AgentName = "agent name",
                    Status = "Failure",
                    State = "Finished"
                },
                new Build
                {
                    Id = 1001,
                    BuildTypeId = "blah blah",
                    StartDateTime = new DateTime(2017, 11, 27, 15, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 27, 16, 5, 30),
                    AgentName = "agent name",
                    Status = "Success",
                    State = "Finished"
                }
            };

            _build.GetBuilds().Returns(builds);

            var metrics =
                new BuildMetric(_tellTheTime, _build).CalculateBuildFailingRateByWeekFor(new BuildFilter(1, "All", "All"));

            Assert.That(metrics.First().RecoveryTime, Is.EqualTo(1));
        }

        [Test]
        public void Return_collection_of_distinct_build_type_ids()
        {
            var builds = GetBuilds("build type 2");

            builds.AddRange(GetBuilds("build type 1"));

            _build.GetBuilds().Returns(builds);

            var buildTypes = new BuildMetric(_tellTheTime, _build).GetDistinctBuildTypeIds();

            Assert.That(buildTypes.Count, Is.EqualTo(2));
            Assert.That(buildTypes.First().BuildTypeId, Is.EqualTo("build type 1"));
            Assert.That(buildTypes.Last().BuildTypeId, Is.EqualTo("build type 2"));
        }

        private static List<Build> GetBuilds(string buildTypeId = "blah blah")
        {
            return new List<Build>
            {
                new Build
                {
                    Id = 1,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    Status = "Failure",
                    State = "Finished"
                },

                new Build
                {
                    Id = 2,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                    Status = "Failure",
                    State = "Finished"
                },

                new Build
                {
                    Id = 3,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 3, 0),
                    Status = "Success",
                    State = "Finished"
                },

                new Build
                {
                    Id = 4,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 0, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 3, 0),
                    Status = "Success",
                    State = "Finished"
                },

                new Build
                {
                    Id = 5,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 4, 0),
                    Status = "Failure",
                    State = "Finished"
                },

                new Build
                {
                    Id = 6,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 5, 30),
                    Status = "Success",
                    State = "Finished"
                }
            };
        }

        private DateTime GetStartOfWeekFor(DateTime today)
        {
            var offset = (int)today.DayOfWeek * -1;

            return today.AddDays(offset);
        }

        private List<Build> GetBuildDataFrom(DateTime fromDate, int rows)
        {
            var dummyBuilds = new List<Build>();

            for (var i = 1; i <= rows; i++)
            {
                dummyBuilds.Add(
                    new Build
                    {
                        BuildTypeId = "Blah_blah",
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

        private string GetStatus(int i)
        {
            return ((i % 3) == 0) ? BuildStatus.Failure.ToString() : BuildStatus.Success.ToString();
        }
    }
}
