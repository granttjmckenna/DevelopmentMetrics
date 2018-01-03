using System;
using System.Collections.Generic;

namespace DevelopmentMetrics.Builds.Metrics
{
    public interface IBuildMetric
    {
        void SetDate(DateTime date);
        void Add(List<Build> builds);
        void Calculate();
    }

    public class BuildMetric : IBuildMetric
    {
        public void SetDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void Add(List<Build> builds)
        {
            throw new NotImplementedException();
        }

        public void Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
