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
    public class CardTests
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

        private static IEnumerable<Card> GetCards()
        {
            var cards = new List<Card>
            {
                new Card { Id = 1, Status = CardStatus.Status.Todo,CreateDate = new DateTime(2017,10,01)},
                new Card { Id = 2, Status = CardStatus.Status.Todo,CreateDate = new DateTime(2017,10,02) },
                new Card { Id = 3, Status = CardStatus.Status.Todo,CreateDate = new DateTime(2017,10,02) },
                new Card { Id = 4, Status = CardStatus.Status.Todo,CreateDate = new DateTime(2017,10,03) },
                new Card { Id = 5, Status = CardStatus.Status.Done,CreateDate = new DateTime(2017,10,01) },
                new Card { Id = 6, Status = CardStatus.Status.Done,CreateDate = new DateTime(2017,10,01) },
                new Card { Id = 7, Status = CardStatus.Status.Done,CreateDate = new DateTime(2017,10,01) },
                new Card { Id = 8, Status = CardStatus.Status.Done,CreateDate = new DateTime(2017,10,03) },
                new Card { Id = 9, Status = CardStatus.Status.Doing,CreateDate = new DateTime(2017,10,02) },
                new Card { Id = 10, Status = CardStatus.Status.Doing,CreateDate = new DateTime(2017,10,02) },
                new Card { Id = 11, Status = CardStatus.Status.Unassigned,CreateDate = new DateTime(2017,10,02) },
            };

            return cards;
        }

        [Test]
        [Description("Card metric test")]
        public void Return_card_count_by_status()
        {
            var cardMetric = new CardMetric(_cards).GetCountByStatus();

            Assert.That(cardMetric[CardStatus.Status.Todo], Is.EqualTo(4));
            Assert.That(cardMetric[CardStatus.Status.Doing], Is.EqualTo(2));
            Assert.That(cardMetric[CardStatus.Status.Done], Is.EqualTo(4));
            Assert.That(cardMetric[CardStatus.Status.Unassigned], Is.EqualTo(1));
        }

        [Test]
        [Description("Card count metric test")]
        public void Return_card_count_for_all_cards_in_the_process_for_a_given_day()
        {
            var calculationDate = new DateTime(2017, 10, 01);

            var cardCount = new CardCount(_tellTheTime, _cards).GetCardCountByDayFrom(calculationDate);

            Assert.That(cardCount.First(c => c.Date == calculationDate).Total, Is.EqualTo(4));
        }

        [Test]
        [Description("Card metric test?")]
        public void Return_defect_rate_for_given_day()
        {
            var calculationDate = new DateTime(2017, 10, 01);

            var cardCount = new CardCount(_tellTheTime, _cards).GetCardCountByDayFrom(calculationDate);

            Assert.That(cardCount.First(c => c.Date == calculationDate).DefectRate, Is.EqualTo(25));
        }

        [Test]
        [Description("Card count metric test")]
        public void Return_card_count_for_done_cards_in_the_process_for_a_given_day()
        {
            var calculationDate = new DateTime(2017, 10, 01);

            var cardCount = new CardCount(_tellTheTime, _cards).GetCardCountByDayFrom(calculationDate);

            Assert.That(cardCount.First(c => c.Date == calculationDate).DoneTotal, Is.EqualTo(3));
        }

        [Test]
        [Description("Card count tests")]
        public void Return_collection_of_count_by_day_for_all_cards()
        {
            var countByDays = new CardCount(_tellTheTime, _cards).GetCardCountByDayFrom(new DateTime(2017, 10, 01));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).Total, Is.EqualTo(4));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).Total, Is.EqualTo(9));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).Total, Is.EqualTo(11));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).DoneTotal, Is.EqualTo(4));

            Assert.That(countByDays.All(c => c.Date != new DateTime(2017, 10, 06)));
        }

        [Test]
        [Description("Card metric test")]
        public void Return_lead_time_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var leadTime = new CardMetric(_cards).CalculateLeadTimeFor(dateTime);

            Assert.That(leadTime, Is.EqualTo(2));
        }

        [Test]
        [Description("Card metric test")]
        public void Return_work_in_process_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var workInProcess = new CardMetric(_cards).CalculateWorkInProcessFor(dateTime);

            Assert.That(workInProcess, Is.EqualTo(7));
        }
    }
}
