using System.Linq;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildStabilityChart
    {
        private BuildStabilityViewModel _buildStabilityViewModel;

        public string ChartTitle = "Build stability";

        public string[] SeriesX { get; set; }

        public double[] SeriesY { get; set; }

        public BuildStabilityChart(BuildStabilityViewModel buildStabilityViewModel)
        {
            _buildStabilityViewModel = buildStabilityViewModel;

            SeriesX = buildStabilityViewModel.BuildFailureRate.Keys.ToArray();

            SeriesY = buildStabilityViewModel.BuildFailureRate.Values.ToArray();
        }
    }
}