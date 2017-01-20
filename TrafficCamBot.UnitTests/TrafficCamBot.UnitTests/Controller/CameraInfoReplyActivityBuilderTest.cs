using FluentAssertions;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using TrafficCamBot.Bot;
using TrafficCamBot.Controllers;

namespace TrafficCamBot.UnitTests.Controller
{
    [TestClass]
    public class CameraInfoReplyActivityBuilderTest
    {
        private Mock<ICameraDataService> cameraData;

        [TestInitialize]
        public void Setup()
        {
            cameraData = new Mock<ICameraDataService>();
            cameraData.Setup(c => c.ListCameras()).Returns(new List<string> { "camera 1", "camera 2" });
        }

        [TestMethod]
        public void TestCameraLookupError()
        {
            var errorInfo = new CameraLookupError("Oops!");
            var builder = new CameraInfoReplyActivityBuilder(errorInfo, cameraData.Object);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().StartWith(CameraInfoReplyActivityBuilder.NotFoundMessage);
        }

        [TestMethod]
        public void TestCameraChoiceList()
        {
            var choiceInfo = new CameraChoiceList(new List<string>
            {
                "Camera 1",
                "Camera 2"
            });
            var builder = new CameraInfoReplyActivityBuilder(choiceInfo, cameraData.Object);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(CameraInfoReplyActivityBuilder.CameraChoiceListPrompt);
            reply.Text.Should().Contain("1. Camera 1");
            reply.Text.Should().Contain("2. Camera 2");
        }

        [TestMethod]
        public void TestCameraImageDefault()
        {
            var imageInfo = new CameraImage("Camera Name", "http://cameraurl");
            var builder = new CameraInfoReplyActivityBuilder(imageInfo, cameraData.Object);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain("![camera](" + "http://cameraurl");
            reply.Text.Should().Contain(CameraInfoReplyActivityBuilder.ViewInBrowserPrompt);
            reply.Text.Should().Contain("Camera Name");
        }

        [TestMethod]
        public void TestCameraImageSkype()
        {
            var imageInfo = new CameraImage("Camera Name", "http://cameraurl");
            var builder = new CameraInfoReplyActivityBuilder(imageInfo, cameraData.Object);
            var activity = ActivityTestUtils.CreateActivity();
            activity.ChannelId = "skype";
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain("![camera](" + "http://cameraurl");
            reply.Text.Should().NotContain(CameraInfoReplyActivityBuilder.ViewInBrowserPrompt);
        }

        [TestMethod]
        public void TestCameraImageTeams()
        {
            var imageInfo = new CameraImage("Camera Name", "http://cameraurl");
            var builder = new CameraInfoReplyActivityBuilder(imageInfo, cameraData.Object);
            var activity = ActivityTestUtils.CreateActivity();
            activity.ChannelId = "teams";
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Type.Should().Be("message");
            reply.Attachments.Count.Should().Be(1);
            var heroCard = reply.Attachments.First().Content as HeroCard;
            heroCard.Buttons.Count.Should().Be(1);
            heroCard.Title.Should().Be("Camera Name");
            heroCard.Buttons[0].Value.Should().Be("http://cameraurl");
            heroCard.Buttons[0].Type.Should().Be("openUrl");
            heroCard.Buttons[0].Title.Should().Be(CameraInfoReplyActivityBuilder.CardViewPrompt);
        }
    }
}
