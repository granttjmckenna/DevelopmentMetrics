using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class FilterBuildsTests
    {
        [Test]
        public void Return_all_builds_when_build_agent_filter_is_all()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter("All", "All"));

            Assert.That(builds.Any(b => b.AgentName.Equals("agent 1")));
            Assert.That(builds.Any(b => b.AgentName.Equals("agent 2")));
        }

        [Test]
        public void Return_all_builds_when_build_agent_filter_is_agent_1()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter("agent 1", "All"));

            Assert.That(builds.All(b => b.AgentName.Equals("agent 1")));
        }

        [Test]
        public void Return_all_builds_when_build_type_id_filter_is_all()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter("All", "All"));

            Assert.That(builds.Any(b => b.BuildTypeId.Equals("build type 1")));
            Assert.That(builds.Any(b => b.BuildTypeId.Equals("build type 2")));
        }

        [Test]
        public void Return_all_builds_when_build_type_id_filter_is_build_type_1()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter("All", "build type 1"));

            Assert.That(builds.All(b => b.BuildTypeId.Equals("build type 1")));
        }

        [Test]
        public void Return_all_builds_when_build_agent_agent_1_and_build_type_id_filter_is_build_type_1()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter("agent 1", "build type 1"));

            Assert.That(builds.All(b => b.AgentName.Equals("agent 1")));
            Assert.That(builds.All(b => b.BuildTypeId.Equals("build type 1")));
        }

        private List<Build> GetBuilds()
        {
            return new List<Build>
            {
                new Build
                {
                    BuildTypeId = "build type 2",
                    AgentName = "agent 1"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 1"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 2"
                },
                new Build
                {
                    BuildTypeId = "build type 2",
                    AgentName = "agent 2"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 1"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 2"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 1"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 1"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 2"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 1"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 2"
                },
                new Build
                {
                    BuildTypeId = "build type 1",
                    AgentName = "agent 2"
                }
            };
        }
    }
}
