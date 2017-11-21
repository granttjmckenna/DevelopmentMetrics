using MvcRouteTester;
using NUnit.Framework;
using System.Web.Routing;

namespace DevelopmentMetrics.Website.Tests.Routes
{
    [TestFixture]
    public class RoutesTest
    {
        private RouteCollection _routeCollection;

        [SetUp]
        public void Setup()
        {
            _routeCollection = new RouteCollection();
            RouteConfig.RegisterRoutes(_routeCollection);
        }

        [Test]
        public void Should_return_home_controller_and_index_action()
        {
            RouteAssert.HasRoute(_routeCollection, "/", new { Controller = "Home", Action = "Index" });
        }

        [Test]
        public void Should_return_home_controller_and_index_action_when_home_specified()
        {
            RouteAssert.HasRoute(_routeCollection, "/home", new { Controller = "Home", Action = "Index" });
        }

        [Test]
        public void Should_return_home_controller_and_about_action()
        {
            RouteAssert.HasRoute(_routeCollection, "/home/about", new { Controller = "Home", Action = "About" });
        }

        [Test]
        public void Should_return_home_controller_and_contact_action()
        {
            RouteAssert.HasRoute(_routeCollection, "/home/contact", new { Controller = "Home", Action = "Contact" });
        }

        [Test]
        public void Should_return_build_stability_controller_and_index_action()
        {
            RouteAssert.HasRoute(_routeCollection, "/buildstability", new {Controller = "BuildStability", Action = "Index"});
        }

        [Test]
        public void Should_return_card_controller_and_index_action()
        {
            RouteAssert.HasRoute(_routeCollection, "/cards", new { Controller = "Cards", Action = "Index" });
        }
    }
}
