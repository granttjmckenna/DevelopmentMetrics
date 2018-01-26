using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds.Metrics
{
    public class BuildDeploymentMetric : IBuildMetric
    {
        private List<double> Intervals { get; } = new List<double>();
        private List<double> Durations { get; } = new List<double>();
        private List<BuildDeploymentMetric> Results { get; } = new List<BuildDeploymentMetric>();

        public DateTime Date { get; internal set; }
        public int IntervalTime { get; set; }
        public int IntervalTimeStdDev { get; set; }
        public int DurationTime { get; set; }
        public int DurationTimeStdDev { get; set; }

        public BuildDeploymentMetric() { }

        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public void Add(IBuild build, List<Build> builds)
        {
            Intervals.AddRange(GetIntervalsInMilliseconds(builds));
            Durations.AddRange(GetDurationsInMilliseconds(build, builds));
        }

        public void Calculate()
        {
            Results.Add(new BuildDeploymentMetric
            {
                Date = Date,
                IntervalTime = CalculateAverageTimeInDaysFor(Intervals),
                IntervalTimeStdDev =
                    Calculator.ConvertMillisecondsToDays(
                        Calculator.CalculateStandardDeviation(Intervals)),
                DurationTime = CalculateAverageTimeInHoursFor(Durations),
                DurationTimeStdDev =
                    Calculator.ConvertMillisecondsToHours(
                        Calculator.CalculateStandardDeviation(Durations))
            });

            Intervals.Clear();
            Durations.Clear();
        }

        public List<BuildDeploymentMetric> GetResults()
        {
            return Results;
        }

        private List<double> GetIntervalsInMilliseconds(List<Build> builds)
        {
            var buildsInDescendingOrder = builds.OrderByDescending(b => b.Id).ToList();

            var results = new List<double>();
            
            for (var x = 0; x < buildsInDescendingOrder.Count - 1; x++)
            {
                results.Add((buildsInDescendingOrder[x].StartDateTime
                             - buildsInDescendingOrder[x + 1].StartDateTime).TotalMilliseconds);
            }

            return results;
        }

        private List<double> GetDurationsInMilliseconds(IBuild build, List<Build> builds)
        {
            return (from bld in builds
                    let buildStep = build.GetMatchingBuildStep(bld)
                    where buildStep != null
                    select (bld.FinishDateTime - buildStep.StartDateTime).TotalMilliseconds)
                    .ToList();
        }

        private int CalculateAverageTimeInDaysFor(List<double> doubles)
        {
            return doubles.Any()
                ? Calculator.ConvertMillisecondsToDays(doubles.Average())
                : 0;
        }

        private int CalculateAverageTimeInHoursFor(List<double> doubles)
        {
            return doubles.Any()
                ? Calculator.ConvertMillisecondsToHours(doubles.Average())
                : 0;
        }
    }
}