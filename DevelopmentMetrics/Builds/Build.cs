﻿using System;
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



            return (from projectDetail in GetProjectList()
                    let buildTypesHref = GetBuildTypesHref(projectDetail.Href)
                    let buildsHref = GetBuildsHref(buildTypesHref)
                    let builds = GetBuilds(buildsHref)
                    from build in builds.Builds
                    let buildDetails = GetBuildDetails(build.Href)
                    select new Build
                    {
                        ProjectId = projectDetail.Id,
                        Name = projectDetail.Name,
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
                    })
                .ToList();
        }

        private List<Build> GetAllBuilds()
        {
            var buildData = _teamCityWebClient.GetBuildData();

            return JsonConvert.DeserializeObject<Build>(buildData).Builds;
        }

        private Build GetBuilds(string buildsHref)
        {
            var data = _teamCityWebClient.GetBuildDataFor(buildsHref);

            return JsonConvert.DeserializeObject<Build>(data);
        }

        private string GetBuildsHref(string href)
        {
            var buildTypeData = _teamCityWebClient.GetBuildTypeDataFor(href);

            var buildType = JsonConvert.DeserializeObject<BuildType>(buildTypeData);

            return buildType.Builds.Href;
        }

        private string GetBuildTypesHref(string href)
        {
            var projectData = _teamCityWebClient.GetProjectDataFor(href);

            var project = JsonConvert.DeserializeObject<ProjectInternal>(projectData);

            return project.BuildTypes.BuildTypeList.First().Href; //TODO: this might need to be a collection of Hrefs
        }

        private List<ProjectDetail> GetProjectList()
        {
            var rootData = _teamCityWebClient.GetRootData();

            var projectDetails = JsonConvert.DeserializeObject<Root>(rootData);

            return projectDetails.Projects.ProjectList;
        }

        private BuildDetail GetBuildDetails(string href)
        {
            var data = _teamCityWebClient.GetBuildDetailsDataFor(href);

            var buildDetails = JsonConvert.DeserializeObject<BuildDetail>(data);

            return buildDetails;
        }
    }
}

internal class Root
{
    public Projects Projects { get; set; }
}

internal class Projects
{
    [JsonProperty(PropertyName = "Project")]
    public List<ProjectDetail> ProjectList { get; set; }
}

internal class ProjectInternal
{
    public BuildTypes BuildTypes { get; set; }
}

internal class BuildTypes
{
    [JsonProperty(PropertyName = "BuildType")]
    public List<BuildTypeItem> BuildTypeList { get; set; }
}

internal class BuildType
{
    public Builds Builds { get; set; }
}

internal class BuildTypeItem
{
    public string Href { get; set; }
}

internal class Builds
{
    public string Href { get; set; }
}

internal class ProjectDetail
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Href { get; set; }
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