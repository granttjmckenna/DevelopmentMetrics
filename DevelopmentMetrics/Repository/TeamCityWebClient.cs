using System;

namespace DevelopmentMetrics.Repository
{

    public interface ITeamCityWebClient
    {
        string GetBuildDataFor(string uri);
        string GetBuildTypeDataFor(string uri);
        string GetProjectDataFor(string uri);
        string GetRootData();
        string GetBuildDetailsDataFor(string buildHref);
        string GetBuildData();
    }

    public class TeamCityWebClient : ITeamCityWebClient
    {
        private readonly IWebClient _webClient;

        public TeamCityWebClient(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public string GetBuildDataFor(string uri)
        {
            return ExecuteGetRequest(uri);
        }

        public string GetBuildTypeDataFor(string uri)
        {
            return ExecuteGetRequest(uri);
        }

        public string GetProjectDataFor(string uri)
        {
            return ExecuteGetRequest(uri);
        }

        public string GetRootData()
        {
            var uri = GetAbsoluteUrlWith("guestAuth/app/rest/projects/id:_root");

            return ExecuteGetRequest(uri);
        }

        public string GetBuildDetailsDataFor(string uri)
        {
            return ExecuteGetRequest(uri);
        }

        public string GetBuildData()
        {
            var uri = GetAbsoluteUrlWith("guestAuth/app/rest/builds?count=10000");

            return ExecuteGetRequest(uri);
        }

        private string ExecuteGetRequest(string uri)
        {
            return _webClient.Get(GetAbsoluteUrlWith(uri));
        }

        private string GetAbsoluteUrlWith(string href)
        {
            if (href.StartsWith("http://") || href.StartsWith("https://"))
            {
                return href;
            }

            var absoluteUrl = $"http://teamcity.energyhelpline.local/{href}";

            return GetUrlWithQueryStringCount(absoluteUrl);
        }

        private string GetUrlWithQueryStringCount(string url)
        {
            const int cnt = 1000;

            return (url.IndexOf("?", StringComparison.InvariantCultureIgnoreCase) > -1)
                ? $"{url}&count={cnt}"
                : $"{url}?count={cnt}";
        }
    }
}