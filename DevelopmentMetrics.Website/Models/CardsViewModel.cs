using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Cards;

namespace DevelopmentMetrics.Website.Models
{
    public class CardsViewModel
    {
        private readonly IEnumerable<Card> _cards;

        public CardsViewModel(IEnumerable<Card> cards)
        {
            _cards = cards;
        }

        public Dictionary<CardStatus.Status, int> GetCardCountByStatus()
        {
            return new CardMetric(_cards).GetCountByStatus();
        }

        public int CalculateLeadTime()
        {
            return new CardMetric(_cards).CalculateLeadTimeFor(DateTime.Now);
        }

        public List<Card> GetCardsInProcess()
        {
            return _cards.Where(c => c.Status == CardStatus.Status.Doing).ToList();
        }
    }
}