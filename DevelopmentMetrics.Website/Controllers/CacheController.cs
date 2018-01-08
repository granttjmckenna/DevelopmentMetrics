using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class CacheController : Controller
    {
        private ICard _card;
        private IBuild _build;
        private ICacheChecker _cacheChecker;

        public CacheController(ICacheChecker cacheChecker,IBuild build, ICard card)
        {
            _cacheChecker = cacheChecker;
            _build = build;
            _card = card;
        }

        [HttpGet]
        public JsonResult ReturnTrueWhenBuildDataCached()
        {
            new Cache(_cacheChecker, _build, _card).IsBuildDataCached();

            return Json(true);
        }
    }
}