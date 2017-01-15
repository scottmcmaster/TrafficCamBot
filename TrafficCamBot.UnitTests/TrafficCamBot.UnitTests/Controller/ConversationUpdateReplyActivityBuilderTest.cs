using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TrafficCamBot.Bot;
using TrafficCamBot.Controllers;

namespace TrafficCamBot.UnitTests.Controller
{
    [TestClass]
    public class ConversationUpdateReplyActivityBuilderTest
    {
        [TestMethod]
        public void TestIntroMessage()
        {
            var cameraDataService = new Mock<ICameraDataService>();
            cameraDataService.SetupGet(cds => cds.Name).Returns("Cleveland");

            var builder = new ConversationUpdateReplyActivityBuilder(cameraDataService.Object);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(SelectCityReplyActivityBuilder.NowViewingMessage + "Cleveland");
        }
    }
}
