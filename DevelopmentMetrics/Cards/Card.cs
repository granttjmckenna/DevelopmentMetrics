using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;
using Newtonsoft.Json;

namespace DevelopmentMetrics.Cards
{
    public class Card
    {
        private readonly ILeanKitWebClient _leanKitLeanKitWebClient;
        private readonly ITellTheTime _tellTheTime;

        public CardStatus.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string TypeName { get; set; }
        public DateTime? DoneDate { get; set; }

        public static string CacheKey = "cards";

        public Card() { }
        public Card(ILeanKitWebClient leanKitWebClient, ITellTheTime tellTheTime)
        {
            _leanKitLeanKitWebClient = leanKitWebClient;
            _tellTheTime = tellTheTime;
        }

        public List<Card> GetCards()
        {
            var cards = CacheHelper.GetObjectFromCache<List<Card>>(CacheKey, 60, GetCardsFromRepo);

            return cards;
        }

        private List<Card> GetCardsFromRepo()
        {
            var boardData = _leanKitLeanKitWebClient.GetBoardData();

            var rootObject = JsonConvert.DeserializeObject<RootObject>(boardData);

            return (from lane in rootObject.ReplyData.First().Lanes
                    from card in lane.Cards
                    let cardDates = GetCardCreateDateFor(card.Id)
                    select new Card
                    {
                        Id = card.Id,
                        Title = card.Title,
                        Status = GetCardStatusFor(lane.Type),
                        CreateDate = cardDates.CreateDate,
                        DoneDate = cardDates.DoneDate,
                        TypeName = card.TypeName
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

        private CardDate GetCardCreateDateFor(int cardId)
        {
            var cardData = _leanKitLeanKitWebClient.GetCardDataFor(cardId);

            var cardDetails = JsonConvert.DeserializeObject<RootObject>(cardData).ReplyData.First();

            var cardDate = new CardDate
            {
                CreateDate = DateTime.Parse(cardDetails.CreateDate)
            };

            if (!string.IsNullOrWhiteSpace(cardDetails.DoneDate))
            {
                cardDate.DoneDate = _tellTheTime.ParseDateToUkFormat(cardDetails.DoneDate);
            }

            return cardDate;
        }
    }

    internal class CardDate
    {
        public DateTime CreateDate { get; set; }
        public DateTime? DoneDate { get; set; }
    }

    internal class ReplyData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Lane> Lanes { get; set; }
        public string CreateDate { get; set; }
        [JsonPropertyAttribute("ActualFinishDate")]
        public string DoneDate { get; set; }

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