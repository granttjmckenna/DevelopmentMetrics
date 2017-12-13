using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Builds
{
    public class BuildRate
    {
        public string BuildTypeId { get; set; }
        public double Rate { get; set; }
        public string DisplayRate => Display.PercentageAsString(Rate);
    }
}