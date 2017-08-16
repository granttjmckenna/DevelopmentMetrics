using System.Web.Mvc;
using DevelopmentMetrics.Models;

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
            var metrics = _buildStability.GetBuildStabilityMetrics();

            return View();
        }
    }
}