using System;

namespace DevelopmentMetrics
{
    public class Metric
    {
        public DateTime Date { get; set; }
        public double FailureRate { get; set; }
        public int RecoveryTime { get; set; }
    }
}
