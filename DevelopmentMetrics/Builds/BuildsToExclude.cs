using System.Collections.Generic;

namespace DevelopmentMetrics.Builds
{
    public interface IBuildsToExclude
    {
        List<string> Builds();
    }

    public class BuildsToExclude : IBuildsToExclude
    {
        public List<string> Builds()
        {
            return new List<string> {"CodemanshipDrivingTest", "PoolLeague", "Forks", "Core" }; //Core is Core_Update_Fri_References
        }
    }
}