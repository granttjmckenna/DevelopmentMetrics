using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Builds
{
    public class Build
    {
        public Build() { }

        private readonly ITeamCityWebClient _teamCityWebClient;
        private readonly ITellTheTime _tellTheTime;

        [JsonProperty(PropertyName = "Build")]
        private List<Build> Builds { get; set; }

        public string ProjectId { get; set; }
        public string Name { get; set; }

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

        public static string CacheKey = "builds";

        public Build(ITeamCityWebClient teamCityWebClient, ITellTheTime tellTheTime)
        {
            _teamCityWebClient = teamCityWebClient;
            _tellTheTime = tellTheTime;
        }

        public List<Build> GetBuilds()
        {
            var builds = CacheHelper.GetObjectFromCache<List<Build>>(CacheKey, 60, GetBuildsFromRepo);

            return builds;
        }

        public List<Build> GetBuildsFromRepo()
        {
            var results = new List<Build>();

            foreach (var build in GetAllBuilds())
            {
                var buildDetails = GetBuildDetails(build.Href);

                results.Add(new Build
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
                    AgentName = buildDetails.Agent.Name
                });
            }

            return results;
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

            return buildDetails;
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
}

internal class Agent
{
    public string Name { get; set; }
}