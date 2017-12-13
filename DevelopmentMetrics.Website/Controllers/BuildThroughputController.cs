using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class BuildThroughputController : Controller
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildThroughputController(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }
        // GET: BuildThroughput
        public ActionResult Index()
        {
            var model = new BuildThroughputViewModel(_build, _tellTheTime);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetBuildThroughputChartDataFor(int numberOfWeeks)
        {
            var buildData = new BuildThroughput(_build, _tellTheTime).CalculateBuildIntervalByWeekFor(numberOfWeeks);

            return Json(buildData);
        }
    }
}