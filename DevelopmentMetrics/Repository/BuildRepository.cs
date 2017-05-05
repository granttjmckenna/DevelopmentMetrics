namespace DevelopmentMetrics.Repository
{
    public interface IBuildRepository
    {
        string GetDataFor(string path);
    }

    public class BuildRepository : IBuildRepository
    {
        private readonly IWebClient _webClient;

        public BuildRepository(IWebClient webClient)
        {
            _webClient = webClient;
        }
        public string GetDataFor(string path)
        {
            return _webClient.Get(path);
        }
    }
}