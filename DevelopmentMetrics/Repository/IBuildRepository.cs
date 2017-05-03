namespace DevelopmentMetrics.Repository
{
    public interface IBuildRepository
    {
        string GetRoot();

        string GetProjectBuild();

        string GetJsonFor(string path);
    }
}