using System.Collections.Generic;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class ProjectBuild
    {
        private readonly IBuildRepository _buildRepository;

        [JsonProperty(PropertyName = "Build")]
        public List<BuildDto> BuildList { get; set; }

        public ProjectBuild(IBuildRepository buildRepository)
        {
            _buildRepository = buildRepository;
        }

        public List<BuildDto> GetBuildsFor(string buildHref)
        {
            var returnedJson = _buildRepository.GetJsonFor(buildHref);

            return GetBuilds(returnedJson).BuildList;
        }

        private ProjectBuild GetBuilds(string returnedJson)
        {
            var projectBuild = JsonConvert.DeserializeObject<ProjectBuild>(returnedJson);

            return projectBuild;
        }
    }
}