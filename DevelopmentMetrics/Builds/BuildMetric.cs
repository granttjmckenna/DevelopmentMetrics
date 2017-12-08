using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildMetric
    {
        private readonly List<Build> _builds;
        private readonly ITellTheTime _tellTheTime;

        public BuildMetric(List<Build> builds, ITellTheTime tellTheTime)
        {
            _tellTheTime = tellTheTime;
            _builds = builds;
        }

        public List<Metric> CalculateBuildFailingRateByWeekFor(int numberOfWeeks)
        {
            var results = new List<Metric>();

            var fromDate = GetFromDate(numberOfWeeks);

            var weeks = GetNumberOfWeeksFrom(fromDate);

            for (var x = 0; x < weeks; x++)
            {
                var startDate = fromDate.AddDays(x * 7);
                var endDate = startDate.AddDays(7);

                var selectedBuilds = _builds
                    .Where(b =>
                        b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)
                        && b.StartDateTime >= startDate
                        && b.StartDateTime < endDate)
                    .ToList();

                var total = selectedBuilds.Count;

                var failures = selectedBuilds.Count(b =>
                    b.Status.Equals(BuildStatus.Failure.ToString(), StringComparison.InvariantCultureIgnoreCase));

                var doubles = CalculateMillisecondsBetweenBuilds(GetAlternatingBuilds(selectedBuilds));

                results.Add(new Metric
                {
                    Date = startDate,
                    FailureRate = Calculator.Percentage(failures, total),
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
                var selectedBuilds = builds
                    .Where(b => b.BuildTypeId.Equals(buildTypeId))
                    .OrderBy(b => b.StartDateTime)
                    .ToList();

                var alternatingBuilds = new BuildMetric(builds, _tellTheTime).GetAlternatingBuilds(selectedBuilds);

                var collection = CalculateMillisecondsBetweenAlternatingBuilds(alternatingBuilds);

                if (collection.Any())
                    doubles.Add(collection.Average());
            }

            return doubles;
        }

        private List<Build> GetAlternatingBuilds(List<Build> builds)
        {
            var results = new List<Build>();

            var isPreviousBuildSuccess = true;

            foreach (var build in builds)
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

            if (builds.Count % 2 != 0)
            {
                doubles.Add((_tellTheTime.Now() - builds.Last().FinishDateTime).TotalMilliseconds);
            }

            return doubles;
        }

        private int CalculateAverageRecoveryTimeInHoursFor(List<double> doubles)
        {
            if (!doubles.Any())
                return 0;

            return ConvertMillisecondsToHours(doubles.Average());
        }

        private int ConvertMillisecondsToHours(double milliseconds)
        {
            return (int)(Math.Round(TimeSpan.FromMilliseconds(milliseconds).TotalHours, 0));
        }

        private int GetNumberOfWeeksFrom(DateTime fromDate)
        {
            return (int)(_tellTheTime.Now() - fromDate).TotalDays / 7;
        }

        private DateTime GetFromDate(int numberOfWeeks)
        {
            switch (numberOfWeeks)
            {
                case -1:
                    return GetStartOfWeekFor(_tellTheTime.Today()).AddDays(6 * -7);
                case -2:
                    return GetStartOfWeekFor(_builds.Min(c => c.StartDateTime));
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
    }
}
