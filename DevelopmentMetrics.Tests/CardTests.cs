using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class CardTests
    {
        private List<Card> _cards;

        [SetUp]
        public void Setup()
        {
            _cards = GetCards().ToList();
        }

        private static IEnumerable<Card> GetCards()
        {
            var cards = new List<Card>
            {
                new Card { Id = 1, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,01)},
                new Card { Id = 2, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,02) },
                new Card { Id = 3, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,02) },
                new Card { Id = 4, Status = CardStatus.Status.Todo,CreatedDate = new DateTime(2017,10,03) },
                new Card { Id = 5, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,01) },
                new Card { Id = 6, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,01) },
                new Card { Id = 7, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,01) },
                new Card { Id = 8, Status = CardStatus.Status.Done,CreatedDate = new DateTime(2017,10,03) },
                new Card { Id = 9, Status = CardStatus.Status.Doing,CreatedDate = new DateTime(2017,10,02) },
                new Card { Id = 10, Status = CardStatus.Status.Doing,CreatedDate = new DateTime(2017,10,02) },
            };

            return cards;
        }

        [Test]
        [Description("Card metric test")]
        public void Return_card_count_by_status()
        {
            var cardMetric = new CardMetric(_cards).GetCountByStatus();

            Assert.That(cardMetric[CardStatus.Status.Todo], Is.EqualTo(4));
            Assert.That(cardMetric[CardStatus.Status.Doing], Is.EqualTo(2));
            Assert.That(cardMetric[CardStatus.Status.Done], Is.EqualTo(4));
        }

        [Test]
        [Description("Card count metric test")]
        public void Return_card_count_for_all_cards_in_the_process_for_a_given_day()
        {
            var calculationDate = new DateTime(2017, 10, 01);

            var cardCount = new CardCount(_cards).GetCardCountByDayFrom(calculationDate);

            Assert.That(cardCount.First(c => c.Date == calculationDate).Total, Is.EqualTo(4));
        }

        [Test]
        [Description("Card count metric test")]
        public void Return_card_count_for_done_cards_in_the_process_for_a_given_day()
        {
            var calculationDate = new DateTime(2017, 10, 01);

            var cardCount = new CardCount(_cards).GetCardCountByDayFrom(calculationDate);

            Assert.That(cardCount.First(c => c.Date == calculationDate).DoneTotal, Is.EqualTo(3));
        }

        [Test]
        [Description("Card metric test")]
        public void Return_lead_time_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var leadTime = new CardMetric(_cards).CalculateLeadTimeFor(dateTime);

            Assert.That(leadTime, Is.EqualTo(2));
        }

        [Test]
        [Description("Card metric test")]
        public void Return_work_in_process_for_a_given_day()
        {
            var dateTime = new DateTime(2017, 10, 03);

            var workInProcess = new CardMetric(_cards).CalculateWorkInProcessFor(dateTime);

            Assert.That(workInProcess, Is.EqualTo(6));
        }

        [Test]
        [Description("Card count tests")]
        public void Return_collection_of_count_by_day_for_all_cards()
        {
            var countByDays = new CardCount(_cards).GetCardCountByDayFrom(new DateTime(2017, 10, 01));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).Total, Is.EqualTo(4));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).Total, Is.EqualTo(8));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).Total, Is.EqualTo(10));

            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 01)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 02)).DoneTotal, Is.EqualTo(3));
            Assert.That(countByDays.First(c => c.Date == new DateTime(2017, 10, 03)).DoneTotal, Is.EqualTo(4));

            Assert.That(countByDays.All(c => c.Date != new DateTime(2017, 10, 04)));
        }
    }

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
                {CardStatus.Status.Done, _cards.Count(c => c.Status.Equals(CardStatus.Status.Done))}
            };

            return result;
        }

        public int CalculateLeadTimeFor(DateTime calculationDate)
        {
            var cardPosition = _cards
                .OrderBy(c => c.CreatedDate)
                .Count(DonePredicateFor(calculationDate));

            var cardDate = _cards.OrderBy(c => c.CreatedDate).Take(cardPosition).Max(c => c.CreatedDate);

            return (calculationDate - cardDate).Days;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime calculationDate)
        {
            return c => c.CreatedDate <= calculationDate && c.Status.Equals(CardStatus.Status.Done);
        }

        public int CalculateWorkInProcessFor(DateTime calculationDateTime)
        {
            var cardCount = _cards.Count(c => c.CreatedDate <= calculationDateTime);

            var doneCardCount = _cards.Count(DonePredicateFor(calculationDateTime));

            return cardCount - doneCardCount;
        }
    }

    public class CardCount
    {
        private readonly IEnumerable<Card> _cards;
        public DateTime Date { get; set; }
        public int DoneTotal { get; set; }
        public int Total { get; set; }

        private CardCount() { }

        public CardCount(IEnumerable<Card> cards)
        {
            _cards = cards;
        }

        public List<CardCount> GetCardCountByDayFrom(DateTime dateTime)
        {
            var maxCreatedDate = _cards.Max(c => c.CreatedDate);

            var days = Enumerable.Range(0, 1 + maxCreatedDate.Subtract(dateTime).Days)
                .Select(o => dateTime.AddDays(o)).ToList();

            return (from day in days
                    let countByDay = GetCardCountsFor(AllPredicateFor(day))
                    let doneCountByDay = GetCardCountsFor(DonePredicateFor(day))
                    select new CardCount
                    {
                        Date = day,
                        DoneTotal = doneCountByDay,
                        Total = countByDay
                    }).ToList();
        }

        private static Func<Card, bool> AllPredicateFor(DateTime day)
        {
            return c => c.CreatedDate <= day;
        }

        private static Func<Card, bool> DonePredicateFor(DateTime dateTime)
        {
            return c => c.CreatedDate <= dateTime && c.Status.Equals(CardStatus.Status.Done);
        }

        private int GetCardCountsFor(Func<Card, bool> func)
        {
            return _cards.Count(func);
        }
    }

    public static class CardStatus
    {
        public enum Status
        {
            Todo,
            Doing,
            Done
        };
    }

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

        public IEnumerable<Card> GetCards()
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
                    });
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

    public interface ILeanKitWebClient
    {
        string GetBoardData();

        string GetCardDataFor(int cardId);
    }

    public class LeanKitWebClient : ILeanKitWebClient
    {
        public string GetBoardData()
        {
            const string url = @"https://ehl.leankit.com/kanban/api/boards/566488298";

            return Get(url);
        }

        public string GetCardDataFor(int cardId)
        {
            var url = $"https://ehl.leankit.com/kanban/api/board/566488298/GetCard/{cardId}";

            return Get(url);
        }

        private string Get(string url)
        {
            var result = string.Empty;

            var webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json; charset=utf-8;";

            using (var webResponse = webRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null)
                        return result;

                    var streamReader = new StreamReader(responseStream);

                    result = streamReader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
