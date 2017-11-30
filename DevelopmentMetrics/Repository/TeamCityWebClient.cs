namespace DevelopmentMetrics.Repository
{

    public interface ITeamCityWebClient
    {
        string GetBuildDataFor(string uri);
        string GetBuildTypeDataFor(string uri);
        string GetProjectDataFor(string uri);
        string GetRootData();
    }

    public class TeamCityWebClient : ITeamCityWebClient
    {
        public string GetBuildDataFor(string uri)
        {
            return null;
        }

        public string GetBuildTypeDataFor(string uri)
        {
            return null;
        }

        public string GetProjectDataFor(string uri)
        {
            return null;
        }

        public string GetRootData()
        {
            return null;
        }
    }
}