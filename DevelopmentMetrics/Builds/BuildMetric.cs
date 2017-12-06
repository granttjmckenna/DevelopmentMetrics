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

                results.Add(new Metric
                {
                    Date = startDate,
                    Rate = Calculator.Percentage(failures, total)
                });
            }

            return results;
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
