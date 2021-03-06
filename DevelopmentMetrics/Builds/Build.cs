﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Builds
{
    public interface IBuild
    {
        List<Build> GetBuilds();

        List<Build> GetSuccessfulBuildStepsContaining(string step);

        Build GetMatchingBuildStep(Build productionBuild);

        List<Build> GetMatchingProductionSteps(Build productionBuild);
    }

    public class Build : IBuild
    {
        public Build() { }

        private readonly ITeamCityWebClient _teamCityWebClient;
        private readonly ITellTheTime _tellTheTime;
        private readonly IBuildsToExclude _buildsToExclude;

        [JsonProperty(PropertyName = "Build")]
        private List<Build> Builds { get; set; }

        public int Id { get; set; }

        public string BuildTypeId { get; set; }

        public string Number { get; set; }

        public string Status { get; set; }

        public string State { get; set; }

        public string AgentName { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime FinishDateTime { get; set; }

        public DateTime QueueDateTime { get; set; }

        public string Href { get; set; }

        public int IgnoredTestCount { get; set; }

        public static string CacheKey = "builds";

        public Build(ITeamCityWebClient teamCityWebClient, ITellTheTime tellTheTime, IBuildsToExclude buildsToExclude)
        {
            _teamCityWebClient = teamCityWebClient;
            _tellTheTime = tellTheTime;
            _buildsToExclude = buildsToExclude;
        }

        public List<Build> GetBuilds()
        {
            var builds = CacheHelper.GetObjectFromCache<List<Build>>(CacheKey, 60, GetBuildsFromRepo);

            return builds;
        }

        public List<Build> GetSuccessfulBuildStepsContaining(string step)
        {
            var build = GetBuilds();

            return build
                .Where(
                    b => b.BuildTypeId.Contains(step)
                         && b.Status.Equals(BuildStatus.Success.ToString(), StringComparison.InvariantCultureIgnoreCase)
                         && b.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        public Build GetMatchingBuildStep(Build productionBuild)
        {
            var buildStep = GetMatchingBuilds("01", productionBuild.BuildTypeId)
                .FirstOrDefault(b => b.Number == productionBuild.Number);

            return buildStep;
        }

        public List<Build> GetMatchingProductionSteps(Build productionBuild)
        {
            var productionSteps = GetMatchingBuilds("Production", productionBuild.BuildTypeId);

            return productionSteps;
        }

        private List<Build> GetMatchingBuilds(string match, string buildTypeId)
        {
            return GetSuccessfulBuildStepsContaining(match)
                .Where(b =>
                    b.BuildTypeId.StartsWith(new BuildGroup(buildTypeId).BuildTypeGroup, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(b => b.StartDateTime)
                .ToList();
        }

        private List<Build> GetBuildsFromRepo()
        {
            var allBuilds = GetAllBuilds()
                .Where(b =>
                    !_buildsToExclude.Builds()
                        .Contains(new BuildGroup(b.BuildTypeId).BuildTypeGroup))
                .ToList();

            return (from build in allBuilds
                    let buildDetails = GetBuildDetails(build.Href)
                    select new Build
                    {
                        Id = build.Id,
                        BuildTypeId = build.BuildTypeId,
                        Number = build.Number,
                        Status = build.Status,
                        State = build.State,
                        Href = build.Href,
                        StartDateTime = _tellTheTime.ParseBuildDetailDateTimes(buildDetails.StartDateTime),
                        FinishDateTime = _tellTheTime.ParseBuildDetailDateTimes(buildDetails.FinishDateTime),
                        QueueDateTime = _tellTheTime.ParseBuildDetailDateTimes(buildDetails.QueuedDateTime),
                        AgentName = buildDetails.Agent.Name,
                        IgnoredTestCount = buildDetails.IgnoredTestCount
                    })
                .ToList();
        }

        private List<Build> GetAllBuilds()
        {
            var buildData = _teamCityWebClient.GetBuildData();

            return JsonConvert.DeserializeObject<Build>(buildData).Builds;
        }

        private BuildDetail GetBuildDetails(string href)
        {
            var data = _teamCityWebClient.GetBuildDetailsDataFor(href);

            var buildDetails = JsonConvert.DeserializeObject<BuildDetail>(data);

            buildDetails.IgnoredTestCount = GetBuildIgnoredTestsCountFor(buildDetails.Statistics.Href);

            return buildDetails;
        }

        private int GetBuildIgnoredTestsCountFor(string statisticsHref)
        {
            var data = _teamCityWebClient.GetBuildStatisticsFor(statisticsHref);

            var buildStatistics = JsonConvert.DeserializeObject<BuildStatistics>(data);

            return (from prop in buildStatistics.Property
                    where prop.Values.First() == "IgnoredTestCount"
                    select int.Parse(prop.Values.Last()))
                    .FirstOrDefault();
        }
    }
}

internal class BuildDetail
{
    public Agent Agent { get; set; }

    [JsonProperty(PropertyName = "startDate")]
    public string StartDateTime { get; set; }

    [JsonProperty(PropertyName = "finishDate")]
    public string FinishDateTime { get; set; }

    [JsonProperty(PropertyName = "queuedDate")]
    public string QueuedDateTime { get; set; }

    public Statistics Statistics { get; set; }

    public int IgnoredTestCount { get; set; }
}

internal class Agent
{
    public string Name { get; set; }
}

internal class Statistics
{
    public string Href { get; set; }
}

internal class BuildStatistics
{
    public IEnumerable<IDictionary<string, string>> Property { get; set; }

    public int IgnoredTestCount { get; set; }
}
