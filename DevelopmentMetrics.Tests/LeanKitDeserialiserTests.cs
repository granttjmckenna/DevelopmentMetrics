using System;
using System.IO;
using System.Linq;
using DevelopmentMetrics.Cards;
using DevelopmentMetrics.Models;
using DevelopmentMetrics.Repository;
using NSubstitute;
using NUnit.Framework;

namespace DevelopmentMetrics.Tests
{
    [TestFixture]
    public class LeanKitDeserialiserTests
    {
        [Test]
        public void Return_lanes_from_board_reply_data()
        {
            var leanKitWebClient = Substitute.For<ILeanKitWebClient>();

            leanKitWebClient.GetBoardData().Returns(GetJsonResponse());

            leanKitWebClient.GetCardDataFor(Arg.Any<int>()).Returns(CardDetailResponse());

            var cards = new Card(leanKitWebClient).GetCards().ToList();

            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Todo)));
            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Doing)));
            Assert.That(cards.Any(c => c.Status.Equals(CardStatus.Status.Done)));
            Assert.That(cards.All(c => c.CreateDate == new DateTime(2017, 11, 20)));
        }

        [Test]
        public void Return_cards_with_card_type_set()
        {
            var leanKitWebClient = Substitute.For<ILeanKitWebClient>();

            leanKitWebClient.GetBoardData().Returns(GetJsonResponse());

            leanKitWebClient.GetCardDataFor(Arg.Any<int>()).Returns(CardDetailResponse());

            var cards = new Card(leanKitWebClient).GetCards();

            Assert.That(cards.Any(c => c.TypeName == "Defect"));

        }

        private string GetJsonResponse()
        {
            const string filePath = @"C:\code\DevelopmentMetrics\DevelopmentMetrics.Tests\leankit json response.txt";
            string jsonResponse;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
                jsonResponse = streamReader.ReadToEnd();

            return jsonResponse;
        }

        private static string CardDetailResponse()
        {
            return @"{""ReplyData"":[{""SystemType"":""Card"",""BoardId"":311598445,""BoardTitle"":""Development Team Delivery"",""LaneId"":311726655,""LaneTitle"":""DEPLOY"",""Title"":""Log messages from API to Loggly"",""Description"":null,""Type"":{""Id"":311598710},""TypeId"":311598710,""Priority"":1,""PriorityText"":""Normal"",""TypeName"":""New Feature"",""TypeIconPath"":null,""TypeColorHex"":""#F7E308"",""Size"":0,""Active"":false,""Color"":""#F7E308"",""Icon"":""../../customicons/24/84b558/lk_icons_final_05-08.png"",""Version"":7,""AssignedUsers"":[],""IsBlocked"":false,""BlockReason"":null,""BlockStateChangeDate"":null,""Index"":0,""StartDate"":null,""DueDate"":null,""ExternalSystemName"":null,""ExternalSystemUrl"":null,""ExternalCardID"":null,""ExternalCardIdPrefix"":null,""Tags"":""PSR"",""ParentBoardId"":0,""ParentBoardIds"":[],""ParentTaskboardId"":null,""CountOfOldCards"":0,""CardContexts"":[],""LastMove"":""21/11/2017 01:22:45 PM"",""LastActivity"":""21/11/2017 01:22:45 PM"",""DateArchived"":null,""CommentsCount"":0,""LastComment"":null,""AttachmentsCount"":0,""LastAttachment"":null,""CreateDate"":""20/11/2017"",""ActualStartDate"":""11/20/2017 4:26:51 PM"",""ActualFinishDate"":""11/21/2017 1:22:45 PM"",""AssignedUserName"":"""",""AssignedUserId"":0,""AssignedUserIds"":[],""GravatarLink"":"""",""SmallGravatarLink"":"""",""IsOlderThanXDays"":false,""Id"":578974847,""DrillThroughBoardId"":null,""CardDrillThroughBoardIds"":[],""HasDrillThroughBoard"":false,""HasMultipleDrillThroughBoards"":false,""DrillThroughStatistics"":null,""DrillThroughCompletionPercent"":null,""DrillThroughProgressTotal"":null,""DrillThroughProgressComplete"":null,""DrillThroughProgressSizeComplete"":null,""DrillThroughProgressSizeTotal"":null,""ClassOfServiceId"":325770308,""ClassOfServiceTitle"":""Other applications"",""ClassOfServiceIconPath"":""../../customicons/24/84b558/lk_icons_final_05-08.png"",""ClassOfServiceColorHex"":""#FFFFFF"",""ClassOfServiceCustomIconName"":""lk_icons_final_05-08"",""ClassOfServiceCustomIconColor"":""84b558"",""CardTypeIconColor"":null,""CardTypeIconName"":null,""CurrentTaskBoardId"":null,""TaskBoardCompletionPercent"":0,""TaskBoardCompletedCardCount"":0,""TaskBoardCompletedCardSize"":0,""TaskBoardTotalCards"":0,""TaskBoardTotalSize"":0,""CurrentContext"":null,""ParentCardId"":null,""ParentCardIds"":[]}],""ReplyCode"":200,""ReplyText"":""Card successfully retrieved.""}";
        }
    }
}