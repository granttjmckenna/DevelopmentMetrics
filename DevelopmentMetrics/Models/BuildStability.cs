using System.Collections.Generic;
using DevelopmentMetrics.Repository;

namespace DevelopmentMetrics.Models
{
    public interface IBuildStability
    {
        List<BuildMetric> GetBuildStabilityMetrics();
    }

    public class BuildStability : IBuildStability
    {
        private readonly IBuildRepository _buildRepository;

        public BuildStability(IBuildRepository repository)
        {
            _buildRepository = repository;
        }

        public List<BuildMetric> GetBuildStabilityMetrics()
        {
            return new Project(_buildRepository).GetBuildMetrics();
        }
    }
}
