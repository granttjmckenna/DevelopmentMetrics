using System;

namespace DevelopmentMetrics.Builds
{
    public class BuildThroughputMetric
    {
        public DateTime Date { get; set; }
        public int BuildIntervalTime { get; set; }
        public double BuildIntervalTimeStdDev { get; set; }
    }
}