﻿using System;
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
                        ProjectId = "Blah",
                        Name = "Blah",
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