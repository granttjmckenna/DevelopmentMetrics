﻿using System.Collections.Generic;
using System.Linq;
using DevelopmentMetrics.Cards;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class TeamCityDeserialiserTests
    {
        [Test]
        public void Return_builds_from_build_data()
        {
            var teamCityWebClient = Substitute.For<ITeamCityWebClient>();

            teamCityWebClient.GetBuildDataFor(Arg.Any<string>()).Returns(GetBuildJsonResponse());

            var builds = new Build(teamCityWebClient).GetBuilds();

            Assert.That(builds.Any());
        }

        private string GetBuildJsonResponse()
        {
            return
                @"{""count"":26,""href"":""/guestAuth/app/rest/buildTypes/id:AddressService_BuildPublish/builds"",""build"":[{""id"":365628,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.24.99"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365628"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365628&buildTypeId=AddressService_BuildPublish""},{""id"":365606,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.23.98"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365606"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365606&buildTypeId=AddressService_BuildPublish""},{""id"":365597,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.22.97"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365597"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365597&buildTypeId=AddressService_BuildPublish""},{""id"":365590,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.21.96"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365590"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365590&buildTypeId=AddressService_BuildPublish""},{""id"":365331,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.20.92"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365331"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365331&buildTypeId=AddressService_BuildPublish""},{""id"":364913,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.19.91"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364913"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364913&buildTypeId=AddressService_BuildPublish""},{""id"":364873,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.18.90"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364873"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364873&buildTypeId=AddressService_BuildPublish""},{""id"":364786,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.17.86"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364786"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364786&buildTypeId=AddressService_BuildPublish""},{""id"":364785,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.16.86"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364785"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364785&buildTypeId=AddressService_BuildPublish""},{""id"":364692,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.15.83"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364692"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364692&buildTypeId=AddressService_BuildPublish""},{""id"":364384,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.14.71"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364384"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364384&buildTypeId=AddressService_BuildPublish""},{""id"":364380,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.13.71"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364380"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364380&buildTypeId=AddressService_BuildPublish""},{""id"":364021,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.12.57"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364021"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364021&buildTypeId=AddressService_BuildPublish""},{""id"":363889,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.11.52"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363889"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363889&buildTypeId=AddressService_BuildPublish""},{""id"":363886,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.10.37"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363886"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363886&buildTypeId=AddressService_BuildPublish""},{""id"":363613,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.9.33"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363613"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363613&buildTypeId=AddressService_BuildPublish""},{""id"":363611,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.8.32"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363611"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363611&buildTypeId=AddressService_BuildPublish""},{""id"":363602,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.7.29"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363602"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363602&buildTypeId=AddressService_BuildPublish""},{""id"":363601,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.6.28"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363601"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363601&buildTypeId=AddressService_BuildPublish""},{""id"":363600,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.5.23"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363600"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363600&buildTypeId=AddressService_BuildPublish""},{""id"":363599,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.4.22"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363599"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363599&buildTypeId=AddressService_BuildPublish""},{""id"":363598,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.3"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363598"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363598&buildTypeId=AddressService_BuildPublish""},{""id"":363596,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.2.21"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363596"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363596&buildTypeId=AddressService_BuildPublish""},{""id"":363567,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.1.7"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363567"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363567&buildTypeId=AddressService_BuildPublish""},{""id"":363557,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.2"",""status"":""FAILURE"",""state"":""finished"",""history"":true,""href"":""/guestAuth/app/rest/builds/id:363557"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363557&buildTypeId=AddressService_BuildPublish""},{""id"":363550,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.1"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363550"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363550&buildTypeId=AddressService_BuildPublish""}]}"; ;
        }
    }

    public class Build
    {
        private readonly ITeamCityWebClient _teamCityWebClient;
        public int Id { get; set; }
        public string BuildTypeId { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string Href { get; set; }
        public string WebUrl { get; set; }

        [JsonProperty(PropertyName = "Build")]
        public List<Build> Builds { get; set; }

        public Build(ITeamCityWebClient teamCityWebClient)
        {
            _teamCityWebClient = teamCityWebClient;
        }
        public List<Build> GetBuilds()
        {
            var buildData = _teamCityWebClient.GetBuildDataFor("");

            return JsonConvert.DeserializeObject<Build>(buildData).Builds;
        }
    }

    public interface ITeamCityWebClient
    {
        string GetBuildDataFor(string uri);
    }

    internal class TeamCityWebClient : ITeamCityWebClient
    {
        public string GetBuildDataFor(string uri)
        {
            return null;
        }
    }
}
