using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TrafficCamBot.Controllers;
using TrafficCamBot.UnitTests.Bot;

namespace TrafficCamBot.UnitTests.Controller
{
    [TestClass]
    public class SelectCityReplyActivityBuilderTest
    {
        [TestMethod]
        public void TestCityNotFound()
        {
            var builder = new SelectCityReplyActivityBuilder(new TestCameraDataServiceManager());
            var activity = ActivityTestUtils.CreateActivity();
            activity.Text = "Not found city";
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(SelectCityReplyActivityBuilder.SupportedCitiesMessage);
            reply.Text.Should().Contain("* City 1");
            reply.Text.Should().Contain("* City 2");
        }

        [TestMethod]
        public void TestCityFound()
        {
            var builder = new SelectCityReplyActivityBuilder(new TestCameraDataServiceManager());
            var activity = ActivityTestUtils.CreateActivity();
            activity.Text = "City 1";
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(SelectCityReplyActivityBuilder.NowViewingMessage + "City 1");
        }

        [TestMethod]
        public void TestCityFoundViaAlternateName()
        {
            var builder = new SelectCityReplyActivityBuilder(new TestCameraDataServiceManager());
            var activity = ActivityTestUtils.CreateActivity();
            activity.Text = "foo";
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(SelectCityReplyActivityBuilder.NowViewingMessage + "City 1");
        }
    }
}
