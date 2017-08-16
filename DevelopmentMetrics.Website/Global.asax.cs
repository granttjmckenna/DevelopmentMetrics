using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DevelopmentMetrics.Website.DependencyResolution;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace DevelopmentMetrics.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = IoC.Initialize();
            var structureMapDependencyScope = new StructureMapDependencyScope(container);
            DependencyResolver.SetResolver(structureMapDependencyScope);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
