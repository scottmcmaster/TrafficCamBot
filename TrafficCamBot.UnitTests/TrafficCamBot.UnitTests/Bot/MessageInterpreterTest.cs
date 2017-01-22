using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficCamBot.Bot;
using static TrafficCamBot.Bot.MessageInterpreter;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class MessageInterpreterTest
    {
        private MessageInterpreter interpreter;

        [TestInitialize]
        public void Setup()
        {
            interpreter = new MessageInterpreter();
        }

        [TestMethod]
        public void TestListRequest()
        {
            Assert.AreEqual(MessageType.ListCameras,
                interpreter.InterpretMessage(CreateActivity("list")));
            Assert.AreEqual(MessageType.ListCameras,
                interpreter.InterpretMessage(CreateActivity("LIST!!!")));
            Assert.AreEqual(MessageType.ListCameras,
                interpreter.InterpretMessage((CreateActivity("show me a List of cameras"))));

        }

        [TestMethod]
        public void TestHelpRequest()
        {
            Assert.AreEqual(MessageType.HelpRequest,
                interpreter.InterpretMessage(CreateActivity("help")));
            Assert.AreEqual(MessageType.HelpRequest,
                interpreter.InterpretMessage(CreateActivity("HELP!?!")));
            Assert.AreEqual(MessageType.HelpRequest,
                interpreter.InterpretMessage((CreateActivity("can I get a little help here?"))));
        }

        [TestMethod]
        public void TestQuery()
        {
            Assert.AreEqual(MessageType.Query,
                interpreter.InterpretMessage(CreateActivity("ne 85th street")));
        }

        Activity CreateActivity(string text)
        {
            return new Activity(type: ActivityTypes.Message, text: text);
        }
    }
}
