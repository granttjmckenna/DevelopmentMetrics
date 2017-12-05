using System;

namespace DevelopmentMetrics
{
    public interface IMetric
    {
        DateTime Date { get; set; }
        double Rate { get; set; }
    }

    public class Metric : IMetric
    {
        public DateTime Date { get; set; }

        public double Rate { get; set; }
    }
}
