using System.Web.Mvc;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class CardsController : Controller
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly ICard _card;

        public CardsController(ICard card, ITellTheTime tellTheTime)
        {
            _card = card;
            _tellTheTime = tellTheTime;
        }


        // GET: Cards
        public ActionResult Index()
        {
            var model = new CardsViewModel(_card, _tellTheTime);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetCardChartDataFor(int numberOfDays)
        {
            var cardCounts = new CardCount(_card, _tellTheTime).GetCardCountByDayFrom(numberOfDays);

            return Json(cardCounts);
        }
    }
}