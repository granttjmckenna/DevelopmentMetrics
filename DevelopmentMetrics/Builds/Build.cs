using System;
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

        private List<Build> GetBuildsFromRepo()
        {
            var results = new List<Build>();

            var allBuilds = GetAllBuilds().Where(b =>
                    !_buildsToExclude.Builds()
                        .Contains(b.BuildTypeId.Substring(0,
                            b.BuildTypeId.IndexOf("_", StringComparison.InvariantCultureIgnoreCase))))
                .ToList();

            foreach (var build in allBuilds)
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