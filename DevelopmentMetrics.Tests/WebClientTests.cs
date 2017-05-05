using System;
using System.IO;
using System.Net;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class WebClientTests
    {
        private WebClient _webClient;

        [SetUp]
        public void SetUp()
        {
            _webClient = new WebClient();
        }

        [Test]
        public void Should_add_domain_name_to_relative_url()
        {
            var absoluteUrl = GetAsbsoluteUrlFor("relative-url-part");

            Assert.That(absoluteUrl, Is.EqualTo("http://domain/relative-url-part"));
        }

        [TestCase("relative-url-part", "http://domain/relative-url-part?count=1000")]
        [TestCase("relative-url-part?page=1", "http://domain/relative-url-part?page=1&count=1000")]
        public void Should_add_count_to_absolute_url(string relativeUrl, string expected)
        {
            var absoluteUrl = GetAsbsoluteUrlFor(relativeUrl);

            var actual = GetUrlWithQueryStringCountOf(absoluteUrl, 1000);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string GetUrlWithQueryStringCountOf(string url, int cnt)
        {
            return (url.IndexOf("?", StringComparison.InvariantCultureIgnoreCase) > -1)
                ? $"{url}&count={cnt}"
                : $"{url}?count={cnt}";
        }

        private string GetAsbsoluteUrlFor(string relativeUrlPart)
        {
            return $"http://domain/{relativeUrlPart}";
        }
    }

    public class WebClient : IWebClient
    {
        public string Get(string url)
        {
            var result = string.Empty;
            var absoluteUrl = GetAsbsoluteUrlFor(url);

            var webRequest = WebRequest.Create(absoluteUrl);
            webRequest.Headers.Add(HttpRequestHeader.Accept, "application/json");

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

        private string GetAsbsoluteUrlFor(string relativeUrlPart)
        {
            var absoluteUrl = $"http://teamcity.energyhelpline.local/{relativeUrlPart}";

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

    public interface IWebClient
    {
        string Get(string url);
    }
}
