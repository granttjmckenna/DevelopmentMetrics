namespace DevelopmentMetrics.Repository
{
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