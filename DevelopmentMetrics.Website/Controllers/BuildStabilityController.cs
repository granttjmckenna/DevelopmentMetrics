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

        [HttpPost]
        public JsonResult GetBuildChartDataFor(int numberOfWeeks, string buildAgent, string buildTypeId)
        {
            if (IsClearCache(numberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            var buildData = new BuildMetric(_tellTheTime, _build).CalculateBuildFailingRateByWeekFor(
                new BuildFilter(numberOfWeeks, buildAgent, buildTypeId));

            return Json(buildData);
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}