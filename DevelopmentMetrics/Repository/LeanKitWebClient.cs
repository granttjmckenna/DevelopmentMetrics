using System.IO;
using System.Net;

namespace DevelopmentMetrics.Repository
{
    public interface ILeanKitWebClient
    {
        string GetBoardData();

        string GetCardDataFor(int cardId);
    }

    public class LeanKitWebClient : ILeanKitWebClient
    {
        private readonly IWebClient _webClient;
        private const string ApiDomain = "https://ehl.leankit.com/kanban/api";
        private const int BoardId = 311598445;

        public LeanKitWebClient(IWebClient webClient)
        {
            _webClient = webClient;
        }

        public string GetBoardData()
        {
            var url = $"{ApiDomain}/boards/{BoardId}";

            return ExecuteGetRequest(url);
        }

        public string GetCardDataFor(int cardId)
        {
            var url = $"{ApiDomain}/board/{BoardId}/GetCard/{cardId}";

            return ExecuteGetRequest(url);
        }

        private string ExecuteGetRequest(string url)
        {
            return _webClient.Get(url);
        }

        private string Get(string url)
        {
            var result = string.Empty;

            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json; charset=utf-8;";

            webRequest.Headers[HttpRequestHeader.Authorization] =
                "Basic Z3JhbnQubWNrZW5uYUBlbmVyZ3loZWxwbGluZS5jb206TWFudXRkMDE=";

            using (var webResponse = webRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null)
                        return result;

                    var streamReader = new StreamReader(responseStream);

                    result = streamReader.ReadToEnd();
                }
            }

            return result;
        }
    }
}