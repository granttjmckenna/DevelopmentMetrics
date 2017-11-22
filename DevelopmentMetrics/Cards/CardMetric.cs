using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Cards
{
    public class CardMetric
    {
        private readonly IEnumerable<Card> _cards;

        public CardMetric(IEnumerable<Card> cards)
        {
            _cards = cards;
        }

        public Dictionary<CardStatus.Status, int> GetCountByStatus()
        {
            var result = new Dictionary<CardStatus.Status, int>
            {
                {CardStatus.Status.Todo, _cards.Count(c => c.Status.Equals(CardStatus.Status.Todo))},
                {CardStatus.Status.Doing, _cards.Count(c => c.Status.Equals(CardStatus.Status.Doing))},
                {CardStatus.Status.Done, _cards.Count(c => c.Status.Equals(CardStatus.Status.Done))},
                {CardStatus.Status.Unassigned, _cards.Count(c => c.Status.Equals(CardStatus.Status.Unassigned))}
            };

            return result;
        }

        public int CalculateLeadTimeFor(DateTime calculationDate)
        {
            var cardPosition = _cards
                .OrderBy(c => c.CreateDate)
                .Count(DonePredicateFor(calculationDate));

            var cardDate = _cards.OrderBy(c => c.CreateDate).Take(cardPosition).Max(c => c.CreateDate);

            return (calculationDate - cardDate).Days;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime calculationDate)
        {
            return c => c.CreateDate <= calculationDate && c.Status.Equals(CardStatus.Status.Done);
        }

        public int CalculateWorkInProcessFor(DateTime calculationDateTime)
        {
            var cardCount = _cards.Count(c => c.CreateDate <= calculationDateTime);

            var doneCardCount = _cards.Count(DonePredicateFor(calculationDateTime));

            return cardCount - doneCardCount;
        }
    }
}