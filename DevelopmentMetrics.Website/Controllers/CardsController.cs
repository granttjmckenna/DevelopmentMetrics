using System;
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

        public CardsController(ILeanKitWebClient leanKitWebClient)
        {
            _leanKitWebClient = leanKitWebClient;
        }


        // GET: Cards
        public ActionResult Index()
        {
            var cards = new Card(_leanKitWebClient).GetCards();

            var model = new CardsViewModel(cards);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetCardCountByDay(int numberOfDays)
        {
            const int defaultNumberOfDays = 42;

            if (numberOfDays == -1)
            {
                CacheHelper.ClearObjectFromCache("cards");

                numberOfDays = defaultNumberOfDays;
            }

            var cards = new Card(_leanKitWebClient).GetCards();

            return Json(new CardCount(cards).GetCardCountByDayFrom(DateTime.Now.AddDays(numberOfDays * -1)));
        }
    }
}