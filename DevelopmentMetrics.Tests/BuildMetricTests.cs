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

        [SetUp]
        public void Setup()
        {
            _tellTheTime = Substitute.For<ITellTheTime>();

            _tellTheTime.Today().Returns(new DateTime(2017, 12, 04));
            _tellTheTime.Now().Returns(new DateTime(2017, 12, 04));
        }

        [Test]
        public void Return_build_type_with_highest_failure_rate()
        {
            var builds = new List<Build>
            {
                    new Build
                    {
                        Id = 1,
                        BuildTypeId = "lowest failing build type id",
                        StartDateTime = new DateTime(2017, 11, 1, 12, 0, 0),
                        FinishDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                        Status = "Success",
                        State = "Finished"
                    },
                    new Build
                    {
                        Id = 2,
                        BuildTypeId = "lowest failing build type id",
                        StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                        FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                        Status = "Success",
                        State = "Finished"
                    }
            };

            builds.AddRange(GetBuilds("highest failing build type id"));

            var failingBuilds = new BuildMetric(_tellTheTime).GetTopFiveFailingBuildsByRate(builds);

            Assert.That(failingBuilds.First().BuildTypeId, Is.EqualTo("highest failing build type id"));
        }

        [Test]
        public void Return_build_type_with_lowest_failure_rate()
        {
            var builds = new List<Build>
            {
                new Build
                {
                    Id = 1,
                    BuildTypeId = "lowest failing build type id",
                    StartDateTime = new DateTime(2017, 12, 5, 12, 0, 0),
                    FinishDateTime = new DateTime(2017, 12, 5, 12, 0, 30),
                    Status = "Success",
                    State = "Finished"
                },
                new Build
                {
                    Id = 2,
                    BuildTypeId = "lowest failing build type id",
                    StartDateTime = new DateTime(2017, 12, 5, 12, 1, 0),
                    FinishDateTime = new DateTime(2017, 12, 5, 12, 1, 30),
                    Status = "Success",
                    State = "Finished"
                }
            };

            builds.AddRange(GetBuilds("highest failing build type id"));

            var failingBuilds = new BuildMetric(_tellTheTime).GetTopFivePassingBuildsByRate(builds);

            Assert.That(failingBuilds.First().BuildTypeId, Is.EqualTo("lowest failing build type id"));
        }

        [Test]
        public void Should_calculate_failure_percentage_by_week()
        {
            var builds = GetBuildDataFrom(new DateTime(2017, 1, 1), 365);

            var results = new BuildMetric(_tellTheTime).CalculateBuildFailingRateByWeekFor(builds, 6);

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
                    Status = "Failure",
                    State = "Finished"
                },
                new Build
                {
                    Id = 1001,
                    BuildTypeId = "blah blah",
                    StartDateTime = new DateTime(2017, 11, 27, 15, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 27, 16, 5, 30),
                    Status = "Success",
                    State = "Finished"
                }
            };

            var metrics = new BuildMetric(_tellTheTime).CalculateBuildFailingRateByWeekFor(builds, 1);

            Assert.That(metrics.First().RecoveryTime, Is.EqualTo(1));
        }

        [Test]
        public void Return_collection_of_distinct_build_type_ids()
        {
            var builds = GetBuilds("build type 2");

            builds.AddRange(GetBuilds("build type 1"));

            var buildTypes = new BuildMetric(_tellTheTime).GetDistinctBuildTypeIdsFrom(builds);

            Assert.That(buildTypes.Count, Is.EqualTo(2));
            Assert.That(buildTypes.First().BuildTypeId, Is.EqualTo("build type 1"));
            Assert.That(buildTypes.Last().BuildTypeId, Is.EqualTo("build type 2"));
        }

        private List<Build> GetPriceWatchBuilds()
        {
            return new List<Build>
            {
                new Build
                {
                    BuildTypeId = "price-watch-build",
                    StartDateTime = new DateTime(2017, 11, 12, 09, 43, 0),
                    FinishDateTime = new DateTime(2017, 11, 12, 09, 45, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = "price-watch-build",
                    StartDateTime = new DateTime(2017, 11, 12, 10, 43, 0),
                    FinishDateTime = new DateTime(2017, 11, 12, 10, 45, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = "price-watch-build",
                    StartDateTime = new DateTime(2017, 11, 12, 11, 43, 0),
                    FinishDateTime = new DateTime(2017, 11, 12, 11, 45, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = "price-watch-build",
                    StartDateTime = new DateTime(2017, 11, 14, 11, 43, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 11, 45, 0),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = "price-watch-build",
                    StartDateTime = new DateTime(2017, 11, 14, 12, 01, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 12, 03, 0),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = "price-watch-build",
                    StartDateTime = new DateTime(2017, 11, 14, 12, 18, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 12, 18, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = "price-watch-deploy-to-test",
                    StartDateTime = new DateTime(2017, 11, 14, 11, 45, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 11, 45, 0),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = "price-watch-deploy-to-test",
                    StartDateTime = new DateTime(2017, 11, 14, 12, 03, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 12, 03, 0),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = "price-watch-deploy-to-test",
                    StartDateTime = new DateTime(2017, 11, 14, 12, 25, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 12, 25, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = "price-watch-promote-to-staging",
                    StartDateTime = new DateTime(2017, 11, 14, 11, 45, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 11, 45, 0),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = "price-watch-promote-to-staging",
                    StartDateTime = new DateTime(2017, 11, 14, 12, 03, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 12, 03, 0),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = "price-watch-promote-to-staging",
                    StartDateTime = new DateTime(2017, 11, 14, 12, 31, 0),
                    FinishDateTime = new DateTime(2017, 11, 14, 12, 31, 0),
                    Status = "Success"
                }
            };
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
