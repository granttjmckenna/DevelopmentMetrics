﻿using System;
using System.Collections.Generic;
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

        public List<CardCount> GetCardCountByDay()
        {
            return new CardCount(_cards).GetCardCountByDayFrom(DateTime.Now.AddDays(-42));
        }

        public Dictionary<CardStatus.Status, int> GetCardCountByStatus()
        {
            return new CardMetric(_cards).GetCountByStatus();
        }
    }
}