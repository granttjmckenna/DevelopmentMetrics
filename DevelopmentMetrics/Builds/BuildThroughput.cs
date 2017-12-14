using System;
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

            var builds = _build.GetSuccessfulBuildStepsContaining("_01");

            var fromDate = GetFromDate(numberOfWeeks);

            for (var x = 0; x < numberOfWeeks; x++)
            {
                var buildIntervals = new List<double>();
                var buildDurations = new List<double>();

                var startDate = fromDate.AddDays(x * 7);

                var buildsForDateRange = GetBuildsForDateRange(builds, startDate);

                foreach (var buildType in new BuildType().GetDistinctBuildTypeIds(buildsForDateRange))
                {
                    var buildsByType = buildsForDateRange
                        .Where(b => b.BuildTypeId.Equals(buildType.BuildTypeId))
                        .ToList();

                    buildIntervals.AddRange(GetBuildIntervalInMillisecondsFor(buildsByType));

                    buildDurations.AddRange(GetBuildDurationInMillisecondsFor(buildsByType));
                }

                results.Add(new BuildThroughputMetric()
                {
                    Date = startDate,
                    BuildIntervalTime = CalculateAverageTimeInHoursFor(buildIntervals),
                    BuildIntervalTimeStdDev =
                        Calculator.ConvertMillisecondsToHours(Calculator.CalculateStandardDeviation(buildIntervals)),
                    BuildDurationTime = CalculateAverageTimeInHoursFor(buildDurations),
                    BuildDurationTimeStdDev =
                        Calculator.ConvertMillisecondsToHours(Calculator.CalculateStandardDeviation(buildDurations))
                });
            }

            return results;
        }

        private List<double> GetBuildIntervalInMillisecondsFor(List<Build> builds)
        {
            var results = new List<double>();

            for (var x = builds.Count - 1; x > 0; x--)
            {
                results.Add((builds[x].StartDateTime - builds[x - 1].StartDateTime).TotalMilliseconds);
            }

            return results;
        }

        private List<double> GetBuildDurationInMillisecondsFor(List<Build> builds)
        {
            return builds
                .Select(build => (build.FinishDateTime - build.StartDateTime).TotalMilliseconds)
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

        private int CalculateAverageTimeInHoursFor(List<double> doubles)
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
}