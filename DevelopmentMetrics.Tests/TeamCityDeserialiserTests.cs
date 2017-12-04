﻿using System;
using System.Linq;
using DevelopmentMetrics.Builds;
using DevelopmentMetrics.Helpers;
using DevelopmentMetrics.Repository;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class TeamCityDeserialiserTests
    {
        private ITeamCityWebClient _teamCityWebClient;
        private ITellTheTime _tellTheTime;

        [SetUp]
        public void Setup()
        {
            _teamCityWebClient = Substitute.For<ITeamCityWebClient>();
            _tellTheTime = Substitute.For<ITellTheTime>();

            _teamCityWebClient.GetRootData().Returns(GetRootJsonResponse());
            _teamCityWebClient.GetProjectDataFor(Arg.Any<string>()).Returns(GetProjectJsonResponse());
            _teamCityWebClient.GetBuildTypeDataFor(Arg.Any<string>()).Returns(GetBuildTypeJsonResponse());
            _teamCityWebClient.GetBuildDataFor(Arg.Any<string>()).Returns(GetBuildJsonResponse());
            _teamCityWebClient.GetBuildDetailsDataFor(Arg.Any<string>()).Returns(GetBuildDetailsJsonResponse());

            _tellTheTime.ParseBuildDetailDateTimes(Arg.Any<string>()).Returns(new DateTime(2017, 01, 01));
        }

        [Test]
        public void Return_builds_from_build_data()
        {
            var builds = new Build(_teamCityWebClient, _tellTheTime).GetBuilds();

            Assert.That(builds.Any());
            Assert.That(builds.First().ProjectId, Is.EqualTo("AddressService"));
            Assert.That(builds.First().Name, Is.EqualTo("Address-Service"));
            Assert.That(builds.First().StartDateTime, Is.GreaterThanOrEqualTo(new DateTime(2015, 01, 01)));
        }

        private string GetRootJsonResponse()
        {
            return @"{""id"":""_Root"",""name"":""<Root project>"",""description"":""Contains all other projects"",""href"":""/guestAuth/app/rest/projects/id:_Root"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=_Root"",""buildTypes"":{""count"":0,""buildType"":[]},""templates"":{""count"":4,""buildType"":[{""id"":""Build"",""name"":""Build"",""templateFlag"":true,""projectName"":""<Root project>"",""projectId"":""_Root"",""href"":""/guestAuth/app/rest/buildTypes/id:Build""},{""id"":""PriceWatch_Deployment"",""name"":""Deployment"",""templateFlag"":true,""projectName"":""<Root project>"",""projectId"":""_Root"",""href"":""/guestAuth/app/rest/buildTypes/id:PriceWatch_Deployment""},{""id"":""Promote"",""name"":""Promote"",""templateFlag"":true,""projectName"":""<Root project>"",""projectId"":""_Root"",""href"":""/guestAuth/app/rest/buildTypes/id:Promote""},{""id"":""PromoteLogInNewRelic"",""name"":""Promote: Log in NewRelic"",""templateFlag"":true,""projectName"":""<Root project>"",""projectId"":""_Root"",""href"":""/guestAuth/app/rest/buildTypes/id:PromoteLogInNewRelic""}]},""parameters"":{""count"":4,""href"":""/app/rest/projects/id:_Root/parameters"",""property"":[{""name"":""fri_nuget_url"",""value"":""%nuget_publish_server%/nuget/default/"",""own"":true},{""name"":""nuget_api_key"",""value"":""ciuser:lamentabletoad"",""own"":true},{""name"":""nuget_org_url"",""value"":""https://www.nuget.org/api/v2/"",""own"":true},{""name"":""nuget_publish_server"",""value"":""http://nuget.energyhelpline.local"",""own"":true}]},""vcsRoots"":{""href"":""/guestAuth/app/rest/vcs-roots?locator=project:(id:_Root)""},""projects"":{""count"":21,""project"":[{""id"":""AddressService"",""name"":""Address-Service"",""parentProjectId"":""_Root"",""description"":""Back End Service For Core & API"",""href"":""/guestAuth/app/rest/projects/id:AddressService"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=AddressService""},{""id"":""Admin"",""name"":""Admin"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:Admin"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Admin""},{""id"":""CallCentre"",""name"":""Call Centre"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:CallCentre"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=CallCentre""},{""id"":""CodemanshipDrivingTest"",""name"":""Codemanship Driving Test"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:CodemanshipDrivingTest"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=CodemanshipDrivingTest""},{""id"":""Consumer"",""name"":""Consumer"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:Consumer"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Consumer""},{""id"":""Core"",""name"":""Core"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:Core"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Core""},{""id"":""CustomerAccountsService"",""name"":""Customer Accounts Service"",""parentProjectId"":""_Root"",""description"":""Manages customer accounts"",""href"":""/guestAuth/app/rest/projects/id:CustomerAccountsService"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=CustomerAccountsService""},{""id"":""EhlInsight"",""name"":""EHL: Insight"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:EhlInsight"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=EhlInsight""},{""id"":""ExternalLookupServices"",""name"":""External Lookup Services"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:ExternalLookupServices"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=ExternalLookupServices""},{""id"":""Forks"",""name"":""Forks"",""parentProjectId"":""_Root"",""description"":""Forks of OSS projects used internally"",""href"":""/guestAuth/app/rest/projects/id:Forks"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Forks""},{""id"":""HeatCover"",""name"":""HeatCover"",""parentProjectId"":""_Root"",""description"":""Build shared and publish to nuget"",""href"":""/guestAuth/app/rest/projects/id:HeatCover"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=HeatCover""},{""id"":""InternalTools"",""name"":""Internal Tools"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:InternalTools"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=InternalTools""},{""id"":""MfWriter"",""name"":""MF-Writer"",""parentProjectId"":""_Root"",""description"":""Write domain events to MFi"",""href"":""/guestAuth/app/rest/projects/id:MfWriter"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=MfWriter""},{""id"":""MFiMf3"",""name"":""MFi MF3"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:MFiMf3"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=MFiMf3""},{""id"":""PoolLeague"",""name"":""PoolLeague"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:PoolLeague"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=PoolLeague""},{""id"":""PriceWatch"",""name"":""PriceWatch"",""parentProjectId"":""_Root"",""description"":""Notify registered users when they can save above a threshold"",""href"":""/guestAuth/app/rest/projects/id:PriceWatch"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=PriceWatch""},{""id"":""Reporting"",""name"":""Reporting"",""parentProjectId"":""_Root"",""description"":""Sales and Market Data Reporting"",""href"":""/guestAuth/app/rest/projects/id:Reporting"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Reporting""},{""id"":""RestApi"",""name"":""Rest API"",""parentProjectId"":""_Root"",""href"":""/guestAuth/app/rest/projects/id:RestApi"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=RestApi""},{""id"":""Somb"",""name"":""SOMB"",""parentProjectId"":""_Root"",""description"":""Somb app, Somb Writer, and Somb Admin"",""href"":""/guestAuth/app/rest/projects/id:Somb"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Somb""},{""id"":""Shared"",""name"":""Shared"",""parentProjectId"":""_Root"",""description"":""Build shared and publish to nuget"",""href"":""/guestAuth/app/rest/projects/id:Shared"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Shared""},{""id"":""Tools"",""name"":""Tools"",""parentProjectId"":""_Root"",""description"":""Various tools e.g. Confirmation Email Sender"",""href"":""/guestAuth/app/rest/projects/id:Tools"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=Tools""}]}}";
        }

        private string GetProjectJsonResponse()
        {
            return
                @"{""id"":""AddressService"",""name"":""Address-Service"",""parentProjectId"":""_Root"",""description"":""Back End Service For Core & API"",""href"":""/guestAuth/app/rest/projects/id:AddressService"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=AddressService"",""parentProject"":{""id"":""_Root"",""name"":""<Root project>"",""description"":""Contains all other projects"",""href"":""/guestAuth/app/rest/projects/id:_Root"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=_Root""},""buildTypes"":{""count"":1,""buildType"":[{""id"":""AddressService_BuildPublish"",""name"":""Build & Publish"",""projectName"":""Address-Service"",""projectId"":""AddressService"",""href"":""/guestAuth/app/rest/buildTypes/id:AddressService_BuildPublish"",""webUrl"":""http://teamcity.energyhelpline.local/viewType.html?buildTypeId=AddressService_BuildPublish""}]},""templates"":{""count"":0,""buildType"":[]},""parameters"":{""count"":4,""href"":""/app/rest/projects/id:AddressService/parameters"",""property"":[{""name"":""fri_nuget_url"",""value"":""%nuget_publish_server%/nuget/default/""},{""name"":""nuget_api_key"",""value"":""ciuser:lamentabletoad""},{""name"":""nuget_org_url"",""value"":""https://www.nuget.org/api/v2/""},{""name"":""nuget_publish_server"",""value"":""http://nuget.energyhelpline.local""}]},""vcsRoots"":{""href"":""/guestAuth/app/rest/vcs-roots?locator=project:(id:AddressService)""},""projects"":{""count"":1,""project"":[{""id"":""AddressService_AddressLookup"",""name"":""AddressLookup"",""parentProjectId"":""AddressService"",""description"":""Front end tool for internal business users"",""href"":""/guestAuth/app/rest/projects/id:AddressService_AddressLookup"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=AddressService_AddressLookup""}]}}";
        }

        private string GetBuildTypeJsonResponse()
        {
            return @"{""id"":""AddressService_BuildPublish"",""name"":""Build & Publish"",""projectName"":""Address-Service"",""projectId"":""AddressService"",""href"":""/guestAuth/app/rest/buildTypes/id:AddressService_BuildPublish"",""webUrl"":""http://teamcity.energyhelpline.local/viewType.html?buildTypeId=AddressService_BuildPublish"",""project"":{""id"":""AddressService"",""name"":""Address-Service"",""parentProjectId"":""_Root"",""description"":""Back End Service For Core & API"",""href"":""/guestAuth/app/rest/projects/id:AddressService"",""webUrl"":""http://teamcity.energyhelpline.local/project.html?projectId=AddressService""},""vcs-root-entries"":{""vcs-root-entry"":[{""id"":""AddressService_AddressServiceGit"",""vcs-root"":{""id"":""AddressService_AddressServiceGit"",""name"":""Address-Service-Git"",""href"":""/guestAuth/app/rest/vcs-roots/id:AddressService_AddressServiceGit""},""checkout-rules"":""""}]},""settings"":{""count"":16,""property"":[{""name"":""allowExternalStatus"",""value"":""false""},{""name"":""artifactRules"",""value"":""""},{""name"":""buildNumberCounter"",""value"":""26""},{""name"":""buildNumberPattern"",""value"":""1.0.%build.counter%""},{""name"":""checkoutDirectory""},{""name"":""checkoutMode"",""value"":""ON_AGENT""},{""name"":""cleanBuild"",""value"":""false""},{""name"":""enableHangingBuildsDetection"",""value"":""true""},{""name"":""executionTimeoutMin"",""value"":""0""},{""name"":""maximumNumberOfBuilds"",""value"":""0""},{""name"":""shouldFailBuildIfTestsFailed"",""value"":""true""},{""name"":""shouldFailBuildOnAnyErrorMessage"",""value"":""true""},{""name"":""shouldFailBuildOnBadExitCode"",""value"":""true""},{""name"":""shouldFailBuildOnOOMEOrCrash"",""value"":""true""},{""name"":""showDependenciesChanges"",""value"":""false""},{""name"":""vcsLabelingBranchFilter"",""value"":""+:<default>""}]},""parameters"":{""count"":4,""href"":""/app/rest/buildTypes/id:AddressService_BuildPublish/parameters"",""property"":[{""name"":""fri_nuget_url"",""value"":""%nuget_publish_server%/nuget/default/""},{""name"":""nuget_api_key"",""value"":""ciuser:lamentabletoad""},{""name"":""nuget_org_url"",""value"":""https://www.nuget.org/api/v2/""},{""name"":""nuget_publish_server"",""value"":""http://nuget.energyhelpline.local""}]},""steps"":{""count"":1,""step"":[{""id"":""RUNNER_206"",""name"":"""",""type"":""jetbrains_powershell"",""properties"":{""property"":[{""name"":""jetbrains_powershell_bitness"",""value"":""x64""},{""name"":""jetbrains_powershell_errorToError"",""value"":""true""},{""name"":""jetbrains_powershell_execution"",""value"":""PS1""},{""name"":""jetbrains_powershell_script_file"",""value"":""build.ps1""},{""name"":""jetbrains_powershell_script_mode"",""value"":""FILE""},{""name"":""jetbrains_powershell_scriptArguments"",""value"":""-Target TeamCity""},{""name"":""teamcity.step.mode"",""value"":""default""}]}}]},""features"":{""count"":1,""feature"":[{""id"":""swabra"",""type"":""swabra"",""properties"":{""property"":[{""name"":""swabra.enabled"",""value"":""swabra.after.build""},{""name"":""swabra.processes"",""value"":""kill""},{""name"":""swabra.strict"",""value"":""true""}]}}]},""triggers"":{""count"":1,""trigger"":[{""id"":""vcsTrigger"",""type"":""vcsTrigger"",""properties"":{""property"":[{""name"":""groupCheckinsByCommitter"",""value"":""true""},{""name"":""perCheckinTriggering"",""value"":""true""},{""name"":""quietPeriodMode"",""value"":""DO_NOT_USE""}]}}]},""snapshot-dependencies"":{""snapshot-dependency"":[]},""artifact-dependencies"":{""artifact-dependency"":[]},""agent-requirements"":{""agent-requirement"":[]},""builds"":{""href"":""/guestAuth/app/rest/buildTypes/id:AddressService_BuildPublish/builds/""}}";
        }

        private string GetBuildJsonResponse()
        {
            return
                @"{""count"":26,""href"":""/guestAuth/app/rest/buildTypes/id:AddressService_BuildPublish/builds"",""build"":[{""id"":365628,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.24.99"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365628"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365628&buildTypeId=AddressService_BuildPublish""},{""id"":365606,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.23.98"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365606"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365606&buildTypeId=AddressService_BuildPublish""},{""id"":365597,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.22.97"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365597"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365597&buildTypeId=AddressService_BuildPublish""},{""id"":365590,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.21.96"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365590"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365590&buildTypeId=AddressService_BuildPublish""},{""id"":365331,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.20.92"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365331"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365331&buildTypeId=AddressService_BuildPublish""},{""id"":364913,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.19.91"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364913"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364913&buildTypeId=AddressService_BuildPublish""},{""id"":364873,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.18.90"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364873"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364873&buildTypeId=AddressService_BuildPublish""},{""id"":364786,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.17.86"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364786"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364786&buildTypeId=AddressService_BuildPublish""},{""id"":364785,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.16.86"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364785"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364785&buildTypeId=AddressService_BuildPublish""},{""id"":364692,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.15.83"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364692"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364692&buildTypeId=AddressService_BuildPublish""},{""id"":364384,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.14.71"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364384"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364384&buildTypeId=AddressService_BuildPublish""},{""id"":364380,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.13.71"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364380"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364380&buildTypeId=AddressService_BuildPublish""},{""id"":364021,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.12.57"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:364021"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=364021&buildTypeId=AddressService_BuildPublish""},{""id"":363889,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.11.52"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363889"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363889&buildTypeId=AddressService_BuildPublish""},{""id"":363886,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.10.37"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363886"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363886&buildTypeId=AddressService_BuildPublish""},{""id"":363613,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.9.33"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363613"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363613&buildTypeId=AddressService_BuildPublish""},{""id"":363611,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.8.32"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363611"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363611&buildTypeId=AddressService_BuildPublish""},{""id"":363602,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.7.29"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363602"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363602&buildTypeId=AddressService_BuildPublish""},{""id"":363601,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.6.28"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363601"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363601&buildTypeId=AddressService_BuildPublish""},{""id"":363600,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.5.23"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363600"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363600&buildTypeId=AddressService_BuildPublish""},{""id"":363599,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.4.22"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363599"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363599&buildTypeId=AddressService_BuildPublish""},{""id"":363598,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.3"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363598"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363598&buildTypeId=AddressService_BuildPublish""},{""id"":363596,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.2.21"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363596"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363596&buildTypeId=AddressService_BuildPublish""},{""id"":363567,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.1.7"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363567"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363567&buildTypeId=AddressService_BuildPublish""},{""id"":363557,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.2"",""status"":""FAILURE"",""state"":""finished"",""history"":true,""href"":""/guestAuth/app/rest/builds/id:363557"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363557&buildTypeId=AddressService_BuildPublish""},{""id"":363550,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.1"",""status"":""FAILURE"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:363550"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=363550&buildTypeId=AddressService_BuildPublish""}]}";
        }

        private string GetBuildDetailsJsonResponse()
        {
            return @"{""id"":365628,""buildTypeId"":""AddressService_BuildPublish"",""number"":""1.0.24.99"",""status"":""SUCCESS"",""state"":""finished"",""href"":""/guestAuth/app/rest/builds/id:365628"",""webUrl"":""http://teamcity.energyhelpline.local/viewLog.html?buildId=365628&buildTypeId=AddressService_BuildPublish"",""statusText"":""Tests passed: 158"",""buildType"":{""id"":""AddressService_BuildPublish"",""name"":""Build & Publish"",""projectName"":""Address-Service"",""projectId"":""AddressService"",""href"":""/guestAuth/app/rest/buildTypes/id:AddressService_BuildPublish"",""webUrl"":""http://teamcity.energyhelpline.local/viewType.html?buildTypeId=AddressService_BuildPublish""},""tags"":{""tag"":[]},""queuedDate"":""20170801T162114+0100"",""startDate"":""20170801T162200+0100"",""finishDate"":""20170801T162324+0100"",""triggered"":{""type"":""vcs"",""details"":""jetbrains.git"",""date"":""20170801T162114+0100""},""lastChanges"":{""count"":1,""change"":[{""id"":52551,""version"":""0ec716ed279aef4f996ace88f40ef4044fd01493"",""username"":""francis programmer"",""date"":""20170801T162020+0100"",""href"":""/guestAuth/app/rest/changes/id:52551"",""webLink"":""http://teamcity.energyhelpline.local/viewModification.html?modId=52551&personal=false""}]},""changes"":{""count"":1,""href"":""/guestAuth/app/rest/changes?locator=build:(id:365628)""},""revisions"":{""revision"":[{""version"":""0ec716ed279aef4f996ace88f40ef4044fd01493"",""vcs-root-instance"":{""id"":""822"",""vcs-root-id"":""AddressService_AddressServiceGit"",""name"":""Address-Service-Git"",""href"":""/guestAuth/app/rest/vcs-root-instances/id:822""}}]},""agent"":{""id"":38,""name"":""lon-devtcagent5"",""typeId"":38,""href"":""/guestAuth/app/rest/agents/id:38""},""testOccurrences"":{""count"":158,""href"":""/guestAuth/app/rest/testOccurrences?locator=build:(id:365628)"",""passed"":158,""default"":false},""artifacts"":{""href"":""/guestAuth/app/rest/builds/id:365628/artifacts/children""},""relatedIssues"":{""href"":""/guestAuth/app/rest/builds/id:365628/relatedIssues""},""properties"":{""count"":4,""property"":[{""name"":""fri_nuget_url"",""value"":""%nuget_publish_server%/nuget/default/""},{""name"":""nuget_api_key"",""value"":""ciuser:lamentabletoad""},{""name"":""nuget_org_url"",""value"":""https://www.nuget.org/api/v2/""},{""name"":""nuget_publish_server"",""value"":""http://nuget.energyhelpline.local""}]},""statistics"":{""href"":""/guestAuth/app/rest/builds/id:365628/statistics""}}";
        }
    }
}