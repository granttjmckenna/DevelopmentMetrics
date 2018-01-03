using System;
using System.Collections.Generic;
using DevelopmentMetrics.Builds.Metrics;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildDeployment
    {
        private IBuild _build;
        private ITellTheTime _tellTheTime;

        public BuildDeployment(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }
        public List<BuildDeploymentMetric> CalculateBuildDeploymentIntervalByWeekFor(BuildFilter buildFilter)
        {
            if (IsClearCache(buildFilter.NumberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            var results = new List<BuildDeploymentMetric>();

            var builds = _build.GetSuccessfulBuildStepsContaining("Production");

            var filteredBuilds = new FilterBuilds(builds).Filter(buildFilter);

            var fromDate = GetFromDate(buildFilter.NumberOfWeeks);

            for (var x = 0; x < buildFilter.NumberOfWeeks; x++)
            {
                var buildIntervals = new List<double>();
                var buildDurations = new List<double>();

                var startDate = fromDate.AddDays(x * 7);

                //var buildsForDateRange = GetBuildsForDateRange(filteredBuilds, startDate);

                //foreach (var buildType in new BuildType().GetDistinctBuildTypeIds(filteredBuilds))
                //{
                //    var buildsByType = buildsForDateRange
                //        .Where(b => b.BuildTypeId.Equals(buildType.BuildTypeId))
                //        .ToList();

                //    buildIntervals.AddRange(GetBuildIntervalInMillisecondsFor(buildsByType));

                //    buildDurations.AddRange(GetBuildDurationInMillisecondsFor(buildsByType));
                //}

                //results.Add(new BuildDeploymentMetric()
                //{
                //    Date = startDate,
                //    DeploymentIntervalTime = CalculateAverageTimeInHoursFor(buildIntervals),
                //    DeploymentIntervalTimeStdDev =
                //        Calculator.ConvertMillisecondsToHours(Calculator.CalculateStandardDeviation(buildIntervals)),
                //    DeploymentDurationTime = CalculateAverageTimeInMinutesFor(buildDurations),
                //    DeploymentDurationTimeStdDev =
                //        Calculator.ConvertMillisecondsToMinutes(Calculator.CalculateStandardDeviation(buildDurations))
                //});
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

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}