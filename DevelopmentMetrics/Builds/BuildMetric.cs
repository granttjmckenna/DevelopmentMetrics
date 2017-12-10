﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildMetric
    {
        private readonly ITellTheTime _tellTheTime;

        public BuildMetric(ITellTheTime tellTheTime)
        {
            _tellTheTime = tellTheTime;
        }

        public List<FailureRate> GetTopFiveFailingBuildsByRate(List<Build> builds)
        {
            return (
                    from buildTypeId in builds
                        .Select(b => b.BuildTypeId)
                        .Distinct()
                    let selectedBuilds = builds
                        .Where(
                            b => b.BuildTypeId.Equals(buildTypeId, StringComparison.InvariantCultureIgnoreCase)
                            && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                        .ToList()
                    select new FailureRate
                    {
                        BuildTypeId = buildTypeId,
                        Rate = new BuildMetric(_tellTheTime).CalculateBuildFailingRate(selectedBuilds)
                    })
                .OrderByDescending(b => b.Rate)
                .ThenBy(b => b.BuildTypeId)
                .Take(5)
                .ToList();
        }

        public List<FailureRate> GetTopFiveSucceedingBuildsByRate(List<Build> builds)
        {
            return (
                    from buildTypeId in builds
                        .Select(b => b.BuildTypeId)
                        .Distinct()
                    let selectedBuilds = builds
                        .Where(
                            b => b.BuildTypeId.Equals(buildTypeId, StringComparison.InvariantCultureIgnoreCase)
                                 && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                        .ToList()
                    select new FailureRate
                    {
                        BuildTypeId = buildTypeId,
                        Rate = new BuildMetric(_tellTheTime).CalculateBuildFailingRate(selectedBuilds)
                    })
                .OrderBy(b => b.Rate)
                .ThenBy(b => b.BuildTypeId)
                .Take(5)
                .ToList();
        }

        public List<Metric> CalculateBuildFailingRateByWeekFor(List<Build> builds, int numberOfWeeks)
        {
            var results = new List<Metric>();

            var fromDate = GetFromDate(builds, numberOfWeeks);

            var weeks = GetNumberOfWeeksFrom(fromDate);

            for (var x = 0; x < weeks; x++)
            {
                var startDate = fromDate.AddDays(x * 7);

                var finishedBuilds = GetFinishedBuildsForDateRange(builds, startDate);

                var doubles = CalculateMillisecondsBetweenBuilds(finishedBuilds);

                results.Add(new Metric
                {
                    Date = startDate,
                    FailureRate = CalculateFailureRateFor(finishedBuilds),
                    RecoveryTime = CalculateAverageRecoveryTimeInHoursFor(doubles),
                    RecoveryTimeStdDev = ConvertMillisecondsToHours(Calculator.CalculateStandardDeviation(doubles))
                });
            }

            return results;
        }

        public List<double> CalculateMillisecondsBetweenBuilds(List<Build> builds)
        {
            var doubles = new List<double>();

            foreach (var buildTypeId in GetDistinctBuildTypeIdsFrom(builds))
            {
                var buildTypeBuilds = GetBuildsFor(builds, buildTypeId);

                var alternatingBuilds = new BuildMetric(_tellTheTime).GetAlternatingBuilds(buildTypeBuilds);

                var collection = CalculateMillisecondsBetweenAlternatingBuilds(alternatingBuilds);

                doubles.AddRange(collection);
            }

            return doubles;
        }

        private static int GetFailureCountFor(List<Build> finishedBuilds)
        {
            return finishedBuilds.Count(b =>
                b.Status.Equals(BuildStatus.Failure.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        private static List<Build> GetFinishedBuildsForDateRange(List<Build> builds, DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            return builds
                .Where(b =>
                    b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)
                    && b.StartDateTime >= startDate
                    && b.StartDateTime < endDate)
                .ToList();
        }

        private static List<Build> GetBuildsFor(List<Build> builds, string buildTypeId)
        {
            return builds
                .Where(b => b.BuildTypeId.Equals(buildTypeId))
                .ToList();
        }

        private List<Build> GetAlternatingBuilds(List<Build> builds)
        {
            var orderedBuilds = builds.OrderBy(b => b.Id).ToList();

            var results = new List<Build>();

            var isPreviousBuildSuccess = true;

            foreach (var build in orderedBuilds)
            {
                if (build.Status.Equals(BuildStatus.Failure.ToString(), StringComparison.InvariantCultureIgnoreCase) && isPreviousBuildSuccess)
                {
                    results.Add(build);
                    isPreviousBuildSuccess = false;
                }
                else if (build.Status.Equals(BuildStatus.Success.ToString(), StringComparison.InvariantCultureIgnoreCase) && !isPreviousBuildSuccess)
                {
                    results.Add(build);
                    isPreviousBuildSuccess = true;
                }
            }

            return results;
        }

        private List<double> CalculateMillisecondsBetweenAlternatingBuilds(List<Build> builds)
        {
            var doubles = new List<double>();

            for (var x = 0; x < builds.Count - 1; x += 2)
            {
                doubles.Add((builds[x + 1].FinishDateTime - builds[x].StartDateTime).TotalMilliseconds);
            }

            return doubles;
        }

        public int CalculateAverageRecoveryTimeInHoursFor(List<double> doubles)
        {
            return !doubles.Any() ? 0 : ConvertMillisecondsToHours(doubles.Average());
        }

        public int ConvertMillisecondsToHours(double milliseconds)
        {
            return (int)(Math.Round(TimeSpan.FromMilliseconds(milliseconds).TotalHours, 0));
        }

        private int GetNumberOfWeeksFrom(DateTime fromDate)
        {
            return (int)(_tellTheTime.Now() - fromDate).TotalDays / 7;
        }

        private DateTime GetFromDate(List<Build> builds, int numberOfWeeks)
        {
            switch (numberOfWeeks)
            {
                case -1:
                    return GetStartOfWeekFor(_tellTheTime.Today()).AddDays(6 * -7);
                case -2:
                    return GetStartOfWeekFor(builds.Min(c => c.StartDateTime));
                default:
                    return GetStartOfWeekFor(_tellTheTime.Today()).AddDays(numberOfWeeks * -7);
            }
        }

        private DateTime GetStartOfWeekFor(DateTime today)
        {
            var offset = (int)today.DayOfWeek * -1;

            return today.AddDays(offset);
        }

        public List<string> GetDistinctBuildTypeIdsFrom(List<Build> builds)
        {
            return builds.OrderBy(b => b.BuildTypeId).Select(b => b.BuildTypeId).Distinct().ToList();
        }

        private double CalculateFailureRateFor(List<Build> finishedBuilds)
        {
            return Calculator.Percentage(GetFailureCountFor(finishedBuilds), finishedBuilds.Count);
        }

        private double CalculateBuildFailingRate(List<Build> builds)
        {
            return CalculateFailureRateFor(builds);
        }
    }
}
