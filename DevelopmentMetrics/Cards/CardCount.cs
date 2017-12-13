using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Cards
{
    public class CardCount
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly ICard _card;

        public CardCount(ICard card, ITellTheTime tellTheTime)
        {
            _card = card;
            _tellTheTime = tellTheTime;
        }

        public List<Count> GetCardCountByDayFrom(int numberOfDays)
        {
            if (IsClearCache(numberOfDays))
            {
                CacheHelper.ClearObjectFromCache(Card.CacheKey);
            }

            var fromDate = GetFromDate(numberOfDays);

            var days = Enumerable.Range(0, 1 + _tellTheTime.Now().Subtract(fromDate).Days)
                .Select(o => fromDate.AddDays(o)).ToList();

            return (from day in days
                    let countByDay = GetCardCountsFor(AllPredicateFor(day))
                    let doneCountByDay = GetCardCountsFor(DonePredicateFor(day))
                    let reworkCountByDay = GetCardCountsFor(AllDefectsNotDoneFor(day))
                    select new Count
                    {
                        Date = day,
                        DoneTotal = doneCountByDay,
                        Total = countByDay,
                        Rate = Calculator.Percentage(reworkCountByDay, countByDay)
                    })
                .ToList();
        }

        public Dictionary<CardStatus.Status, int> GetCountByStatus()
        {
            var cards = _card.GetCards();

            var result = new Dictionary<CardStatus.Status, int>
            {
                {CardStatus.Status.Todo, cards.Count(c => c.Status.Equals(CardStatus.Status.Todo))},
                {CardStatus.Status.Doing, cards.Count(c => c.Status.Equals(CardStatus.Status.Doing))},
                {CardStatus.Status.Done, cards.Count(c => c.Status.Equals(CardStatus.Status.Done))},
                {CardStatus.Status.Unassigned, cards.Count(c => c.Status.Equals(CardStatus.Status.Unassigned))},
                {CardStatus.Status.All, cards.Count() }
            };

            return result;
        }

        public int GetInWorkInProcessCountFor(DateTime calculationDateTime)
        {
            var cards = _card.GetCards();

            return cards.Count(c => c.CreateDate <= calculationDateTime)
                - cards.Count(DonePredicateFor(calculationDateTime));
        }

        private DateTime GetFromDate(int numberOfDays)
        {
            switch (numberOfDays)
            {
                case -1:
                    return _tellTheTime.Now().AddDays(-42);
                case -2:
                    return _card.GetCards().Min(c => c.CreateDate);
                default:
                    return _tellTheTime.Now().AddDays(numberOfDays * -1);
            }
        }

        private static Func<Card, bool> AllPredicateFor(DateTime day)
        {
            return c => c.CreateDate <= day;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime dateTime)
        {
            return c => c.CreateDate <= dateTime && c.Status.Equals(CardStatus.Status.Done);
        }

        private static Func<Card, bool> AllDefectsNotDoneFor(DateTime day)
        {
            return c => c.TypeName.Equals("Defect") && c.CreateDate <= day &&
                        (c.DoneDate.HasValue && c.DoneDate.Value < day);
        }

        private int GetCardCountsFor(Func<Card, bool> func)
        {
            return _card.GetCards().Count(func);
        }

        private bool IsClearCache(int numberOfDays)
        {
            return numberOfDays == -1;
        }
    }
}