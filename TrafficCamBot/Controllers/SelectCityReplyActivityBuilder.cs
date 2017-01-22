using Microsoft.Bot.Connector;
using System.Text;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    public class SelectCityReplyActivityBuilder : IReplyActivityBuilder
    {
        public const string SupportedCitiesMessage = "Supported cities are:\n";
        public const string NowViewingMessage = "Now viewing cameras for ";

        readonly ICameraDataServiceManager cameraDataServiceManager;

        public SelectCityReplyActivityBuilder(ICameraDataServiceManager cameraDataServiceManager)
        {
            this.cameraDataServiceManager = cameraDataServiceManager;
        }

        public Activity BuildReplyActivity(Activity messageActivity, IUserData userData)
        {
            return HandleSelectCityResponse(messageActivity, userData);
        }

        private Activity HandleSelectCityResponse(Activity activity, IUserData userData)
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
                var result = activity.CreateReply(NowViewingMessage + selectedServiceName);
                userData.SetCity(selectedServiceName);
                return result;
            }
            else
            {
                var sb = new StringBuilder(SupportedCitiesMessage);
                foreach (string serviceName in cameraDataServiceManager.GetCameraDataServiceNames())
                {
                    sb.Append("* ");
                    sb.Append(serviceName);
                    sb.Append("\n");
                }
                sb.Append("I hope to learn about more cameras in the future.");
                return activity.CreateReply(sb.ToString());
            }
        }
    }
}