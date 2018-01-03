using System;
using System.Collections.Generic;

namespace DevelopmentMetrics.Builds.Metrics
{
    public interface IBuildMetric
    {
        void SetDate(DateTime date);
        void Add(IBuild build, List<Build> builds);
        void Calculate();
    }
}
