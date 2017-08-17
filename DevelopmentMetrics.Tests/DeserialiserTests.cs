using System;
using System.Linq;
using DevelopmentMetrics.Models;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class DeserialiserTests
    {
        private FakeRepository _fakeRepository;

        [SetUp]
        public void SetUp()
        {
            _fakeRepository = new FakeRepository();
        }

        [Test]
        public void Should_deserialise_json_to_root_project_object()
        {
            var expected = GetExpectedRootProject();

            var rootProject = new RootProject(_fakeRepository).GetProject();

            Assert.AreEqual(expected, rootProject);
            Assert.That(rootProject.Projects.ProjectList.Any());
        }

        [Test]
        public void Should_deserialise_json_to_build_types()
        {
            var projectBuildTypes = new ProjectBuildTypes(_fakeRepository).GetProjectBuildTypesFor("BuildTypes");

            Assert.That(projectBuildTypes.BuildTypes.BuildTypeList.Any());
            Assert.That(projectBuildTypes.BuildTypes.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_deserialise_json_to_builds()
        {
            var builds =
                new ProjectBuild(_fakeRepository).GetBuildsFor(
                    "/guestAuth/app/rest/buildTypes/id:Admin_00UpdateCoreNuGetPackages/builds");

            Assert.That(builds.Any());
        }

        [Test]
        public void Should_deserialise_json_to_build_details()
        {
            var buildDetail = new BuildDetail(_fakeRepository).GetBuildDetailsFor("/guestAuth/app/rest/builds/id:363550");

            Assert.That(buildDetail.Id, Is.EqualTo(365628));
            Assert.That(buildDetail.BuildTypeId, Is.EqualTo("AddressService_BuildPublish"));
            Assert.That(buildDetail.AgentDto.Name, Is.EqualTo("lon-devtcagent5"));
            Assert.That(!string.IsNullOrWhiteSpace(buildDetail.StartDateTime));
            Assert.That(!string.IsNullOrWhiteSpace(buildDetail.FinishDateTime));
            Assert.That(!string.IsNullOrWhiteSpace(buildDetail.QueuedDateTime));
            Assert.That(buildDetail.State.Equals("Finished", StringComparison.InvariantCultureIgnoreCase));
            Assert.That(buildDetail.Status.Equals("Success", StringComparison.InvariantCultureIgnoreCase));
        }

        private static RootProject GetExpectedRootProject()
        {
            return new RootProject(new FakeRepository())
            {
                Id = "_Root",
                Name = "<Root project>",
                Description = "Contains all other projects",
                Href = "/guestAuth/app/rest/projects/id:_Root",
                WebUrl = "http://teamcity.energyhelpline.local/project.html?projectId=_Root",
                Projects = new ProjectsDto
                {
                    Count = 17
                }
            };
        }
    }
}