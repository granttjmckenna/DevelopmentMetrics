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

        public DateTime Date { get; set; }
        public int DoneTotal { get; set; }
        public int Total { get; set; }
        public double DefectRate { get; set; }

        private CardCount() { }

        public CardCount(ITellTheTime tellTheTime, IEnumerable<Card> cards)
        {
            _tellTheTime = tellTheTime;
            _cards = cards;
        }

        public List<CardCount> GetCardCountByDayFrom(DateTime dateTime)
        {
            var maxCreatedDate = _tellTheTime.Now();

            var days = Enumerable.Range(0, 1 + maxCreatedDate.Subtract(dateTime).Days)
                .Select(o => dateTime.AddDays(o)).ToList();

            return (from day in days
                    let countByDay = GetCardCountsFor(AllPredicateFor(day))
                    let doneCountByDay = GetCardCountsFor(DonePredicateFor(day))
                    let defectCountByDay = GetCardCountsFor(c => c.TypeName.Equals("Defect") && c.CreateDate <= day)
                    select new CardCount
                    {
                        Date = day,
                        DoneTotal = doneCountByDay,
                        Total = countByDay,
                        DefectRate = Calculator.Percentage(defectCountByDay, countByDay)
                    })
                .ToList();
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


        public int GetCountOfCardsInWorkInProcessFor(DateTime calculationDateTime)
        {
            var cardCount = _cards.Count(c => c.CreateDate <= calculationDateTime);

            var doneCardCount = _cards.Count(DonePredicateFor(calculationDateTime));

            return cardCount - doneCardCount;
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