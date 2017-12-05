using System;

namespace DevelopmentMetrics
{
    public class Count : IMetric
    {
        public DateTime Date { get; set; }
        public int DoneTotal { get; set; }
        public int Total { get; set; }
        public double Rate { get; set; }
    }
}
