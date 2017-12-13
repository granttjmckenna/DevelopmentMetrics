using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildMetric
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly IBuild _build;

        public BuildMetric(ITellTheTime tellTheTime, IBuild build)
        {
            _tellTheTime = tellTheTime;
            _build = build;
        }

        public List<Metric> CalculateBuildFailingRateByWeekFor(BuildFilter buildFilter)
        {
            if (IsClearCache(buildFilter.NumberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            var builds = _build.GetBuilds();

            var filteredBuilds = new FilterBuilds(builds).Filter(buildFilter);

            var results = new List<Metric>();

            var fromDate = GetFromDate(builds, buildFilter.NumberOfWeeks);

            var weeks = GetNumberOfWeeksFrom(fromDate);

            for (var x = 0; x < weeks; x++)
            {
                var startDate = fromDate.AddDays(x * 7);

                var finishedBuilds = GetFinishedBuildsForDateRange(filteredBuilds, startDate);

                var doubles = CalculateMillisecondsBetweenBuilds(finishedBuilds);

                results.Add(new Metric
                {
                    Date = startDate,
                    FailureRate = CalculateFailureRateFor(finishedBuilds),
                    RecoveryTime = CalculateAverageRecoveryTimeInHoursFor(doubles),
                    RecoveryTimeStdDev = Calculator.ConvertMillisecondsToHours(Calculator.CalculateStandardDeviation(doubles))
                });
            }

            return results;
        }

        public List<FailureRate> GetFailingBuildsByRate()
        {
            var builds = _build.GetBuilds();

            return GetFailureRatesFor(builds)
                .Where(b => b.Rate > 0.5d)
                .OrderByDescending(b => b.Rate)
                .ThenBy(b => b.BuildTypeId)
                .ToList();
        }

        public List<FailureRate> GetPassingBuildsByRate()
        {
            var builds = _build.GetBuilds();

            return GetFailureRatesFor(builds)
                .Where(b => b.Rate <= 0.5d)
                .OrderByDescending(b => b.Rate)
                .ThenBy(b => b.BuildTypeId)
                .ToList();
        }

        public List<BuildGroup> GetDistinctBuildGroups()
        {
            var results = new List<BuildGroup>();

            foreach (var buildGroup in _build.GetBuilds()
                .Select(b => new BuildGroup(b.BuildTypeId)))
            {
                if (!results.Exists(b => b.BuildTypeGroup.Equals(buildGroup.BuildTypeGroup)))
                    results.Add(buildGroup);
            };

            return results.OrderBy(b => b.BuildTypeGroup).ToList();
        }

        private List<BuildType> GetDistinctBuildTypeIds(List<Build> builds)
        {
            return builds
                .OrderBy(b => b.BuildTypeId)
                .Select(b => b.BuildTypeId)
                .Distinct()
                .Select(buildTypeId => new BuildType(buildTypeId))
                .ToList();
        }

        private List<double> CalculateMillisecondsBetweenBuilds(List<Build> builds)
        {
            var doubles = new List<double>();

            foreach (var buildType in GetDistinctBuildTypeIds(builds))
            {
                var buildTypeBuilds = GetBuildsFor(builds, buildType.BuildTypeId);

                var alternatingBuilds = GetAlternatingBuilds(buildTypeBuilds);

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

        private int CalculateAverageRecoveryTimeInHoursFor(List<double> doubles)
        {
            return doubles.Any()
                ? Calculator.ConvertMillisecondsToHours(doubles.Average())
                : 0;
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

        private double CalculateFailureRateFor(List<Build> finishedBuilds)
        {
            return Calculator.Percentage(GetFailureCountFor(finishedBuilds), finishedBuilds.Count);
        }

        private double CalculateBuildFailingRate(List<Build> builds)
        {
            return CalculateFailureRateFor(builds);
        }

        private IEnumerable<FailureRate> GetFailureRatesFor(List<Build> builds)
        {
            return from buildTypeId in builds
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
                       Rate = CalculateBuildFailingRate(selectedBuilds)
                   };
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }

    public class BuildGroup
    {
        public string BuildTypeGroup { get; }

        public string BuildTypeGroupDisplay => Display.ConvertCamelCaseString(BuildTypeGroup);

        public BuildGroup(string buildTypeId)
        {
            BuildTypeGroup = buildTypeId.Substring(0, buildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
