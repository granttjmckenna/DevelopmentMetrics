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
                        Status = "Success"
                    },
                    new Build
                    {
                        Id = 2,
                        BuildTypeId = "lowest failing build type id",
                        StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                        FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                        Status = "Success"
                    }
            };

            builds.AddRange(GetBuilds("highest failing build type id"));

            var failingBuilds = new BuildMetric(_tellTheTime).GetTopFiveFailingBuildsByRate(builds);

            Assert.That(failingBuilds.First().BuildTypeId, Is.EqualTo("highest failing build type id"));
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
        public void Return_cumulative_milliseconds_when_two_build_types_exist()
        {
            var builds = GetBuilds("build type 1");

            builds.AddRange(GetBuilds("build type 2"));

            var milliseconds = new BuildMetric(_tellTheTime).CalculateMillisecondsBetweenBuilds(builds).Sum();

            Assert.That(milliseconds, Is.EqualTo(600000d));
        }

        [Test]
        public void Return_collection_of_doubles_representing_milliseconds_between_failing_and_succeeding_builds()
        {
            var doubles = new List<double>();

            var builds = GetBuilds("build type 1");

            builds.AddRange(GetBuilds("build type 2"));

            doubles.AddRange(
                new BuildMetric(_tellTheTime).CalculateMillisecondsBetweenBuilds(builds));

            Assert.That(doubles, Is.EqualTo(new List<double> { 180000d, 120000d, 180000d, 120000d }));
        }

        [Test]
        public void Return_milliseconds_between_failing_and_next_succeeding_build()
        {
            var builds = GetBuilds();

            var doubles = new BuildMetric(_tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles.First(), Is.EqualTo(180000d));
        }

        [Test]
        public void Return_zero_milliseconds_between_failing_and_next_succeeding_build_when_list_is_empty()
        {
            var builds = new List<Build>();

            var doubles = new BuildMetric(_tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles.Sum(), Is.EqualTo(0));
        }

        [Test]
        public void Return_milliseconds_between_failing_and_next_succeeding_build_when_list_ends_with_failing_build()
        {
            var builds = GetBuilds();

            builds.Add(new Build
            {
                Id = 999,
                BuildTypeId = "blah blah",
                StartDateTime = new DateTime(2017, 11, 2, 15, 3, 30),
                FinishDateTime = new DateTime(2017, 11, 2, 15, 5, 30),
                Status = "Failure"
            });

            var doubles = new BuildMetric(_tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles.Sum(), Is.EqualTo(300000d));
        }


        [Test]
        public void Return_for_standard_deviation_when_list_is_empty()
        {
            var values = new List<double>();

            var standardDeviation = Calculator.CalculateStandardDeviation(values);

            Assert.That(standardDeviation, Is.EqualTo(0));
        }

        [Test]
        public void Return_standard_deviation_when_list_is_not_empty()
        {
            var values = new List<double> { 1d, 2d, 3d, 2d, 1d };

            var standardDeviation = Calculator.CalculateStandardDeviation(values);

            Assert.That(standardDeviation, Is.GreaterThan(0.83d));
            Assert.That(standardDeviation, Is.LessThan(0.84d));
        }

        [Test]
        public void Return_collection_of_distinct_buildd_type_ids()
        {
            var builds = GetBuilds("build type 2");

            builds.AddRange(GetBuilds("build type 1"));

            var buildTypeIds = new BuildMetric(_tellTheTime).GetDistinctBuildTypeIdsFrom(builds);

            Assert.That(buildTypeIds.Count, Is.EqualTo(2));
            Assert.That(buildTypeIds.First(), Is.EqualTo("build type 1"));
            Assert.That(buildTypeIds.Last(), Is.EqualTo("build type 2"));
        }

        [Test]
        public void Return_the_difference_in_milliseconds_between_failing_and_succeeding_builds()
        {
            var builds = GetPriceWatchBuilds();

            var doubles = new BuildMetric(_tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles[0], Is.EqualTo(2100000d));
            Assert.That(doubles[1], Is.EqualTo(2400000d));
            Assert.That(doubles[2], Is.EqualTo(2760000d));
        }

        [Test]
        public void Return_average_recovery_time_in_hours()
        {
            var doubles = new List<double> { 2100000d, 2400000d, 2760000d };

            var average = new BuildMetric(_tellTheTime).CalculateAverageRecoveryTimeInHoursFor(doubles);

            Assert.That(average, Is.EqualTo(1));
        }

        [Test]
        public void Return_standard_deviation_of_recovery_time_in_hours()
        {
            var doubles = new List<double> { 2100000d, 2400000d, 2760000d };

            var standardDeviationInHours =
                new BuildMetric(_tellTheTime).ConvertMillisecondsToHours(
                    Calculator.CalculateStandardDeviation(doubles));

            Assert.That(standardDeviationInHours, Is.EqualTo(0));
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
                    Status = "Failure"
                },

                new Build
                {
                    Id = 2,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                    Status = "Failure"
                },

                new Build
                {
                    Id = 3,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 3, 0),
                    Status = "Success"
                },

                new Build
                {
                    Id = 4,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 0, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 3, 0),
                    Status = "Success"
                },

                new Build
                {
                    Id = 5,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 4, 0),
                    Status = "Failure"
                },

                new Build
                {
                    Id = 6,
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 5, 30),
                    Status = "Success"
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
