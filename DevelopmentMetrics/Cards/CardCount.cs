using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Cards
{
    public class CardCount
    {
        private readonly ITellTheTime _tellTheTime;

        private readonly IEnumerable<Card> _cards;

        public CardCount(ITellTheTime tellTheTime, IEnumerable<Card> cards)
        {
            _tellTheTime = tellTheTime;
            _cards = cards;
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
                        DefectRate = Calculator.Percentage(reworkCountByDay, countByDay)
                    })
                .ToList();
        }

        private DateTime GetFromDate(int numberOfDays)
        {
            if (numberOfDays == -1)
            {
                return _tellTheTime.Now().AddDays(-42);
            }
            else if (numberOfDays == 9999)
            {
                return _cards.Min(c => c.CreateDate);
            }
            else
            {
                return _tellTheTime.Now().AddDays(numberOfDays * -1);
            }
        }

        public Dictionary<CardStatus.Status, int> GetCountByStatus()
        {
            var result = new Dictionary<CardStatus.Status, int>
            {
                {CardStatus.Status.Todo, _cards.Count(c => c.Status.Equals(CardStatus.Status.Todo))},
                {CardStatus.Status.Doing, _cards.Count(c => c.Status.Equals(CardStatus.Status.Doing))},
                {CardStatus.Status.Done, _cards.Count(c => c.Status.Equals(CardStatus.Status.Done))},
                {CardStatus.Status.Unassigned, _cards.Count(c => c.Status.Equals(CardStatus.Status.Unassigned))},
                {CardStatus.Status.All, _cards.Count() }
            };

            return result;
        }

        public int GetInWorkInProcessCountFor(DateTime calculationDateTime)
        {
            return _cards.Count(c => c.CreateDate <= calculationDateTime)
                - _cards.Count(DonePredicateFor(calculationDateTime));
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
            return _cards.Count(func);
        }

        private bool IsClearCache(int numberOfDays)
        {
            return numberOfDays == -1;
        }
    }
}