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
                    b.Status.Equals("Failure", StringComparison.InvariantCultureIgnoreCase));

                /*
                if calculatemillisecondsbetweenbuilds returns a list of doubles
                then the list of doubles could be passed to the method
                GetAverageRecoveryTimeFor, which will convert the list into a
                single figure for RecoveryTime, and it could also be passed into
                the method for calculating standard deviation - giving two values
                from one list

                START with the test!
                */

                results.Add(new Metric
                {
                    Date = startDate,
                    FailureRate = Calculator.Percentage(failures, total),
                    RecoveryTime = GetAverageRecoveryTimeFor(selectedBuilds)
                });
            }

            return results;
        }

        private int GetAverageRecoveryTimeFor(List<Build> selectedBuilds)
        {
            if (!selectedBuilds.Any())
                return 0;

            var totalMilliseconds = GetTotalMillisecondsFor(selectedBuilds);

            var averageMilliseconds = totalMilliseconds / (double)selectedBuilds.Count;

            return (int)(Math.Round(TimeSpan.FromMilliseconds(averageMilliseconds).TotalHours, 0));
        }

        public double GetTotalMillisecondsFor(List<Build> builds)
        {
            var distinctBuildTypes = builds.Select(b => b.BuildTypeId).Distinct().ToList();

            var milliseconds = distinctBuildTypes.Sum(
                buildType => CalculateMillisecondsBetweenBuilds(
                    GetAlternatingBuilds(
                        builds.Where(b => b.BuildTypeId.Equals(buildType, StringComparison.InvariantCultureIgnoreCase))
                            .ToList())));

            return milliseconds;
        }

        public List<Build> GetAlternatingBuilds(List<Build> builds)
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

        public List<double> CalculateMillisecondsBetweenBuildsTwo()
        {
            var doubles = new List<double>();

            for (var x = 0; x < _builds.Count - 1; x += 2)
            {
                doubles.Add((_builds[x + 1].FinishDateTime - _builds[x].StartDateTime).TotalMilliseconds);
            }

            if (_builds.Count % 2 != 0)
            {
                doubles.Add((_tellTheTime.Now() - _builds.Last().FinishDateTime).TotalMilliseconds);
            }

            return doubles;
        }

        public double CalculateMillisecondsBetweenBuilds(List<Build> builds)
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
    }
}
