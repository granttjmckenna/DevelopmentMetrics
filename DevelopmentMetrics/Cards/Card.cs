using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Cards
{
    public class Card
    {
        private readonly ILeanKitWebClient _leanKitLeanKitWebClient;
        public CardStatus.Status Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }

        public Card() { }

        public Card(ILeanKitWebClient leanKitWebClient)
        {
            _leanKitLeanKitWebClient = leanKitWebClient;
        }

        public List<Card> GetCards()
        {
            var cards = Helpers.CacheHelper.GetObjectFromCache<List<Card>>("cards", 60, GetCardsFromRepo);

            return cards;
        }

        private List<Card> GetCardsFromRepo()
        {
            var boardData = _leanKitLeanKitWebClient.GetBoardData();

            var rootObject = JsonConvert.DeserializeObject<RootObject>(boardData);

            return (from lane in rootObject.ReplyData.First().Lanes
                    from card in lane.Cards
                    select new Card
                    {
                        Id = card.Id,
                        Title = card.Title,
                        Status = GetCardStatusFor(lane.Type),
                        CreatedDate = GetCardCreatedDateFor(card.Id)
                    })
                .ToList();
        }

        private CardStatus.Status GetCardStatusFor(int laneTypeId)
        {
            switch (laneTypeId)
            {
                case 1:
                    return CardStatus.Status.Todo;
                case 2:
                    return CardStatus.Status.Doing;
                case 3:
                case 99: //archived
                    return CardStatus.Status.Done;
                default:
                    throw new Exception($"Lane type not recognised: {laneTypeId}");
            }
        }

        private DateTime GetCardCreatedDateFor(int cardId)
        {
            var cardData = _leanKitLeanKitWebClient.GetCardDataFor(cardId);

            var cardDetail = JsonConvert.DeserializeObject<CardDetail>(cardData);

            return cardDetail.CreatedDate;
        }
    }
    internal class ReplyData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Lane> Lanes { get; set; }
    }

    internal class RootObject
    {
        public List<ReplyData> ReplyData { get; set; }
    }

    internal class Lane
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public List<Card> Cards { get; set; }
    }

    internal class CardDetail
    {
        public DateTime CreatedDate { get; set; }
    }
}