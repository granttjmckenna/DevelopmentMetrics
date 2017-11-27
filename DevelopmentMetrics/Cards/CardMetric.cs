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
        
        public int CalculateLeadTimeFor(DateTime calculationDate)
        {
            var cardPosition = _cards
                .OrderBy(c => c.CreateDate)
                .Count(DonePredicateFor(calculationDate));

            var cardDate = _cards.OrderBy(c => c.CreateDate).Take(cardPosition).Max(c => c.CreateDate);

            return (calculationDate - cardDate).Days;
        }

        public int CalculateWorkInProcessFor(DateTime calculationDateTime)
        {
            var cardCount = _cards.Count(c => c.CreateDate <= calculationDateTime);

            var doneCardCount = _cards.Count(DonePredicateFor(calculationDateTime));

            return cardCount - doneCardCount;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime calculationDate)
        {
            return c => c.CreateDate <= calculationDate && c.Status.Equals(CardStatus.Status.Done);
        }
    }
}