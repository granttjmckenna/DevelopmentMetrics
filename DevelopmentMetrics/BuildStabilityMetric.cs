using System;

namespace DevelopmentMetrics
{
    public class BuildStabilityMetric
    {
        public DateTime Date { get; set; }
        public double FailureRate { get; set; }
        public int RecoveryTime { get; set; }
        public double RecoveryTimeStdDev { get; set; }
    }
}
