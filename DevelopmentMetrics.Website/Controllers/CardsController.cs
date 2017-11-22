using System;
using System.Web.Mvc;
using DevelopmentMetrics.Cards;
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

        //GET: CardMetrics
        public JsonResult GetCardCountByDay()
        {
            var cards = new Card(_leanKitWebClient).GetCards();

            return Json(new CardCount(cards).GetCardCountByDayFrom(DateTime.Now.AddDays(-42)),
                JsonRequestBehavior.AllowGet);
        }
    }
}