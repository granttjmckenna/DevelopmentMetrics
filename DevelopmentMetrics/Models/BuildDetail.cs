﻿using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class BuildDetail
    {
        private readonly IBuildRepository _buildRepository;
        public int Id { get; set; }

        public string BuildTypeId { get; set; }

        [JsonProperty(PropertyName = "Agent")]
        public AgentDto AgentDto { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public string StartDateTime { get; set; }

        [JsonProperty(PropertyName = "finishDate")]
        public string FinishDateTime { get; set; }

        [JsonProperty(PropertyName = "queuedDate")]
        public string QueuedDateTime { get; set; }

        public string State { get; set; }

        public string Status { get; set; }

        public BuildDetail(IBuildRepository buildRepository)
        {
            _buildRepository = buildRepository;
        }

        public BuildDetail GetBuildDetailsFor(string buildUrl)
        {
            var returnedJson = _buildRepository.GetDataFor(buildUrl);

            return JsonConvert.DeserializeObject<BuildDetail>(returnedJson);
        }
    }
}