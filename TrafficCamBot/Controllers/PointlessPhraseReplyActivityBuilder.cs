using Microsoft.Bot.Connector;
using System;
using System.Text;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    /// <summary>
    /// Replies using the pointless-chatter generator.
    /// </summary>
    public class PointlessPhraseReplyActivityBuilder : IReplyActivityBuilder
    {
        readonly ICameraDataService cameraData;

        enum ResponseType
        {
            SuggestHelp1,
            SuggestHelp2,
            SuggestView,
            SuggestCameraSearches1,
            SuggestCameraSearches2
        }

        readonly static ResponseType[] ResponseTypes = (ResponseType[])Enum.GetValues(typeof(ResponseType));

        readonly PointlessChatterGenerator chatterGenerator = new PointlessChatterGenerator();
        readonly Random rnd = new Random();

        public PointlessPhraseReplyActivityBuilder(ICameraDataService cameraData)
        {
            this.cameraData = cameraData;
        }

        public Activity BuildReplyActivity(Activity messageActivity, IUserData userData)
        {
            var pointlessResponse = chatterGenerator.GeneratePointlessResponse(messageActivity.Text);
            var sb = new StringBuilder();
            sb.Append(pointlessResponse);
            if (PointlessChatterGenerator.ShouldMakeSuggestion(pointlessResponse))
            {
                sb.Append(' ');
                GenerateSuggestion(sb);
            }

            return messageActivity.CreateReply(sb.ToString());
        }

        void GenerateSuggestion(StringBuilder sb)
        {
            var responseType = ResponseTypes[rnd.Next(ResponseTypes.Length)];
            switch (responseType)
            {
                case ResponseType.SuggestCameraSearches1:
                    sb.Append("Try searching for a traffic camera, for example *");
                    sb.Append(GetSearchSuggestion());
                    sb.Append("*");
                    break;

                case ResponseType.SuggestHelp2:
                    sb.Append("If you need some ideas, type 'help'.");
                    break;

                case ResponseType.SuggestCameraSearches2:
                    sb.Append("You can search for a traffic camera like this: *");
                    sb.Append(GetSearchSuggestion());
                    sb.Append('*');
                    break;

                case ResponseType.SuggestView:
                    sb.Append("Try asking to 'view' cameras for a city.");
                    break;

#pragma warning disable RECS0073 // Redundant case label
                case ResponseType.SuggestHelp1:
#pragma warning restore RECS0073 // Redundant case label
                default:
                    sb.Append("Type 'help' to get started.");
                    break;
            }
        }

        string GetSearchSuggestion()
        {
            return new CameraSearchSuggestionBuilder(cameraData).BuildSearchSuggestion();
        }
    }
}