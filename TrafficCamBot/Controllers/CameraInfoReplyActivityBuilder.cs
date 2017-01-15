using log4net;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Text;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    /// <summary>
    /// Handles replies based on the CameraInfo data.
    /// </summary>
    public class CameraInfoReplyActivityBuilder : IReplyActivityBuilder
    {
        public const string CameraChoiceListPrompt = "Did you mean:\n";
        public const string ViewInBrowserPrompt = "View in browser";

        readonly ILog logger = LogManager.GetLogger(typeof(CameraInfoReplyActivityBuilder));
        readonly ICameraLookupData cameraInfo;

        public CameraInfoReplyActivityBuilder(ICameraLookupData cameraInfo)
        {
            this.cameraInfo = cameraInfo;
        }

        public Activity BuildReplyActivity(Activity messageActivity, IUserData userData)
        {
            if (cameraInfo is CameraImage)
            {
                return HandleCameraImageReply(messageActivity);
            }
            else if (cameraInfo is CameraLookupError)
            {
                return HandleCameraLookupErrorReply(messageActivity);
            }
            else if (cameraInfo is CameraChoiceList)
            {
                return HandleCameraChoiceListReply(messageActivity, userData);
            }

            logger.Error("Unknown cameraInfo type: " + cameraInfo.GetType());
            return null;
        }

        Activity HandleCameraLookupErrorReply(Activity activity)
        {
            return activity.CreateReply(((CameraLookupError)cameraInfo).Message);
        }

        Activity HandleCameraImageReply(Activity activity)
        {
            return ReplyWithMarkdown(activity);
            //return ReplyWithHeroCard(activity);
        }

        private Activity ReplyWithMarkdown(Activity activity)
        {
            var cameraImage = (CameraImage)cameraInfo;
            var sb = new StringBuilder();
            sb.Append("## ");
            sb.Append(cameraImage.Name);
            sb.Append("<br>![camera](");
            sb.Append(cameraImage.Url);
            sb.Append(" \"");
            sb.Append(cameraImage.Name);
            sb.Append("\")");
            sb.Append("<br>[");
            sb.Append(ViewInBrowserPrompt);
            sb.Append("](");
            sb.Append(cameraImage.Url);
            sb.Append(")");

            return activity.CreateReply(sb.ToString());
        }

        private Activity ReplyWithHeroCard(Activity activity)
        {
            var cameraImage = (CameraImage)cameraInfo;
            var reply = activity.CreateReply();
            reply.Recipient = activity.From;
            reply.Type = "message";
            reply.Attachments = new List<Attachment>();
            var cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: cameraImage.Url));
            var cardButtons = new List<CardAction>();
            var plButton = new CardAction
            {
                Value = cameraImage.Url,
                Type = "openUrl",
                Title = "View"
            };
            cardButtons.Add(plButton);
            var plCard = new HeroCard
            {
                Title = cameraImage.Name,
                Subtitle = "",
                Images = cardImages,
                Buttons = cardButtons
            };
            var plAttachment = plCard.ToAttachment();
            reply.Attachments.Add(plAttachment);
            return reply;
        }

        Activity HandleCameraChoiceListReply(Activity activity, IUserData userData)
        {
            var sb = new StringBuilder();
            sb.Append(CameraChoiceListPrompt);
            int i = 1;
            foreach (string choiceName in (cameraInfo as CameraChoiceList).CameraNames)
            {
                sb.Append($"{i++}. {choiceName}  \n");
            }

            var replyMessage = activity.CreateReply(sb.ToString());
            userData.SetChoiceList(cameraInfo as CameraChoiceList);
            return replyMessage;
        }
    }
}