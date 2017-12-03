using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class Project
    {
        private readonly IBuildRepository _buildRepository;
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Href { get; set; }

        public string WebUrl { get; set; }

        public ProjectsDto Projects { get; set; }

        public Project(IBuildRepository repository)
        {
            _buildRepository = repository;
        }

        public List<BuildMetric> GetBuildMetrics()
        {
            var buildMetrics = Helpers.CacheHelper.GetObjectFromCache<List<BuildMetric>>("buildMetrics", 20,
                GetBuildMetricsFromRepo);

            return buildMetrics;
        }

        private List<BuildMetric> GetBuildMetricsFromRepo()
        {
            var buildMetrics = new List<BuildMetric>();

            var rootProject = GetProject();

            //projects
            foreach (var project in rootProject.Projects.ProjectList.Where(p => p.Name != "_root"))
            {
                //build types
                var buildTypes = new ProjectBuildTypes(_buildRepository).GetProjectBuildTypesFor(project.Href);

                if (buildTypes != null)
                {
                    foreach (var buildType in buildTypes.BuildTypes.BuildTypeList)
                    {
                        //builds
                        var builds = new ProjectBuild(_buildRepository).GetBuildsFor(buildType.Href + "/builds"); //TODO: use builds property for url

                        if (builds != null)
                        {
                            foreach (var build in builds)
                            {
                                //build details
                                var buildDetail = new BuildDetail(_buildRepository).GetBuildDetailsFor(build.Href);

                                if (buildDetail != null)
                                {
                                    buildMetrics.Add(
                                        new BuildMetric
                                        {
                                            ProjectId = project.Id,
                                            ProjectName = project.Name,
                                            BuildTypeId = buildType.Id,
                                            BuildId = buildDetail.Id,
                                            StartDateTime = ParseDateTimeString(buildDetail.StartDateTime),
                                            FinishDateTime = ParseDateTimeString(buildDetail.FinishDateTime),
                                            QueueDateTime = ParseDateTimeString(buildDetail.QueuedDateTime),
                                            State = buildDetail.State,
                                            Status = buildDetail.Status,
                                            AgentName = buildDetail.AgentDto.Name,
                                        });
                                }
                            }
                        }
                    }


                }
            }

            return buildMetrics;

            //return (from project in rootProject.Projects.ProjectList.Where(p => p.Name != "_root")
            //        let buildJson = _buildRepository.GetDataFor(project.Href)
            //        let projectBuilds = JsonConvert.DeserializeObject<ProjectBuildTypes>(buildJson)
            //        where projectBuilds != null
            //        from buildType in projectBuilds.BuildTypes.BuildTypeList
            //        let buildDetail = new BuildDetail(_buildRepository).GetBuildDetailsDataFor(buildType.Href + "/builds")
            //        where buildDetail != null && buildDetail.Status.Equals("finished", StringComparison.InvariantCultureIgnoreCase)
            //        select new BuildMetric
            //        {
            //            ProjectId = project.Id,
            //            ProjectName = project.Name,
            //            BuildTypeId = buildType.Id,
            //            BuildId = buildDetail.Id,
            //            StartDateTime = ParseDateTimeString(buildDetail.StartDateTime),
            //            FinishDateTime = ParseDateTimeString(buildDetail.FinishDateTime),
            //            QueueDateTime = ParseDateTimeString(buildDetail.QueuedDateTime),
            //            State = buildDetail.State,
            //            Status = buildDetail.Status,
            //            AgentName = buildDetail.AgentDto.Name,
            //        })
            //    .ToList();
        }

        public Project GetProject()
        {
            var returnedJson = _buildRepository.GetDataFor("guestAuth/app/rest/projects/id:_root");

            var project = JsonConvert.DeserializeObject<Project>(returnedJson);

            return project;
        }

        private static DateTime ParseDateTimeString(string dateTimeString)
        {
            var newDateTimeString = dateTimeString
                .Replace("T", "")
                .Substring(0, dateTimeString.IndexOf("+", StringComparison.InvariantCultureIgnoreCase) - 1);

            return DateTime.ParseExact(newDateTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Project;

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
}