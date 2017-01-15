using Microsoft.Bot.Connector;
using System.Text;

namespace TrafficCamBot.Controllers
{
    public class HelpReplyActivityBuilder : IReplyActivityBuilder
    {
        public const string ViewHelp = "* Type 'view city-name' to pick a city for camera search.\n";
        public const string ListHelp = "* Type 'list' to see a list of available traffic cameras.\n";
        public const string SearchHelp = "* Enter the name of a camera to see the current image.\n";
        public const string HelpHelp = "* Type 'help' to see this message again.";

        public Activity BuildReplyActivity(Activity messageActivity, IUserData userData)
        {
            return HandleHelpReply(messageActivity);
        }

        Activity HandleHelpReply(Activity activity)
        {
            var sb = new StringBuilder();
            sb.Append(ViewHelp);
            sb.Append(ListHelp);
            sb.Append(SearchHelp);
            sb.Append(HelpHelp);
            var reply = activity.CreateReply(sb.ToString());
            reply.TextFormat = TextFormatTypes.Markdown;
            return reply;
        }
    }
}