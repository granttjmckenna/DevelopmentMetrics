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
        private readonly ITellTheTime _tellTheTime;

        public DateTime Date { get; set; }

        public double FailureRate { get; set; }

        public BuildMetric(List<Build> builds, ITellTheTime tellTheTime)
        {
            _tellTheTime = tellTheTime;
            _builds = builds;
        }

        public List<BuildMetric> CalculateBuildFailingRateByWeekFor( int numberOfWeeks)
        {
            var results = new List<BuildMetric>();

            var fromDate = GetFromDate(numberOfWeeks);

            for (var x = 0; x < numberOfWeeks; x++)
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

                results.Add(new BuildMetric
                {
                    Date = startDate,
                    FailureRate = Calculator.Percentage(failures, total)
                });
            }

            return results;
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

        //public List<BuildMetric> CalculateBuildFailingRateByMonthFrom(DateTime fromDate)

        //{

        //    var results = new List<BuildMetric>();


        //    for (var i = 0; i < 12; i++)

        //    {

        //        var queryDate = fromDate.AddMonths(i);


        //        var monthBuildMetrics =

        //            _builds

        //                .Where(b => b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase)

        //                            && b.StartDateTime.Month.Equals(queryDate.Month)

        //                            && b.StartDateTime.Year.Equals(queryDate.Year))

        //                .ToList();


        //        if (!monthBuildMetrics.Any())

        //            continue;


        //        var total = monthBuildMetrics.Count();


        //        var failing = monthBuildMetrics

        //            .Count(b => b.Status.Equals(Helpers.BuildStatus.Failure.ToString(), StringComparison.CurrentCultureIgnoreCase));


        //        var failingRate = Calculator.Percentage(failing, total);


        //        results.Add(new BuildMetric

        //        {

        //            BuildMonth = queryDate.ToString("MMM-yyyy"),

        //            FailureRate = failingRate

        //        });

        //    }


        //    return results;

        //}
    }
}
