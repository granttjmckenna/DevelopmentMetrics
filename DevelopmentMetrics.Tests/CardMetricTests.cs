using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class CardMetricTests
    {
        private List<Card> _cards;
        private ITellTheTime _tellTheTime;

        [SetUp]
        public void Setup()
        {
            _cards = GetCards().ToList();
            _tellTheTime = Substitute.For<ITellTheTime>();
            _tellTheTime.Now().Returns(new DateTime(2017, 10, 05));
        }

        [TestCase(1, 4, ExpectedResult = 0.25d)]
        [TestCase(1, 3, ExpectedResult = 0.33d)]
        public double Return_percentage(int nominator, int denominator)
        {
            return Calculator.Percentage(nominator, denominator);
        }

        [Test]
        public void Return_defect_rate_for_given_day()
        {
            var calculationDate = new DateTime(2017, 10, 01);

            var cardCount = new CardCount(_tellTheTime, _cards).GetCardCountByDayFrom(calculationDate);

            Assert.That(cardCount.First(c => c.Date == calculationDate).DefectRate, Is.EqualTo(0.25d));
        }

        private static IEnumerable<Card> GetCards()
        {
            var cards = new List<Card>
            {
                new Card
                {
                    Id = 1,
                    Status = CardStatus.Status.Todo,
                    CreateDate = new DateTime(2017, 10, 01),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 2,
                    Status = CardStatus.Status.Todo,
                    CreateDate = new DateTime(2017, 10, 02),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 3,
                    Status = CardStatus.Status.Todo,
                    CreateDate = new DateTime(2017, 10, 02),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 4,
                    Status = CardStatus.Status.Todo,
                    CreateDate = new DateTime(2017, 10, 03),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 5,
                    Status = CardStatus.Status.Done,
                    CreateDate = new DateTime(2017, 10, 01),
                    TypeName = "Defect"
                },
                new Card
                {
                    Id = 6,
                    Status = CardStatus.Status.Done,
                    CreateDate = new DateTime(2017, 10, 01),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 7,
                    Status = CardStatus.Status.Done,
                    CreateDate = new DateTime(2017, 10, 01),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 8,
                    Status = CardStatus.Status.Done,
                    CreateDate = new DateTime(2017, 10, 03),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 9,
                    Status = CardStatus.Status.Doing,
                    CreateDate = new DateTime(2017, 10, 02),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 10,
                    Status = CardStatus.Status.Doing,
                    CreateDate = new DateTime(2017, 10, 02),
                    TypeName = "New Feature"
                },
                new Card
                {
                    Id = 11,
                    Status = CardStatus.Status.Unassigned,
                    CreateDate = new DateTime(2017, 10, 02),
                    TypeName = "New Feature"
                },
            };

            return cards;
        }
    }
}
