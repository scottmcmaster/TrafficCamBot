using Microsoft.Bot.Connector;

namespace TrafficCamBot.UnitTests.Controller
{
    public static class ActivityTestUtils
    {
        public static Activity CreateActivity()
        {
            var activity = new Activity();
            activity.Recipient = new ChannelAccount("recipientId", "recipientName");
            activity.From = new ChannelAccount("fromId", "fromName");
            activity.Conversation = new ConversationAccount(false, "conversationId", "conversationName");
            return activity;
        }
    }
}
