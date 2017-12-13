using System;
using System.Linq;

namespace DevelopmentMetrics.Cards
{
    public class CardMetric
    {
        private readonly ICard _card;

        public CardMetric(ICard card)
        {
            _card = card;
        }
        
        public int CalculateLeadTimeFor(DateTime calculationDate)
        {
            var cards = _card.GetCards();

            var cardPosition = cards
                .OrderBy(c => c.CreateDate)
                .Count(DonePredicateFor(calculationDate));

            var cardDate = cards.OrderBy(c => c.CreateDate).Take(cardPosition).Max(c => c.CreateDate);

            return (calculationDate - cardDate).Days;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime calculationDate)
        {
            return c => c.CreateDate <= calculationDate && c.Status.Equals(CardStatus.Status.Done);
        }
    }
}