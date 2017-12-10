using System;

namespace DevelopmentMetrics.Builds
{
    public class FailureRate
    {
        public string BuildTypeId { get; set; }
        public double Rate { get; set; }
        public string DisplayRate => Math.Round(Rate * 100, 0) + "%";
    }
}