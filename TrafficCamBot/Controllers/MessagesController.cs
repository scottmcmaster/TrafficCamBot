using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using log4net;
using System.Text;
using System.Collections.Generic;
using TrafficCamBot.Bot;
using static TrafficCamBot.Bot.MessageInterpreter;
using System;
using System.Net.Http;
using System.Net;
using System.Web.Configuration;

namespace TrafficCamBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(MessagesController));

        private CameraDataServiceManager cameraDataServiceManager;

        private readonly MessageInterpreter messageInterpreter = new MessageInterpreter();

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
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var userData = new UserData(activity);

            if (activity.Type == ActivityTypes.Message)
            {
                logger.Debug("Query text: " + activity.Text);

                var cameraData = GetCameraDataService(userData);
                
                string cameraName = null;
                var messageType = messageInterpreter.InterpretMessage(activity);

                int choice;
                var previousChoiceList = userData.GetChoiceList();
                if (previousChoiceList != null && int.TryParse(activity.Text, out choice) && choice >= 1 && choice <= previousChoiceList.CameraNames.Count)
                {
                    // First see if the user is responding to a choice list.
                    // Note that we're 1-indexing for purposes of talking to the user.
                    cameraName = previousChoiceList.CameraNames[choice - 1];
                } else if (messageType == MessageType.LIST_CAMERAS)
                {
                    await connector.Conversations.ReplyToActivityAsync(
                        HandleCameraListReply(activity, cameraData.ListCameras()));
                } else if (messageType == MessageType.HELP_REQUEST)
                {
                    await connector.Conversations.ReplyToActivityAsync(
                        HandleHelpReply(activity));
                } else if (messageType == MessageType.SELECT_CITY)
                {
                    await connector.Conversations.ReplyToActivityAsync(
                        HandleSelectCityResponse(activity, userData));
                }
                else
                {
                    // Otherwise just look up the text they gave us.
                    cameraName = activity.Text;
                }

                var cameraInfo = cameraData.Lookup(cameraName);
                if (cameraInfo is CameraImage)
                {
                    await connector.Conversations.ReplyToActivityAsync(
                        HandleCameraImageReply(activity, cameraInfo));
                }
                else if (cameraInfo is CameraLookupError)
                {
                    await connector.Conversations.ReplyToActivityAsync(
                        HandleCameraLookupErrorReply(activity, cameraInfo));
                }
                else if (cameraInfo is CameraChoiceList)
                {
                    await connector.Conversations.ReplyToActivityAsync(
                        HandleCameraChoiceListReply(activity, cameraInfo, userData));
                }
            }
            var reply = HandleSystemMessage(activity);
            await connector.Conversations.ReplyToActivityAsync(reply);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSelectCityResponse(Activity activity, UserData userData)
        {
            var normalizedText = activity.Text.ToLower();
            string selectedServiceName = null;
            foreach (string serviceName in cameraDataServiceManager.GetCameraDataServiceNames())
            {
                if (normalizedText.Contains(serviceName.ToLower()))
                {
                    selectedServiceName = serviceName;
                    break;
                }
            }
            if (selectedServiceName != null)
            {
                var result = activity.CreateReply("Now viewing cameras for " + selectedServiceName);
                userData.SetCity(selectedServiceName);
                return result;
            } else
            {
                var sb = new StringBuilder("Supported cities are:  \n");
                foreach (string serviceName in cameraDataServiceManager.GetCameraDataServiceNames())
                {
                    sb.Append(serviceName);
                    sb.Append("  \n");
                }
                return activity.CreateReply(sb.ToString());
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

        private Activity HandleHelpReply(Activity activity)
        {
            var sb = new StringBuilder();
            sb.Append("Type 'list' to see a list of available traffic cameras.  \n");
            sb.Append("Enter the name of a camera to see the current image.  \n");
            sb.Append("Type 'help' to see this message again.");
            return activity.CreateReply(sb.ToString());
        }

        private Activity HandleCameraChoiceListReply(Activity activity, ICameraLookupData cameraInfo, UserData userData)
        {
            var sb = new StringBuilder();
            sb.Append("Did you mean:\n");
            int i = 1;
            foreach (string choiceName in (cameraInfo as CameraChoiceList).CameraNames)
            {
                sb.Append($"{i++}. {choiceName}  \n");
            }

            var replyMessage = activity.CreateReply(sb.ToString());
            userData.SetChoiceList(cameraInfo as CameraChoiceList);
            return replyMessage;
        }

        private static Activity HandleCameraListReply(Activity activity, IList<string> cameraList)
        {
            if (cameraList.Count == 0)
            {
                return activity.CreateReply("No cameras found");
            }
            return activity.CreateReply(string.Join(",", cameraList));
        }

        private static Activity HandleCameraLookupErrorReply(Activity activity, ICameraLookupData cameraInfo)
        {
            return activity.CreateReply(((CameraLookupError)cameraInfo).Message);
        }

        private static Activity HandleCameraImageReply(Activity activity, ICameraLookupData cameraInfo)
        {
            return activity.CreateReply(
                "![camera](" + ((CameraImage)cameraInfo).Url + ")");
        }

        private Activity HandleSystemMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Ping:
                    var reply = activity.CreateReply();
                    reply.Type = "Ping";
                    return reply;
            }

            return null;
        }
    }
}