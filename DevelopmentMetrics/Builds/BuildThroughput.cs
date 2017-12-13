﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildThroughput
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildThroughput(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        public List<BuildThroughputMetric> CalculateBuildIntervalByWeekFor(int numberOfWeeks)
        {
            var results = new List<BuildThroughputMetric>();

            var doubles = new List<double>();

            var builds = GetSuccessfulBuildStepBuilds();

            var fromDate = GetFromDate(numberOfWeeks);

            for (var x = 0; x < numberOfWeeks; x++)
            {
                var startDate = fromDate.AddDays(x * 7);

                var buildsForDateRange = GetBuildsForDateRange(builds, startDate);

                foreach (var buildType in new BuildType().GetDistinctBuildTypeIds(buildsForDateRange))
                {
                    var buildsByType = buildsForDateRange
                        .Where(b => b.BuildTypeId.Equals(buildType.BuildTypeId))
                        .ToList();

                    doubles.AddRange(GetTimeInMillisecondsBetweenBuildsFor(buildsByType));
                }

                results.Add(new BuildThroughputMetric()
                {
                    Date = startDate,
                    BuildIntervalTime = CalculateAverageBuildIntervalTimeInHoursFor(doubles),
                    BuildIntervalTimeStdDev =
                        Calculator.ConvertMillisecondsToHours(Calculator.CalculateStandardDeviation(doubles))
                });
            }

            return results;
        }

        private List<double> GetTimeInMillisecondsBetweenBuildsFor(List<Build> builds)
        {
            var results = new List<double>();

            for (var x = builds.Count - 1; x > 0; x--)
            {
                results.Add((builds[x].StartDateTime - builds[x - 1].StartDateTime).TotalMilliseconds);
            }

            return results;
        }

        private List<Build> GetSuccessfulBuildStepBuilds()
        {
            return _build.GetBuilds()
                .Where(
                    b => b.BuildTypeId.Contains("_01")
                         && b.Status.Equals(BuildStatus.Success.ToString())
                         && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private List<Build> GetBuildsForDateRange(List<Build> builds, DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            return builds
                .Where(b =>
                    b.StartDateTime >= startDate
                    && b.StartDateTime < endDate)
                .ToList();
        }

        private int CalculateAverageBuildIntervalTimeInHoursFor(List<double> doubles)
        {
            return doubles.Any()
                ? Calculator.ConvertMillisecondsToHours(doubles.Average())
                : 0;
        }

        private DateTime GetFromDate(int numberOfWeeks)
        {
            return GetStartOfWeekFor(_tellTheTime.Today()).AddDays(numberOfWeeks * -7);
        }

        private DateTime GetStartOfWeekFor(DateTime today)
        {
            var offset = (int)today.DayOfWeek * -1;

            return today.AddDays(offset);
        }
    }

    public class BuildThroughputMetric
    {
        public DateTime Date { get; set; }
        public int BuildIntervalTime { get; set; }
        public double BuildIntervalTimeStdDev { get; set; }
    }
}