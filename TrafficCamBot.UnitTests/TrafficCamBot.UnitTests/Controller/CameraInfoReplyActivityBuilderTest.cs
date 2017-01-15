using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TrafficCamBot.Bot;
using TrafficCamBot.Controllers;

namespace TrafficCamBot.UnitTests.Controller
{
    [TestClass]
    public class CameraInfoReplyActivityBuilderTest
    {
        [TestMethod]
        public void TestCameraLookupError()
        {
            var errorInfo = new CameraLookupError("Oops!");
            var builder = new CameraInfoReplyActivityBuilder(errorInfo);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Be("Oops!");
        }

        [TestMethod]
        public void TestCameraChoiceList()
        {
            var choiceInfo = new CameraChoiceList(new List<string>
            {
                "Camera 1",
                "Camera 2"
            });
            var builder = new CameraInfoReplyActivityBuilder(choiceInfo);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(CameraInfoReplyActivityBuilder.CameraChoiceListPrompt);
            reply.Text.Should().Contain("1. Camera 1");
            reply.Text.Should().Contain("2. Camera 2");
        }

        [TestMethod]
        public void TestCameraImage()
        {
            var imageInfo = new CameraImage("Camera Name", "http://cameraurl");
            var builder = new CameraInfoReplyActivityBuilder(imageInfo);
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain("![camera](" + "http://cameraurl");
            reply.Text.Should().Contain(CameraInfoReplyActivityBuilder.ViewInBrowserPrompt);
            reply.Text.Should().Contain("Camera Name");
        }
    }
}
