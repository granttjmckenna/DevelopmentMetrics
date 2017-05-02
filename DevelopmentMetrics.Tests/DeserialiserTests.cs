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
            var returnedJson = _fakeRepository.GetRoot();

            var expected = GetExpectedRootProject();

            var rootProject = new RootProject(_fakeRepository).GetProject(returnedJson);

            Assert.AreEqual(expected, rootProject);
            Assert.That(rootProject.Projects.ProjectList.Any());
        }

        [Test]
        public void Should_deserialise_json_to_project_builds()
        {
            var returnedJson = _fakeRepository.GetProjectBuild();

            var projectBuild = new ProjectBuild(_fakeRepository).GetBuilds(returnedJson);

            Assert.That(projectBuild.BuildList.Any());
            Assert.That(projectBuild.BuildList.Count, Is.EqualTo(200));
        }

        [Test]
        public void Should_deserialise_json_to_build_details()
        {
            var returnedJson = _fakeRepository.GetBuildDetails();

            var buildDetail = new BuildDetail(_fakeRepository).GetDetails(returnedJson);

            Assert.That(buildDetail.Id, Is.EqualTo(360907));
            Assert.That(buildDetail.BuildTypeId, Is.EqualTo("Consumer_Funnel_31ProductionSmokeTests"));
            Assert.That(buildDetail.Agent.Name, Is.EqualTo("lon-devtcagent3"));
            Assert.IsNotNull(buildDetail.StartDateTime);
            Assert.IsNotNull(buildDetail.FinishDateTime);
            Assert.IsNotNull(buildDetail.QueuedDateTime);
            Assert.IsNotNull(buildDetail.State);
            Assert.IsNotNull(buildDetail.Status);
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
                Projects = new Projects
                {
                    Count = 17
                }
            };
        }
    }
}