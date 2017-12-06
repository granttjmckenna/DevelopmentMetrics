using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
        public void Return_milliseconds_between_failing_and_next_succeeding_build()
        {
            var builds = GetBuilds();

            var millisecondsBetweenBuilds = CalculateMillisecondsBetweenBuilds(GetAlternatingBuilds(builds));

            Assert.That(millisecondsBetweenBuilds, Is.EqualTo(300000));
        }

        [Test]
        public void Return_zero_milliseconds_between_failing_and_next_succeeding_build_when_list_is_empty()
        {
            var builds = new List<Build>();

            var millisecondsBetweenBuilds = CalculateMillisecondsBetweenBuilds(GetAlternatingBuilds(builds));

            Assert.That(millisecondsBetweenBuilds, Is.EqualTo(0));
        }

        [Test]
        public void Return_milliseconds_between_failing_and_next_succeeding_build_when_list_ends_with_failing_build()
        {
            var builds = GetBuilds();

            var alternatingBuilds = GetAlternatingBuilds(builds);

            alternatingBuilds = alternatingBuilds.Take(3).ToList(); //remove last successful build

            var millisecondsBetweenBuilds = CalculateMillisecondsBetweenBuilds(alternatingBuilds);

            Assert.That(millisecondsBetweenBuilds, Is.GreaterThan(300000));
        }

        [Test]
        public void Return_list_of_failing_and_succeeding_builds()
        {
            var builds = GetBuilds();

            var filteredBuilds = GetAlternatingBuilds(builds);

            Assert.That(filteredBuilds.Count, Is.EqualTo(4));
        }

        [Test]
        public void Return_cumulative_milliseconds_when_two_build_types_exist()
        {
            var builds = GetBuilds("build type 1");

            builds.AddRange(GetBuilds("build type 2"));

            var milliseconds = GetTotalMillisecondsFor(builds);

            Assert.That(milliseconds, Is.EqualTo(600000));
        }

        private double GetTotalMillisecondsFor(List<Build> builds)
        {
            var distinctBuildTypes = builds.Select(b => b.BuildTypeId).Distinct().ToList();

            var milliseconds = distinctBuildTypes.Sum(
                buildType => CalculateMillisecondsBetweenBuilds(
                    GetAlternatingBuilds(
                        builds.Where(b => b.BuildTypeId.Equals(buildType, StringComparison.InvariantCultureIgnoreCase))
                            .ToList())));
            return milliseconds;
        }

        private List<Build> GetAlternatingBuilds(List<Build> builds)
        {
            var results = new List<Build>();

            var isPreviousBuildSuccess = true;

            foreach (var build in builds)
            {
                if (build.Status.Equals("Failure", StringComparison.InvariantCultureIgnoreCase) && isPreviousBuildSuccess)
                {
                    results.Add(build);
                    isPreviousBuildSuccess = false;
                }
                else if (build.Status.Equals("Success", StringComparison.InvariantCultureIgnoreCase) && !isPreviousBuildSuccess)
                {
                    results.Add(build);
                    isPreviousBuildSuccess = true;
                }
            }

            return results;
        }


        private double CalculateMillisecondsBetweenBuilds(List<Build> builds)
        {
            var runningTotal = 0d;

            for (var x = 0; x < builds.Count - 1; x += 2)
            {
                runningTotal += (builds[x + 1].FinishDateTime - builds[x].StartDateTime).TotalMilliseconds;
            }

            if (builds.Count % 2 != 0)
            {
                runningTotal += (_tellTheTime.Now() - builds.Last().FinishDateTime).TotalMilliseconds;
            }

            return runningTotal;
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
