using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class BuildThroughputViewModel
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildThroughputViewModel(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }
    }
}