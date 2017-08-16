using System;
using System.Linq;
using DevelopmentMetrics.Models;
using Newtonsoft.Json;
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
            var returnedJson = _fakeRepository.GetDataFor("_root");

            var expected = GetExpectedRootProject();

            var rootProject = new RootProject(_fakeRepository).GetProject(returnedJson);

            Assert.AreEqual(expected, rootProject);
            Assert.That(rootProject.Projects.ProjectList.Any());
        }

        [Test]
        public void Should_deserialise_json_to_project_builds()
        {
            var buildJson = _fakeRepository.GetDataFor("projects");

            var projectBuildTypes = JsonConvert.DeserializeObject<ProjectBuildTypes>(buildJson);

            Assert.That(projectBuildTypes.BuildTypes.BuildTypeList.Any());
        }

        [Test]
        public void Should_deserialise_json_to_build_types()
        {
            var buildJson = _fakeRepository.GetDataFor("BuildTypes");

            var projectBuildTypes = JsonConvert.DeserializeObject<ProjectBuildTypes>(buildJson);

            Assert.That(projectBuildTypes.BuildTypes.BuildTypeList.Any());
            Assert.That(projectBuildTypes.BuildTypes.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_deserialise_json_to_build_details()
        {
            var buildDetail = new BuildDetail(_fakeRepository).GetBuildDetailsFor("builds");

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