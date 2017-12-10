using System;
using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Website.Models;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Website.Tests.Models
{

    [TestFixture]
    public class BuildStabilityViewModelTests
    {
        private ITellTheTime _tellTheTime;

        [SetUp]
        public void Setup()
        {
            _tellTheTime = Substitute.For<ITellTheTime>();
        }

        [Test]
        public void Return_empty_display_list_when_build_type_ids_is_empty()
        {
            var displayList = new BuildStabilityViewModel(new List<Build>(), _tellTheTime).GetBuildTypeIdList();

            Assert.That(displayList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Return_disinct_display_list_of_build_type_ids()
        {
            var builds = new List<Build>
            {
                GetBuild("buildtype1_1"),
                GetBuild("buildtype1_1"),
                GetBuild("buildtype2_1"),
                GetBuild("buildtype2_2")
            };

            var displayList = new BuildStabilityViewModel(builds, _tellTheTime).GetBuildTypeIdList();

            Assert.That(displayList.Count, Is.EqualTo(2));
        }

        [Test]
        public void Return_disinct_display_list_of_build_type_ids_sorted_alphabetically()
        {
            var builds = new List<Build>
            {
                GetBuild("buildtype2_2"),
                GetBuild("buildtype1_1"),
                GetBuild("buildtype1_1"),
                GetBuild("buildtype2_1"),
                GetBuild("buildtype2_2")
            };

            var displayList = new BuildStabilityViewModel(builds, _tellTheTime).GetBuildTypeIdList();

            Assert.That(displayList.Count, Is.EqualTo(2));
            Assert.That(displayList.First(), Is.EqualTo("buildtype1"));
        }

        private static Build GetBuild(string buildTypeId)
        {
            return new Build { BuildTypeId = buildTypeId };
        }

    }
}
