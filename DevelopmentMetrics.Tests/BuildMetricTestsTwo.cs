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
    public class BuildMetricTestsTwo
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
        public void Should_calculate_failure_percentage_by_week()
        {
            var builds = GetBuildDataFrom(new DateTime(2017, 1, 1), 365);

            var results = new BuildMetric(builds, _tellTheTime).CalculateBuildFailingRateByWeekFor(6);

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

            var milliseconds = new BuildMetric(builds, _tellTheTime).CalculateMillisecondsBetweenBuilds(builds).Sum();

            Assert.That(milliseconds, Is.EqualTo(173640000d));
        }

        [Test]
        public void Return_collection_of_doubles_representing_milliseconds_between_failing_and_succeeding_builds()
        {
            var doubles = new List<double>();

            var builds = GetBuilds("build type 1");

            builds.AddRange(GetBuilds("build type 2"));

            doubles.AddRange(
                new BuildMetric(builds, _tellTheTime).CalculateMillisecondsBetweenBuilds(builds));

            Assert.That(doubles, Is.EqualTo(new List<double> { 180000d, 86520000d, 120000d, 180000d, 86520000d, 120000d, }));
        }

        [Test]
        public void Return_milliseconds_between_failing_and_next_succeeding_build()
        {
            var builds = GetBuilds();

            var doubles = new BuildMetric(builds, _tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles.First(), Is.EqualTo(180000d));
        }

        [Test]
        public void Return_zero_milliseconds_between_failing_and_next_succeeding_build_when_list_is_empty()
        {
            var builds = new List<Build>();

            var doubles = new BuildMetric(builds, _tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles.Sum(), Is.EqualTo(0));
        }

        [Test]
        public void Return_milliseconds_between_failing_and_next_succeeding_build_when_list_ends_with_failing_build()
        {
            var builds = GetBuilds();

            var doubles = new BuildMetric(builds, _tellTheTime).CalculateMillisecondsBetweenBuilds(builds);

            Assert.That(doubles.Sum(), Is.GreaterThan(300000));
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

        private static List<Build> GetBuilds(string buildTypeId = "blah blah")
        {
            return new List<Build>
            {
                new Build
                {
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 1, 0),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 1, 30),
                    Status = "Failure"
                },
                new Build
                {
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 1, 12, 0, 30),
                    FinishDateTime = new DateTime(2017, 11, 1, 12, 3, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 0, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 3, 0),
                    Status = "Success"
                },
                new Build
                {
                    BuildTypeId = buildTypeId,
                    StartDateTime = new DateTime(2017, 11, 2, 12, 3, 30),
                    FinishDateTime = new DateTime(2017, 11, 2, 12, 4, 0),
                    Status = "Failure"
                },
                new Build
                {
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
            return ((i % 3) == 0) ? Helpers.BuildStatus.Failure.ToString() : Helpers.BuildStatus.Success.ToString();
        }
    }
}
