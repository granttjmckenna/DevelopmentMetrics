using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Build> _builds;

        public BuildStabilityController(ITeamCityWebClient teamCityWebClient, ITellTheTime tellTheTime)
        {
            _teamCityWebClient = teamCityWebClient;
            _tellTheTime = tellTheTime;
        }

        // GET: BuildStability
        public ActionResult Index()
        {
            _builds = GetBuilds();

            var buildTypeIds = new BuildMetric(_builds, _tellTheTime).GetDistinctBuildTypeIdsFrom(_builds);

            var model = new BuildStabilityViewModel(buildTypeIds);

            return View(model);
        }

        [HttpPost]
        public JsonResult GetBuildChartDataFor(int numberOfWeeks, string buildAgent)
        {
            if (IsClearCache(numberOfWeeks))
            {
                CacheHelper.ClearObjectFromCache(Build.CacheKey);
            }

            _builds = GetBuilds();

            var filteredBuilds = _builds
                .Where(b =>
                    b.AgentName.Equals(buildAgent, StringComparison.InvariantCultureIgnoreCase) ||
                    buildAgent.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var buildData = new BuildMetric(filteredBuilds, _tellTheTime).CalculateBuildFailingRateByWeekFor(numberOfWeeks);

            return Json(buildData);
        }

        private List<Build> GetBuilds()
        {
            return new Build(_teamCityWebClient, _tellTheTime).GetBuilds();
        }

        private bool IsClearCache(int numberOfWeeks)
        {
            return numberOfWeeks == -1;
        }
    }
}