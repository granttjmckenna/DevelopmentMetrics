using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildMetric
    {
        private BuildMetric() { }

        private readonly List<Build> _builds;

        public string BuildMonth { get; set; }

        public double FailureRate { get; set; }

        public BuildMetric(List<Build> builds)
        {
            _builds = builds;
        }

        public List<BuildMetric> CalculateBuildFailingRateByMonthFrom(DateTime fromDate)
        {
            var results = new List<BuildMetric>();

            for (var i = 0; i < 12; i++)
            {
                var queryDate = fromDate.AddMonths(i);

                var monthBuildMetrics =
                    _builds
                        .Where(b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)
                                    && b.StartDateTime.Month.Equals(queryDate.Month)
                                    && b.StartDateTime.Year.Equals(queryDate.Year))
                        .ToList();

                if (!monthBuildMetrics.Any())
                    continue;

                var total = monthBuildMetrics.Count();

                var failing = monthBuildMetrics
                    .Count(b => b.Status.Equals(Helpers.BuildStatus.Failure.ToString(), StringComparison.CurrentCultureIgnoreCase));

                var failingRate = Calculator.Percentage(failing, total);

                results.Add(new BuildMetric
                {
                    BuildMonth = queryDate.ToString("MMM-yyyy"),
                    FailureRate = failingRate
                });
            }

            return results;
        }

        public List<BuildMetric> CalculateBuildFailingRateByWeekFrom(DateTime fromDate)
        {
            return null;
        }
    }
}
