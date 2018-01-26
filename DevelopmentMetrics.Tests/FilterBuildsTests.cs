using System;
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

            builds = new FilterBuilds(builds).Filter(new BuildFilter(0, "All", "All"));

            Assert.That(builds.Any(b => b.AgentName.Equals("agent 1")));
            Assert.That(builds.Any(b => b.AgentName.Equals("agent 2")));
        }

        [Test]
        public void Return_all_builds_when_build_agent_filter_is_agent_1()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter(0, "agent 1", "All"));

            Assert.That(builds.All(b => b.AgentName.Equals("agent 1")));
        }

        [Test]
        public void Return_all_builds_when_build_type_id_filter_is_all()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter(0, "All", "All"));

            Assert.That(builds.Any(b => b.BuildTypeId.Equals("buildType1_A")));
            Assert.That(builds.Any(b => b.BuildTypeId.Equals("buildType2_B")));
        }

        [Test]
        public void Return_all_builds_when_build_type_id_filter_is_build_type_1()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter(0, "All", "buildType1_A"));

            Assert.That(builds.All(b => b.BuildTypeId.Equals("buildType1_A")));
        }

        [Test]
        public void Return_all_builds_when_build_agent_agent_1_and_build_type_id_filter_is_build_type_1()
        {
            var builds = GetBuilds();

            builds = new FilterBuilds(builds).Filter(new BuildFilter(0, "agent 1", "buildType1_A"));

            Assert.That(builds.All(b => b.AgentName.Equals("agent 1")));
            Assert.That(builds.All(b => b.BuildTypeId.Equals("buildType1_A")));
        }

        [Test]
        public void Return_all_builds_for_one_week()
        {
            var builds = new FilterBuilds(GetBuilds()).GetBuildsForOneWeekFrom(new DateTime(2017, 01, 01));

            Assert.That(builds.Count, Is.EqualTo(2));
        }

        [Test]
        public void Return_builds_by_build_group()
        {
            var buildGroup = new BuildGroup("buildType1_A");

            var builds = new FilterBuilds(GetBuilds()).GetBuildsFor(buildGroup);

            Assert.That(builds.All(b => b.BuildTypeId.Equals("buildType1_A", StringComparison.InvariantCultureIgnoreCase)));
            Assert.That(builds.Count, Is.EqualTo(10));
        }

        private List<Build> GetBuilds()
        {
            return new List<Build>
            {
                new Build
                {
                    BuildTypeId = "buildType2_B",
                    AgentName = "agent 1",
                    StartDateTime = new DateTime(2016,12,30)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 1",
                    StartDateTime = new DateTime(2017,01,03)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 2",
                    StartDateTime = new DateTime(2017,01,04)
                },
                new Build
                {
                    BuildTypeId = "buildType2_B",
                    AgentName = "agent 2",
                    StartDateTime = new DateTime(2017,01,08)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 1",
                    StartDateTime = new DateTime(2017,01,11)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 2",
                    StartDateTime = new DateTime(2017,01,14)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 1",
                    StartDateTime = new DateTime(2017,01,14)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 1",
                    StartDateTime = new DateTime(2017,01,14)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 2",
                    StartDateTime = new DateTime(2017,01,14)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 1",
                    StartDateTime = new DateTime(2017,01,21)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 2",
                    StartDateTime = new DateTime(2017,01,21)
                },
                new Build
                {
                    BuildTypeId = "buildType1_A",
                    AgentName = "agent 2",
                    StartDateTime = new DateTime(2017,01,25)
                }
            };
        }
    }
}
