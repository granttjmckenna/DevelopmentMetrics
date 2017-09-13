using System;
using System.Web.Mvc;
using DevelopmentMetrics.Models;
using DevelopmentMetrics.Website.Models;

namespace DevelopmentMetrics.Website.Controllers
{
    public class BuildStabilityController : Controller
    {
        private readonly IBuildStability _buildStability;

        public BuildStabilityController(IBuildStability buildStability)
        {
            _buildStability = buildStability;
        }

        // GET: BuildStability
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DrawChart()
        {
            var metrics = _buildStability.GetBuildStabilityMetrics();

            var buildStabilityViewModel = new BuildStabilityViewModel
            {
                BuildFailureRate = new BuildCalculators().CalculateBuildFailingRateByMonthFrom(new DateTime(2017, 1, 1), metrics)
            };

            var model = new BuildStabilityChart(buildStabilityViewModel);

            return View("BuildStabilityChart", model);
        }
    }
}