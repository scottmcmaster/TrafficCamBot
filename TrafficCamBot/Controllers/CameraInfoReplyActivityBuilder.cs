using log4net;
using Microsoft.Bot.Connector;
using System;
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
        public const string CardViewPrompt = "View";
        public const string NotFoundMessage = "Couldn't find that traffic camera in ";

        readonly ILog logger = LogManager.GetLogger(typeof(CameraInfoReplyActivityBuilder));
        readonly ICameraLookupData cameraInfo;
        readonly ICameraDataService cameraData;

        readonly Random rnd = new Random();

        public CameraInfoReplyActivityBuilder(ICameraLookupData cameraInfo, ICameraDataService cameraData)
        {
            this.cameraInfo = cameraInfo;
            this.cameraData = cameraData;
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
            logger.Debug("Lookup error: " + ((CameraLookupError)cameraInfo).Message);

            var sb = new StringBuilder();
            sb.Append(NotFoundMessage);
            sb.Append(cameraData.Name);
            sb.Append(".  ");

            var allCameras = cameraData.ListCameras();
            var camera1 = allCameras[rnd.Next(allCameras.Count)];
            var camera2 = allCameras[rnd.Next(allCameras.Count)];
            var prompt1 = GetRandomTryPrompt();
            var prompt2 = GetRandomTryPrompt();

            sb.Append("Why not try\n");
            sb.Append("* ");
            sb.Append(prompt1);
            sb.Append(camera1);
            sb.Append("\n");
            sb.Append("* ");
            sb.Append(prompt2);
            sb.Append(camera2);

            return activity.CreateReply(sb.ToString());
        }

        string GetRandomTryPrompt()
        {
            return CameraSearcher.TryPrompts[rnd.Next(CameraSearcher.TryPrompts.Count)];
        }

        Activity HandleCameraImageReply(Activity activity)
        {
            switch (activity.ChannelId)
            {
                case "skype":
                    return ReplyWithSimpleMarkdown(activity);
                case "teams":
                    return ReplyWithHeroCard(activity);
                default:
                    return ReplyWithCardLikeMarkdown(activity);
            }
        }

        private Activity ReplyWithSimpleMarkdown(Activity activity)
        {
            var cameraImage = (CameraImage)cameraInfo;
            var sb = new StringBuilder();
            sb.Append("![camera](");
            sb.Append(cameraImage.Url);
            sb.Append(")");
            return activity.CreateReply(sb.ToString());
        }

        private Activity ReplyWithCardLikeMarkdown(Activity activity)
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
                Title = CardViewPrompt
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