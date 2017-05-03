using System.Collections.Generic;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class Projects
    {
        public int Count { get; set; }

        [JsonProperty(PropertyName = "Project")]
        public List<ProjectDto> ProjectList { get; set; }
    }
}