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
        private IBuildsToExclude _buildsToExclude;
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

            var filteredBuilds = GetFilteredBuilds(buildAgent, buildTypeId);

            var buildData = new BuildMetric(_tellTheTime).CalculateBuildFailingRateByWeekFor(filteredBuilds, numberOfWeeks);

            return Json(buildData);
        }

        private List<Build> GetFilteredBuilds(string buildAgent, string buildTypeId)
        {
            _builds = GetBuilds();

            if (buildAgent.Equals("All") && buildTypeId.Equals("All"))
            {
                return _builds;
            }
            else if (!buildAgent.Equals("All") && !buildTypeId.Equals("All"))
            {
                return _builds.Where(b =>
                        b.AgentName.Equals(buildAgent, StringComparison.InvariantCultureIgnoreCase)
                        && b.BuildTypeId.StartsWith(buildTypeId, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            else if (!buildAgent.Equals("All"))
            {
                return _builds.Where(b =>
                        b.AgentName.Equals(buildAgent, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            else if (!buildTypeId.Equals("All"))
            {
                return _builds.Where(b =>
                        b.BuildTypeId.StartsWith(buildTypeId, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }
            else
            {
                throw new Exception("This has not worked!");
            }
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