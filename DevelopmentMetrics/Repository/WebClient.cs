using System;
using System.IO;
using System.Net;

namespace DevelopmentMetrics.Repository
{
    public interface IWebClient
    {
        string Get(string url);
    }

    public class WebClient : IWebClient
    {
        public string Get(string url)
        {
            var result = string.Empty;

            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json; charset=utf-8;";

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