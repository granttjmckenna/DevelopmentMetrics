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
            var returnedJson = _fakeRepository.GetJsonFor("_root");

            var expected = GetExpectedRootProject();

            var rootProject = new RootProject(_fakeRepository).GetProject(returnedJson);

            Assert.AreEqual(expected, rootProject);
            Assert.That(rootProject.Projects.ProjectList.Any());
        }

        [Test]
        public void Should_deserialise_json_to_project_builds()
        {
            var projects = new ProjectBuild(_fakeRepository).GetBuildsFor("projects");

            Assert.That(projects.Any());
            Assert.That(projects.Count, Is.EqualTo(200));
        }

        [Test]
        public void Should_deserialise_json_to_build_details()
        {
            var buildDetail = new BuildDetail(_fakeRepository).GetBuildDetailsFor("builds/id");

            Assert.That(buildDetail.Id, Is.EqualTo(360907));
            Assert.That(buildDetail.BuildTypeId, Is.EqualTo("Consumer_Funnel_31ProductionSmokeTests"));
            Assert.That(buildDetail.AgentDto.Name, Is.EqualTo("lon-devtcagent3"));
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