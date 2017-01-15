using Microsoft.Bot.Connector;
using System.Text;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    public class ConversationUpdateReplyActivityBuilder : IReplyActivityBuilder
    {
        readonly ICameraDataService cameraDataService;

        public ConversationUpdateReplyActivityBuilder(ICameraDataService cameraDataService)
        {
            this.cameraDataService = cameraDataService;
        }

        public Activity BuildReplyActivity(Activity messageActivity, IUserData userData)
        {
            var sb = new StringBuilder();
            sb.Append("## Welcome to Traffic Cam Bot!");
            sb.Append("<br>Type 'help' to get started");
            sb.Append("<br>");
            sb.Append(SelectCityReplyActivityBuilder.NowViewingMessage);
            sb.Append(cameraDataService.Name);

            var reply = messageActivity.CreateReply(sb.ToString());
            reply.TextFormat = TextFormatTypes.Markdown;
            return reply;
        }
    }
}