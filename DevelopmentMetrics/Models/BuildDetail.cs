using System;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Models
{
    public class BuildDetail
    {
        private readonly IBuildRepository _buildRepository;
        public int Id { get; set; }

        public string BuildTypeId { get; set; }

        public Agent Agent { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime FinishDateTime { get; set; }

        public DateTime QueuedDateTime { get; set; }

        public string State { get; set; }

        public string Status { get; set; }

        public BuildDetail(IBuildRepository buildRepository)
        {
            _buildRepository = buildRepository;
        }

        public BuildDetail GetDetails(string returnedJson)
        {
            var buildDetail = JsonConvert.DeserializeObject<BuildDetail>(returnedJson);

            return buildDetail;
        }

        public BuildDetail GetBuildDetailsFor(string buildHref)
        {
            var returnedJson = _buildRepository.GetProjectBuild();

            return GetDetails(returnedJson);
        }
    }
}