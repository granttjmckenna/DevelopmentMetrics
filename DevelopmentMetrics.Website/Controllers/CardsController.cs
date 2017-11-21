using System.Web.Mvc;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class CardsController : Controller
    {
        // GET: Cards
        public ActionResult Index()
        {
            var model = new CardsViewModel();


            return View(model);
        }
    }
}