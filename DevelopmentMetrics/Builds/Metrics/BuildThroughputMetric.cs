using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds.Metrics
{
    public class BuildThroughputMetric : IBuildMetric
    {
        private List<double> Intervals { get; } = new List<double>();
        private List<double> Durations { get; } = new List<double>();
        private List<BuildThroughputMetric> Results { get; } = new List<BuildThroughputMetric>();

        public DateTime Date { get; private set; }
        public int IntervalTime { get; private set; }
        public double IntervalTimeStdDev { get; private set; }
        public int DurationTime { get; private set; }
        public double DurationTimeStdDev { get; private set; }

        public BuildThroughputMetric() { }

        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public void Add(IBuild build, List<Build> builds)
        {
            Intervals.AddRange(GetIntervalsInMilliseconds(builds));
            Durations.AddRange(GetDurationsInMilliseconds(builds));
        }

        public void Calculate()
        {
            Results.Add(new BuildThroughputMetric
            {
                Date = Date,
                IntervalTime = CalculateAverageTimeInHoursFor(Intervals),
                IntervalTimeStdDev =
                Calculator.ConvertMillisecondsToHours(
                Calculator.CalculateStandardDeviation(Intervals)),
                DurationTime = CalculateAverageTimeInMinutesFor(Durations),
                DurationTimeStdDev =
                Calculator.ConvertMillisecondsToMinutes(
                Calculator.CalculateStandardDeviation(Durations))
            });

            Intervals.Clear();
            Durations.Clear();
        }

        public List<BuildThroughputMetric> GetResults()
        {
            return Results;
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