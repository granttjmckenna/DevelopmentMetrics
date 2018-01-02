using System;

namespace DevelopmentMetrics.Builds
{
    public class BuildStabilityMetric
    {
        public DateTime Date { get; set; }
        public double FailureRate { get; set; }
        public int RecoveryTime { get; set; }
        public double RecoveryTimeStdDev { get; set; }
    }
}
