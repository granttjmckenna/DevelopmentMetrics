using System;

namespace DevelopmentMetrics.Builds
{
    public class BuildThroughputMetric
    {
        public DateTime Date { get; set; }
        public int BuildIntervalTime { get; set; }
        public double BuildIntervalTimeStdDev { get; set; }
        public int BuildDurationTime { get; set; }
        public double BuildDurationTimeStdDev { get; set; }
    }
}