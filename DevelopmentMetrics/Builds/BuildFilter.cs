namespace DevelopmentMetrics.Builds
{
    public class BuildFilter
    {
        public BuildFilter(string buildAgent, string buildTypeId)
        {
            BuildAgent = buildAgent;
            BuildTypeId = buildTypeId;
        }

        public string BuildAgent { get; }

        public string BuildTypeId { get; }
    }
}