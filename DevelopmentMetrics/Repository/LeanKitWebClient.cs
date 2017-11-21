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
        private const string ApiDomain = "https://ehl.leankit.com/kanban/api";
        private const int BoardId = 311598445;

        public string GetBoardData()
        {
            var url = $"{ApiDomain}/boards/{BoardId}";

            return Get(url);
        }

        public string GetCardDataFor(int cardId)
        {
            var url = $"{ApiDomain}/board/{BoardId}/GetCard/{cardId}";

            return Get(url);
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