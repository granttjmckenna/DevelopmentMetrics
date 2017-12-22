using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildMetric
    {
        private readonly List<double> _intervals;
        private readonly List<double> _durations;

        public DateTime Date { get; private set; }
        public int IntervalTime { get; private set; }
        public double IntervalTimeStdDev { get; private set; }
        public int DurationTime { get; private set; }
        public double DurationTimeStdDev { get; private set; }

        public BuildMetric(DateTime startDate)
        {
            Date = startDate;
            _intervals = new List<double>();
            _durations = new List<double>();
        }

        public void Add(List<Build> builds)
        {
            _intervals.AddRange(GetIntervalsInMilliseconds(builds));
            _durations.AddRange(GetDurationsInMilliseconds(builds));

        }

        public BuildMetric Calculate()
        {
            IntervalTime = CalculateAverageTimeInHoursFor(_intervals);
            IntervalTimeStdDev = 
                Calculator.ConvertMillisecondsToHours(
                    Calculator.CalculateStandardDeviation(_intervals));
            DurationTime = CalculateAverageTimeInMinutesFor(_durations);
            DurationTimeStdDev =
                Calculator.ConvertMillisecondsToMinutes(
                    Calculator.CalculateStandardDeviation(_durations));

            return this;
        }

        private List<double> GetIntervalsInMilliseconds(List<Build> builds)
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

        private List<double> GetDurationsInMilliseconds(List<Build> builds)
        {
            return builds
                .Select(build => (build.FinishDateTime - build.StartDateTime).TotalMilliseconds)
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
    }
}
