using System;
using System.Collections.Generic;

namespace DevelopmentMetrics.Builds.Metrics
{
    public class BuildDeploymentMetric : IBuildMetric
    {
        private List<BuildDeploymentMetric> Results { get; } = new List<BuildDeploymentMetric>();

        public DateTime Date { get; internal set; }
        public double DeploymentIntervalTime { get; set; }
        public int DeploymentIntervalTimeStdDev { get; set; }
        public object DeploymentDurationTime { get; set; }
        public int DeploymentDurationTimeStdDev { get; set; }

        public BuildDeploymentMetric() { }

        public void SetDate(DateTime date)
        {
            Date = date;
        }

        public void Add(List<Build> builds)
        {
            throw new NotImplementedException();
        }

        public void Calculate()
        {
            throw new NotImplementedException();
        }

        public List<BuildDeploymentMetric> GetResults()
        {
            return Results;
        }
    }
}