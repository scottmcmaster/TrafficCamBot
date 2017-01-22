using log4net;
using Microsoft.Bot.Connector;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using TrafficCamBot.Bot;
using static TrafficCamBot.Bot.MessageInterpreter;

namespace TrafficCamBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        readonly ILog logger = LogManager.GetLogger(typeof(MessagesController));

        CameraDataServiceManager cameraDataServiceManager;

        readonly MessageInterpreter messageInterpreter = new MessageInterpreter();

        public MessagesController(CameraDataServiceManager cameraDataServiceManager)
        {
            this.cameraDataServiceManager = cameraDataServiceManager;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                logger.Info("ChannelId: " + activity.ChannelId + ";Query: " + activity.Text);
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var userData = new UserData(activity);

                var cameraData = GetCameraDataService(userData);

                string cameraName = null;
                var messageType = messageInterpreter.InterpretMessage(activity);

                int choice;
                var previousChoiceList = userData.GetChoiceList();
                if (previousChoiceList != null && int.TryParse(activity.Text, out choice) && choice >= 1 && choice <= previousChoiceList.CameraNames.Count)
                {
                    logger.Debug("Replying to choice selection");
                    // First see if the user is responding to a choice list.
                    // Note that we're 1-indexing for purposes of talking to the user.
                    cameraName = previousChoiceList.CameraNames[choice - 1];
                    await ReplyWithCameraLookup(activity, connector, userData, cameraData, cameraName);
                }
                else if (messageType == MessageType.PointlessChatter)
                {
                    logger.Debug("Replying with something equally pointless");
                    await connector.Conversations.ReplyToActivityAsync(
                        new PointlessPhraseReplyActivityBuilder(cameraData)
                            .BuildReplyActivity(activity, userData));
                }
                else if (messageType == MessageType.ListCameras)
                {
                    logger.Debug("Replying with list-cameras");
                    await connector.Conversations.ReplyToActivityAsync(
                        new CameraListReplyActivityBuilder(cameraData.ListCameras())
                            .BuildReplyActivity(activity, userData));
                }
                else if (messageType == MessageType.HelpRequest)
                {
                    logger.Debug("Replying with help");
                    await connector.Conversations.ReplyToActivityAsync(
                        new HelpReplyActivityBuilder()
                            .BuildReplyActivity(activity, userData));
                }
                else if (messageType == MessageType.SelectCity)
                {
                    logger.Debug("Replying to city change");
                    await connector.Conversations.ReplyToActivityAsync(
                        new SelectCityReplyActivityBuilder(cameraDataServiceManager)
                            .BuildReplyActivity(activity, userData));
                }
                else
                {
                    // Otherwise just look up the text they gave us.
                    cameraName = activity.Text;
                    logger.Debug("Replying to query: " + cameraName);
                    await ReplyWithCameraLookup(activity, connector, userData, cameraData, cameraName);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static async Task ReplyWithCameraLookup(Activity activity, ConnectorClient connector, UserData userData, ICameraDataService cameraData, string cameraName)
        {
            var cameraInfo = cameraData.Lookup(cameraName);
            var reply = new CameraInfoReplyActivityBuilder(cameraInfo, cameraData)
                .BuildReplyActivity(activity, userData);
            if (reply != null)
            {
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private ICameraDataService GetCameraDataService(UserData userData)
        {
            // First see if there is a service name set on the user data.
            ICameraDataService service = null;
            if (userData != null)
            {
                var selectedCity = userData.GetCity();
                if (selectedCity != null)
                {
                    service = cameraDataServiceManager.GetCameraDataService(selectedCity);
                }
                if (service != null)
                {
                    return service;
                }

            }

            // Then try the default.
            string defaultCameraServiceName = WebConfigurationManager.AppSettings["DefaultCameraServiceName"];
            service = cameraDataServiceManager.GetCameraDataService(defaultCameraServiceName);
            if (service != null)
            {
                return service;
            }

            // Last chance, use Seattle.
            return cameraDataServiceManager.GetCameraDataService("seattle");
        }

        private Activity HandleSystemMessage(Activity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    var connector = new ConnectorClient(new System.Uri(message.ServiceUrl));
                    var userData = new UserData(message);
                    var builder = new ConversationUpdateReplyActivityBuilder(
                        GetCameraDataService(userData));

                    connector.Conversations.ReplyToActivityAsync(
                        builder.BuildReplyActivity(message, userData));
                    break;
                case ActivityTypes.Typing:
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return null;
        }
    }
}