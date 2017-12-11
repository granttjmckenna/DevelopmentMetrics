using System.Collections.Generic;
using System.Web.Mvc;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class BuildStabilityController : Controller
    {
        private readonly ITellTheTime _tellTheTime;
        private readonly ITeamCityWebClient _teamCityWebClient;
        private readonly IBuildsToExclude _buildsToExclude;
        private List<Build> _builds;

        public BuildStabilityController(ITeamCityWebClient teamCityWebClient, ITellTheTime tellTheTime, IBuildsToExclude buildsToExclude)
        {
            _teamCityWebClient = teamCityWebClient;
            _tellTheTime = tellTheTime;
            _buildsToExclude = buildsToExclude;
        }

        // GET: BuildStability
        public ActionResult Index()
        {
            _builds = GetBuilds();

            var model = new BuildStabilityViewModel(_builds, _tellTheTime);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetBuildChartDataFor(int numberOfWeeks, string buildAgent, string buildTypeId)
        {
            if (IsClearCache(numberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            _builds = GetBuilds();

            var buildData = new BuildMetric(_tellTheTime).CalculateBuildFailingRateByWeekFor(_builds,
                new BuildFilter(numberOfWeeks, buildAgent, buildTypeId));

            return Json(buildData);
        }

        private List<Build> GetBuilds()
        {
            return new Build(_teamCityWebClient, _tellTheTime, _buildsToExclude).GetBuilds();
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}