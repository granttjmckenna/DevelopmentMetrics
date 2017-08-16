using System;

namespace DevelopmentMetrics.Models
{
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