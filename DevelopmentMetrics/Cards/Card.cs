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
        public DateTime CreateDate { get; set; }
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
                        CreateDate = GetCardCreateDateFor(card.Id)
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
                    return CardStatus.Status.Done;
                case 99:
                    return CardStatus.Status.Unassigned;
                default:
                    throw new Exception($"Lane type not recognised: {laneTypeId}");

            }
        }

        private DateTime GetCardCreateDateFor(int cardId)
        {
            var cardData = _leanKitLeanKitWebClient.GetCardDataFor(cardId);

            var replyData = JsonConvert.DeserializeObject<RootObject>(cardData);

            return DateTime.Parse(replyData.ReplyData.First().CreateDate);
        }
    }
    internal class ReplyData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Lane> Lanes { get; set; }
        public string CreateDate { get; set; }
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
}