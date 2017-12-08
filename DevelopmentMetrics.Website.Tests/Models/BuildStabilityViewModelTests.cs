using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Website.Models;
using NUnit.Framework;

namespace DevelopmentMetrics.Website.Tests.Models
{

    [TestFixture]
    public class BuildStabilityViewModelTests
    {
        [Test]
        public void Return_empty_display_list_when_build_type_ids_is_empty()
        {
            var displayList = new BuildStabilityViewModel(new List<string>()).GetBuildTypeIdList();

            Assert.That(displayList.Count, Is.EqualTo(0));
        }

        [Test]
        public void Return_disinct_display_list_of_build_type_ids()
        {
            var buildTypeIds = new List<string> { "buildtype1_1", "buildtype1_1", "buildtype2_1", "buildtype2_2" };

            var displayList = new BuildStabilityViewModel(buildTypeIds).GetBuildTypeIdList();

            Assert.That(displayList.Count, Is.EqualTo(2));
        }

        [Test]
        public void Return_disinct_display_list_of_build_type_ids_sorted_alphabetically()
        {
            var buildTypeIds = new List<string> { "buildtype2_2", "buildtype1_1", "buildtype1_1", "buildtype2_1", "buildtype2_2" };

            var displayList = new BuildStabilityViewModel(buildTypeIds).GetBuildTypeIdList();

            Assert.That(displayList.Count, Is.EqualTo(2));
            Assert.That(displayList.First(), Is.EqualTo("buildtype1"));
        }
    }
}
