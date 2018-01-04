using System.Web.Mvc;

namespace DevelopmentMetrics.Website.Controllers
{
    public class CacheController : Controller
    {
        [HttpGet]
        public JsonResult GetBuildCacheStatus()
        {


            return Json("{'IsCached':'true'}");
        }
    }
}