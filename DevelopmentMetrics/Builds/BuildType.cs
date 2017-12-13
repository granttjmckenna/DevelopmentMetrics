namespace DevelopmentMetrics.Builds
{
    public class BuildType
    {
        public string BuildTypeId { get; set; }

        public BuildGroup BuildGroup => new BuildGroup(BuildTypeId);

        public BuildType(string buildTypeId)
        {
            BuildTypeId = buildTypeId;
        }
    }
}