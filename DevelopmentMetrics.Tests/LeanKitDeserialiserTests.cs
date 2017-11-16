using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class LeanKitDeserialiserTests
    {
        [Test]
        public void Return_lanes_from_board_reply_data()
        {
            var leanKitWebClient = Substitute.For<ILeanKitWebClient>();

            leanKitWebClient.GetBoardData().Returns(GetJsonResponse());

            leanKitWebClient.GetCardDataFor(Arg.Any<int>()).Returns(@"{""CreatedDate"":""2017-10-01""}");

            var cards = new Card(leanKitWebClient).GetCards().ToList();

            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Todo)));
            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Doing)));
            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Done)));
        }

        private string GetJsonResponse()
        {
            const string filePath = @"C:\code\DevelopmentMetrics\DevelopmentMetrics.Tests\leankit json response.txt";
            string jsonResponse;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
                jsonResponse = streamReader.ReadToEnd();

            return jsonResponse;
        }
    }
}