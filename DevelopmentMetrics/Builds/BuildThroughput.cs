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

        public List<BuildThroughputMetric> CalculateBuildIntervalByWeekFor(BuildFilter buildFilter)
        {
            if (IsClearCache(buildFilter.NumberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            var results = new List<BuildThroughputMetric>();

            var builds = _build.GetSuccessfulBuildStepsContaining("_01");

            var filteredBuilds = new FilterBuilds(builds).Filter(buildFilter);

            var fromDate = GetFromDate(buildFilter.NumberOfWeeks);

            for (var x = 0; x < buildFilter.NumberOfWeeks; x++)
            {
                var buildIntervals = new List<double>();
                var buildDurations = new List<double>();

                var startDate = fromDate.AddDays(x * 7);

                var buildsForDateRange = GetBuildsForDateRange(filteredBuilds, startDate);

                foreach (var buildType in new BuildType().GetDistinctBuildTypeIds(filteredBuilds))
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
                    BuildDurationTime = CalculateAverageTimeInMinutesFor(buildDurations),
                    BuildDurationTimeStdDev =
                        Calculator.ConvertMillisecondsToMinutes(Calculator.CalculateStandardDeviation(buildDurations))
                });
            }

            return results;
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }

        private List<double> GetBuildIntervalInMillisecondsFor(List<Build> builds)
        {
            var buildsInDescendingOrder = builds.OrderByDescending(b => b.StartDateTime).ToList();

            var results = new List<double>();

            for (var x = 0; x < buildsInDescendingOrder.Count - 1; x++)
            {
                results.Add((buildsInDescendingOrder[x].StartDateTime
                             - buildsInDescendingOrder[x + 1].StartDateTime).TotalMilliseconds);
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

        private int CalculateAverageTimeInMinutesFor(List<double> doubles)
        {
            return doubles.Any()
                ? Calculator.ConvertMillisecondsToMinutes(doubles.Average())
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