using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Text;

namespace TrafficCamBot.Controllers
{
    public class CameraListReplyActivityBuilder : IReplyActivityBuilder
    {
        public const string NoCamerasFoundMessage = "No cameras found";

        readonly IList<string> cameraList;

        public CameraListReplyActivityBuilder(IList<string> cameraList)
        {
            this.cameraList = cameraList;
        }

        public Activity BuildReplyActivity(Activity messageActivity, IUserData userData)
        {
            return HandleCameraListReply(messageActivity);
        }

        Activity HandleCameraListReply(Activity activity)
        {
            if (cameraList.Count == 0)
            {
                return activity.CreateReply(NoCamerasFoundMessage);
            }
            var sb = new StringBuilder();
            foreach (var camera in cameraList)
            {
                sb.Append("* ");
                sb.Append(camera);
                sb.Append("\n");
            }
            var reply = activity.CreateReply(sb.ToString());
            reply.TextFormat = TextFormatTypes.Markdown;
            return reply;
        }
    }
}