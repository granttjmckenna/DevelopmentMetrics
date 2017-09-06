using System.Collections.Generic;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildStabilityMetrics
    {
        public Dictionary<string,double> BuildFailureRate { get; set; }
    }
}