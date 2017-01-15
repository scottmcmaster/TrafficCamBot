using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TrafficCamBot.Controllers;

namespace TrafficCamBot.UnitTests.Controller
{
    [TestClass]
    public class CameraListReplyActivityBuilderTest
    {
        [TestMethod]
        public void TestEmptyList()
        {
            var builder = new CameraListReplyActivityBuilder(new List<string>());
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            Assert.AreEqual(CameraListReplyActivityBuilder.NoCamerasFoundMessage,
                reply.Text);
        }

        [TestMethod]
        public void TestOneItem()
        {
            var builder = new CameraListReplyActivityBuilder(new List<string> { "Camera Name 1" });
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Be("* Camera Name 1\n");
        }

        [TestMethod]
        public void TestTwoItems()
        {
            var builder = new CameraListReplyActivityBuilder(
                new List<string> { "Camera Name 1", "Camera Name 2" });
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain("* Camera Name 1\n");
            reply.Text.Should().Contain("* Camera Name 2\n");
        }
    }
}
