using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class CardTests
    {
        private List<Card> _cards;

        [SetUp]
        public void Setup()
        {
            _cards = GetCards().ToList();
        }

        [Test]
        [Description("Card metric test")]
        public void Return_card_count_by_status()
        {
            Assert.That(_cards.Count(x => x.Status.Equals(CardStatus.Status.Todo)), Is.EqualTo(4));
            Assert.That(_cards.Count(x => x.Status.Equals(CardStatus.Status.Doing)), Is.EqualTo(2));
            Assert.That(_cards.Count(x => x.Status.Equals(CardStatus.Status.Done)), Is.EqualTo(4));
        }

        [Test]
        [Ignore("Replaced by card count tests")]
        public void Return_card_count_for_all_cards_in_the_process_for_a_given_day()
        {
            Assert.That(GetCardCountsFor(c => c.CreatedDate <= new DateTime(2017, 10, 01)), Is.EqualTo(4));
            Assert.That(GetCardCountsFor(c => c.CreatedDate <= new DateTime(2017, 10, 02)), Is.EqualTo(8));
        }

        [Test]
        [Ignore("Replaced by card count tests")]
        public void Return_card_count_for_done_cards_in_the_process_for_a_given_day()
        {
            Assert.That(
                GetCardCountsFor(c => c.CreatedDate <= new DateTime(2017, 10, 01)
                                      && c.Status.Equals(CardStatus.Status.Done)), Is.EqualTo(3));

            Assert.That(
                GetCardCountsFor(c => c.CreatedDate <= new DateTime(2017, 10, 03)
                                      && c.Status.Equals(CardStatus.Status.Done)), Is.EqualTo(4));
        }

        [Test]
        [Description("Card metric test")]
        public void Return_lead_time_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var leadTime = CalculateLeadTimeFor(dateTime);

            Assert.That(leadTime, Is.EqualTo(2));
        }

        [Test]
        [Description("Card metric test")]
        public void Return_work_in_process_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var workInProcess = CalculateWorkInProcessFor(dateTime);

            Assert.That(workInProcess, Is.EqualTo(6));
        }

        [Test]
        [Description("Card count tests")]
        public void Return_collection_of_count_by_day_for_all_cards()
        {
            var countByDays = new CardCount(_cards).GetCardCountByDayFrom(new DateTime(2017, 10, 01));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).Total, Is.EqualTo(4));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).Total, Is.EqualTo(8));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).Total, Is.EqualTo(10));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).DoneTotal, Is.EqualTo(4));

            Assert.That(countByDays.All(c => c.Date != new DateTime(2017, 10, 04)));
        }

        private int CalculateWorkInProcessFor(DateTime dateTime)
        {
            var cardCount = _cards.Count(c => c.CreatedDate <= dateTime);

            var doneCardCount = _cards.Count(DonePredicateFor(dateTime));

            return cardCount - doneCardCount;
        }

        private int CalculateLeadTimeFor(DateTime dateTime)
        {
            var cardPosition = _cards
                .OrderBy(c => c.CreatedDate)
                .Count(DonePredicateFor(dateTime));

            var cardDate = _cards.OrderBy(c => c.CreatedDate).Take(cardPosition).Max(c => c.CreatedDate);

            return (dateTime - cardDate).Days;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime dateTime)
        {
            return c => c.CreatedDate <= dateTime && c.Status.Equals(CardStatus.Status.Done);
        }

        private int GetCardCountsFor(Func<Card, bool> func)
        {
            return _cards.Count(func);
        }

        private static IEnumerable<Card> GetCards()
        {
            var cards = new List<Card>
            {
                new Card { Id = 1, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,01)},
                new Card { Id = 2, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,02) },
                new Card { Id = 3, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,02) },
                new Card { Id = 4, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,03) },
                new Card { Id = 5, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,01) },
                new Card { Id = 6, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,01) },
                new Card { Id = 7, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,01) },
                new Card { Id = 8, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,03) },
                new Card { Id = 9, Status = CardStatus.Status.Doing,CreatedDate = new DateTime(2017,10,02) },
                new Card { Id = 10, Status = CardStatus.Status.Doing,CreatedDate = new DateTime(2017,10,02) },
            };

            return cards;
        }
    }

    public class CardCount
    {
        private readonly IEnumerable<Card> _cards;
        public DateTime Date { get; set; }
        public int DoneTotal { get; set; }
        public int Total { get; set; }

        public CardCount(){}

        public CardCount(IEnumerable<Card> cards)
        {
            _cards = cards;
        }

        public List<CardCount> GetCardCountByDayFrom(DateTime dateTime)
        {
            var maxCreatedDate = _cards.Max(c => c.CreatedDate);

            var days = Enumerable.Range(0, 1 + maxCreatedDate.Subtract(dateTime).Days)
                .Select(o => dateTime.AddDays(o)).ToList();

            return (from day in days
                let countByDay = GetCardCountsFor(AllPredicateFor(day))
                let doneCountByDay = GetCardCountsFor(DonePredicateFor(day))
                select new CardCount
                {
                    Date = day,
                    DoneTotal = doneCountByDay,
                    Total = countByDay
                }).ToList();
        }

        private static Func<Card, bool> AllPredicateFor(DateTime day)
        {
            return c => c.CreatedDate <= day;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime dateTime)
        {
            return c => c.CreatedDate <= dateTime && c.Status.Equals(CardStatus.Status.Done);
        }

        private int GetCardCountsFor(Func<Card, bool> func)
        {
            return _cards.Count(func);
        }
    }

    public static class CardStatus
    {
        public enum Status
        {
            Todo,
            Doing,
            Done
        };
    }

    public class Card
    {
        public CardStatus.Status Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Id { get; set; }
    }
}
