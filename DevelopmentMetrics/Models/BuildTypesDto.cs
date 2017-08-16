using System.Collections.Generic;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class BuildTypesDto
    {
        public int Count { get; set; }

        [JsonProperty(PropertyName = "BuildType")]
        public List<BuildTypeDto> BuildTypeList { get; set; }
    }
}
