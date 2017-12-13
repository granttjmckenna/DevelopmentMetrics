using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;

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
            return View();
        }

        [HttpPost]
        public JsonResult GetBuildThroughputChartDataFor(int numberOfWeeks)
        {
            var buildData = new BuildThroughput(_build, _tellTheTime).CalculateBuildIntervalByWeekFor(numberOfWeeks);

            return Json(buildData);
        }
    }
}