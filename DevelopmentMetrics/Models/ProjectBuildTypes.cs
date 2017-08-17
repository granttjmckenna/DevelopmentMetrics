using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class ProjectBuildTypes
    {
        private readonly IBuildRepository _buildRepository;
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentProjectId { get; set; }
        public string Href { get; set; }
        public string WebUrl { get; set; }
        [JsonProperty(PropertyName = "BuildTypes")]
        public BuildTypesDto BuildTypes { get; set; }

        public ProjectBuildTypes(IBuildRepository buildRepository)
        {
            this._buildRepository = buildRepository;
        }

        public ProjectBuildTypes GetProjectBuildTypesFor(string href)
        {
            var buildJson = _buildRepository.GetDataFor(href);

            return JsonConvert.DeserializeObject<ProjectBuildTypes>(buildJson);
        }
    }
}
