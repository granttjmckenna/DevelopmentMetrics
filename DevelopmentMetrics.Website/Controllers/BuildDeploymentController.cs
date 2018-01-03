using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class BuildDeploymentController : Controller
    {
        private readonly IBuild _build;
        private readonly ITellTheTime _tellTheTime;

        public BuildDeploymentController(IBuild build, ITellTheTime tellTheTime)
        {
            _build = build;
            _tellTheTime = tellTheTime;
        }

        // GET: BuildDeployment
        public ActionResult Index()
        {
            var model = new BuildDeploymentViewModel(_build, _tellTheTime);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetBuildChartDataFor(int numberOfWeeks, string buildAgent, string buildTypeId)
        {
            var buildData = new BuildDeployment(_build, _tellTheTime)
                .CalculateBuildDeploymentIntervalByWeekFor(new BuildFilter(numberOfWeeks, buildAgent, buildTypeId));

            return Json(buildData);
        }
    }
}