namespace DevelopmentMetrics.Builds
{
    public class BuildFilter
    {
        public BuildFilter(int numberOfWeeks,string buildAgent, string buildTypeId)
        {
            NumberOfWeeks = numberOfWeeks;
            BuildAgent = buildAgent;
            BuildTypeId = buildTypeId;
        }

        public string BuildAgent { get; }
        public string BuildTypeId { get; }
        public int NumberOfWeeks { get; }
    }
}