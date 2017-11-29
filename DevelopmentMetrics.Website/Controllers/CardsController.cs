using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class CardsController : Controller
    {
        private readonly ILeanKitWebClient _leanKitWebClient;
        private readonly ITellTheTime _tellTheTime;
        private List<Card> _cards;

        public CardsController(ILeanKitWebClient leanKitWebClient, ITellTheTime tellTheTime)
        {
            _tellTheTime = tellTheTime;
            _leanKitWebClient = leanKitWebClient;
        }


        // GET: Cards
        public ActionResult Index()
        {
            _cards = GetCards();

            var model = new CardsViewModel(_tellTheTime, _cards);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetCardChartDataFor(int numberOfDays)
        {
            _cards = GetCards();

            var cardCounts = new CardCount(_tellTheTime, _cards).GetCardCountByDayFrom(numberOfDays);

            return Json(cardCounts);
        }

        private List<Card> GetCards()
        {
            return new Card(_leanKitWebClient, _tellTheTime).GetCards();
        }
    }
}