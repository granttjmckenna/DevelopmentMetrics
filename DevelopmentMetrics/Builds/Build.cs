using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Builds
{
    public class Build
    {
        private readonly ITeamCityWebClient _teamCityWebClient;

        [JsonProperty(PropertyName = "Build")]
        private List<Build> Builds { get; set; }

        public string ProjectId { get; set; }
        public string Name { get; set; }

        public int Id { get; set; }

        public string BuildTypeId { get; set; }

        public string Number { get; set; }

        public string Status { get; set; }

        public string State { get; set; }

        public DateTime StartDateTime { get; set; }

        //public DateTime FinishDateTime { get; set; }

        //public DateTime QueueDateTime { get; set; }

        public string Href { get; set; }

        private Build() { }

        public Build(ITeamCityWebClient teamCityWebClient)
        {
            _teamCityWebClient = teamCityWebClient;
        }

        //var buildDetail = new BuildDetail(_buildRepository).GetBuildDetailsDataFor(build.Href);

        public List<Build> GetBuilds()
        {
            var results = new List<Build>();

            foreach (var projectDetail in GetProjectList())
            {
                var buildTypesHref = GetBuildTypesHref(projectDetail.Href);

                var buildsHref = GetBuildsHref(buildTypesHref);

                var builds = JsonConvert.DeserializeObject<Build>(_teamCityWebClient.GetBuildDataFor(buildsHref));

                foreach (var build in builds.Builds)
                {
                    var buildDetails = _teamCityWebClient.GetBuildDetailsDataFor(build.Href);

                    results.Add(new Build
                    {
                        ProjectId = projectDetail.Id,
                        Name = projectDetail.Name,
                        Id = build.Id,
                        BuildTypeId = build.BuildTypeId,
                        Number = build.Number,
                        Status = build.Status,
                        State = build.State,
                        Href = build.Href,
                    });
                }
            }


            return results;





            return (from projectDetail in GetProjectList()
                    let buildTypeHref = GetBuildTypesHref(projectDetail.Href)
                    let buildsHref = GetBuildsHref(buildTypeHref)
                    let buildData = _teamCityWebClient.GetBuildDataFor(buildsHref)
                    let builds = JsonConvert.DeserializeObject<Build>(buildData)
                    from build in builds.Builds
                    select new Build
                    {
                        ProjectId = projectDetail.Id,
                        Name = projectDetail.Name,
                        Id = build.Id,
                        BuildTypeId = build.BuildTypeId,
                        Number = build.Number,
                        Status = build.Status,
                        State = build.State,
                        Href = build.Href
                    })
                .ToList();
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