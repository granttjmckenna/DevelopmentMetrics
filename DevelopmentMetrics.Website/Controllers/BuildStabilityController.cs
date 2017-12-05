using System.Collections.Generic;
using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;

namespace DevelopmentMetrics.Website.Controllers
{
    public class BuildStabilityController : Controller
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly ITeamCityWebClient _teamCityWebClient;
        private List<Build> _builds;

        public BuildStabilityController(ITeamCityWebClient teamCityWebClient, ITellTheTime tellTheTime)
        {
            _teamCityWebClient = teamCityWebClient;
            _tellTheTime = tellTheTime;
        }

        // GET: BuildStability
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetBuildChartDataFor(int numberOfWeeks)
        {
            _builds = new Build(_teamCityWebClient, _tellTheTime).GetBuilds();

            var buildData = new BuildMetric(_builds, _tellTheTime).CalculateBuildFailingRateByWeekFor(numberOfWeeks);

            return Json(buildData);
        }
    }
}