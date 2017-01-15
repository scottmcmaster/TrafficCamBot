using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TrafficCamBot.Controllers;

namespace TrafficCamBot.UnitTests.Controller
{
    [TestClass]
    public class HelpReplyActivityBuilderTest
    {
        [TestMethod]
        public void TestHelp()
        {
            var builder = new HelpReplyActivityBuilder();
            var activity = ActivityTestUtils.CreateActivity();
            var userData = new Mock<IUserData>();

            var reply = builder.BuildReplyActivity(activity, userData.Object);
            reply.Text.Should().Contain(HelpReplyActivityBuilder.HelpHelp);
            reply.Text.Should().Contain(HelpReplyActivityBuilder.ListHelp);
            reply.Text.Should().Contain(HelpReplyActivityBuilder.SearchHelp);
            reply.Text.Should().Contain(HelpReplyActivityBuilder.ViewHelp);
        }
    }
}
