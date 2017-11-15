using System.Linq;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class LeanKitDeserialiserTests
    {
        [Test]
        public void Return_lanes_from_board_reply_data()
        {
            var cards = new Card().GetCardsFromReplyData().ToList();

            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Todo)));
            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Doing)));
            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Done)));
        }
    }
}