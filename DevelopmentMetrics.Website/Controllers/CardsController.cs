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
        private List<Card> _cards;

        public CardsController(ILeanKitWebClient leanKitWebClient)
        {
            _leanKitWebClient = leanKitWebClient;
        }


        // GET: Cards
        public ActionResult Index()
        {
            _cards = GetCards();

            var model = new CardsViewModel(_cards);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetCardCountByDay(int numberOfDays)
        {
            if (IsClearCache(numberOfDays))
            {
                CacheHelper.ClearObjectFromCache(Card.CacheKey);

                const int defaultNumberOfDays = 42;

                numberOfDays = defaultNumberOfDays;
            }

            _cards = GetCards();

            return Json(new CardCount(_cards).GetCardCountByDayFrom(DateTime.Now.AddDays(numberOfDays * -1)));
        }

        private List<Card> GetCards()
        {
            return new Card(_leanKitWebClient).GetCards();
        }

        private bool IsClearCache(int numberOfDays)
        {
            return numberOfDays == -1;
        }
    }
}