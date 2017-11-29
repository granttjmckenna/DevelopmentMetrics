using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class CardsViewModel
    {
        private readonly IEnumerable<Card> _cards;
        private readonly ITellTheTime _tellTheTime;

        public CardsViewModel(ITellTheTime tellTheTime, IEnumerable<Card> cards)
        {
            _cards = cards;
            _tellTheTime = tellTheTime;
        }

        public Dictionary<CardStatus.Status, int> GetCardCountByStatus()
        {
            return new CardCount(_tellTheTime, _cards).GetCountByStatus();
        }

        public int CalculateLeadTime()
        {
            return new CardMetric(_cards).CalculateLeadTimeFor(DateTime.Now);
        }

        public List<Card> GetCardsInTodo()
        {
            return _cards.Where(Predicate(CardStatus.Status.Todo)).ToList();
        }

        public List<Card> GetCardsInDoing()
        {
            return _cards.Where(Predicate(CardStatus.Status.Doing)).ToList();
        }

        private static Func<Card, bool> Predicate(CardStatus.Status cardStatus)
        {
            return c => c.Status == cardStatus;
        }
    }
}