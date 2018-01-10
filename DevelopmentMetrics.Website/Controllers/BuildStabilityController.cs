using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class BuildStabilityController : Controller
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildStabilityController(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        // GET: BuildStability
        public ActionResult Index()
        {
            var model = new BuildStabilityViewModel(_build, _tellTheTime);

            return View(model);
        }

        [HttpGet]
        public JsonResult ReturnWhenBuildDataCached()
        {
            GetBuildChartDataFor(6, "All", "All");

            return Json(true);
        }

        [HttpPost]
        public JsonResult GetBuildChartDataFor(int numberOfWeeks, string buildAgent, string buildTypeId)
        {
            var buildData = new BuildStability(_tellTheTime, _build)
                .CalculateBuildFailingRateByWeek(new BuildFilter(numberOfWeeks, buildAgent, buildTypeId));

            return Json(buildData);
        }
    }
}