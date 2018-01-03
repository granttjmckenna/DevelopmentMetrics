using System;

namespace DevelopmentMetrics.Builds.Metrics
{
    public class BuildDeploymentMetric
    {
        public DateTime Date { get; internal set; }
        public double DeploymentIntervalTime { get; set; }
        public int DeploymentIntervalTimeStdDev { get; set; }
        public object DeploymentDurationTime { get; set; }
        public int DeploymentDurationTimeStdDev { get; set; }
    }
}