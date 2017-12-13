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
    public class CardCountTests
    {
        private ITellTheTime _tellTheTime;
        private ICard _card;

        [SetUp]
        public void Setup()
        {
            _tellTheTime = Substitute.For<ITellTheTime>();
            _card = Substitute.For<ICard>();

            _tellTheTime.Now().Returns(new DateTime(2017, 10, 05));
            _card.GetCards().Returns(GetCards());
        }

        [Test]
        public void Return_card_count_by_status()
        {
            var countByStatus = new CardCount(_card, _tellTheTime).GetCountByStatus();

            Assert.That(countByStatus[CardStatus.Status.Todo], Is.EqualTo(4));
            Assert.That(countByStatus[CardStatus.Status.Doing], Is.EqualTo(2));
            Assert.That(countByStatus[CardStatus.Status.Done], Is.EqualTo(4));
            Assert.That(countByStatus[CardStatus.Status.Unassigned], Is.EqualTo(1));
        }

        [Test]
        public void Return_collection_of_count_by_day_for_all_cards()
        {
            var countByDays = new CardCount(_card, _tellTheTime).GetCardCountByDayFrom(4);

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).Total, Is.EqualTo(4));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).Total, Is.EqualTo(9));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).Total, Is.EqualTo(11));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).DoneTotal, Is.EqualTo(4));

            Assert.That(countByDays.All(c => c.Date != new DateTime(2017, 10, 06)));
        }

        [Test]
        public void Return_work_in_process_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var workInProcess = new CardCount(_card, _tellTheTime).GetInWorkInProcessCountFor(dateTime);

            Assert.That(workInProcess, Is.EqualTo(7));
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
