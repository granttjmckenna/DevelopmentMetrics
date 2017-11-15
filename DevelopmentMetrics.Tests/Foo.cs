using System.IO;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class Foo
    {
        [Test]
        public void Get_fake_json_response()
        {
            var jsonResponse = GetJsonResponse();

            Assert.That(jsonResponse, Is.Not.Null);
        }

        [Test]
        public void Get_cards_collection_from_json_response()
        {
            
        }

        private string GetJsonResponse()
        {
            const string filePath = @"C:\code\DevelopmentMetrics\DevelopmentMetrics.Tests\leankit json response.txt";
            string jsonResponse = null;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
                jsonResponse = streamReader.ReadToEnd();

            return jsonResponse;
        }
    }
}
