using System.Collections.Generic;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildStabilityViewModel
    {
        public Dictionary<string, double> BuildFailureRate { get; set; }
    }
}