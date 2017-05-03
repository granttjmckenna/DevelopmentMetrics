using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class RootProject
    {
        private readonly IBuildRepository _buildRepository;
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Href { get; set; }

        public string WebUrl { get; set; }

        public Projects Projects { get; set; }

        public RootProject(IBuildRepository repository)
        {
            _buildRepository = repository;
        }

        public List<BuildMetric> GetBuildMetrics()
        {
            var rootProjectJson = _buildRepository.GetRoot();

            var rootProject = GetProject(rootProjectJson);

            return (from project in rootProject.Projects.ProjectList
                    let builds = new ProjectBuild(_buildRepository).GetBuildsFor(project.Href)
                    from build in builds
                    let buildDetail = new BuildDetail(_buildRepository).GetBuildDetailsFor(build.Href)
                    select new BuildMetric
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        BuildTypeId = build.BuildTypeId,
                        BuildId = buildDetail.Id,
                        //StartDateTime = DateTime.Parse(buildDetail.StartDateTime),
                        //FinishDateTime = DateTime.Parse(buildDetail.FinishDateTime),
                        //QueueDateTime = DateTime.Parse(buildDetail.QueuedDateTime),
                        State = buildDetail.State,
                        Status = buildDetail.Status,
                        AgentName = buildDetail.AgentDto.Name,

                    })
                .ToList();
        }

        public RootProject GetProject(string returnedJson)
        {
            var project = JsonConvert.DeserializeObject<RootProject>(returnedJson);

            return project;
        }

        public override bool Equals(object obj)
        {
            var other = obj as RootProject;

            if (other == null)
                return false;

            return Id.Equals(other.Id, StringComparison.InvariantCultureIgnoreCase)
                   && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase)
                   && Description.Equals(other.Description, StringComparison.InvariantCultureIgnoreCase)
                   && Href.Equals(other.Href, StringComparison.InvariantCultureIgnoreCase)
                   && WebUrl.Equals(other.WebUrl, StringComparison.InvariantCultureIgnoreCase)
                   && Projects.Count == other.Projects.Count;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public class BuildMetric
    {
        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string BuildTypeId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime FinishDateTime { get; set; }

        public DateTime QueueDateTime { get; set; }

        public string State { get; set; }

        public string Status { get; set; }

        public string AgentName { get; set; }

        public int BuildId { get; set; }
    }
}