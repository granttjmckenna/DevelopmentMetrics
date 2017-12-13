using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;

namespace DevelopmentMetrics.Website.Models
{
    public class CardsViewModel
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly ICard _card;

        public CardsViewModel(ICard card, ITellTheTime tellTheTime)
        {
            _card = card;
            _tellTheTime = tellTheTime;
        }

        public Dictionary<CardStatus.Status, int> GetCardCountByStatus()
        {
            return new CardCount(_card, _tellTheTime).GetCountByStatus();
        }

        public int CalculateLeadTime()
        {
            return new CardMetric(_card).CalculateLeadTimeFor(DateTime.Now);
        }

        public List<Card> GetCardsInTodo()
        {
            return _card.GetCards().Where(Predicate(CardStatus.Status.Todo)).ToList();
        }

        public List<Card> GetCardsInDoing()
        {
            return _card.GetCards().Where(Predicate(CardStatus.Status.Doing)).ToList();
        }

        public Dictionary<string, string> GetBuildWeeksList()
        {
            var results = new Dictionary<string, string>
            {
                {"6 weeks (default)", "6"},
                {"7 weeks", "7"},
                {"8 weeks", "8"},
                {"9 weeks", "9"},
                {"10 weeks", "10"},
                {"15 weeks", "15"},
                {"20 weeks", "20"},
                {"All", "-2"}
            };

            return results;
        }

        private static Func<Card, bool> Predicate(CardStatus.Status cardStatus)
        {
            return c => c.Status == cardStatus;
        }
    }
}