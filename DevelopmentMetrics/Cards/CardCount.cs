using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Cards
{
    public class CardCount
    {
        private readonly IEnumerable<Card> _cards;
        public DateTime Date { get; set; }
        public int DoneTotal { get; set; }
        public int Total { get; set; }

        private CardCount() { }

        public CardCount(IEnumerable<Card> cards)
        {
            _cards = cards;
        }

        public List<CardCount> GetCardCountByDayFrom(DateTime dateTime)
        {
            var maxCreatedDate = _cards.Max(c => c.CreateDate).AddDays(2);

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
                })
                .ToList();
        }

        private static Func<Card, bool> AllPredicateFor(DateTime day)
        {
            return c => c.CreateDate <= day;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime dateTime)
        {
            return c => c.CreateDate <= dateTime && c.Status.Equals(CardStatus.Status.Done);
        }

        private int GetCardCountsFor(Func<Card, bool> func)
        {
            return _cards.Count(func);
        }
    }
}