using Microsoft.Bot.Connector;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    /// <summary>
    /// Wraps the conversational data.
    /// </summary>
    public class UserData : IUserData
    {
        /// <summary>
        /// Property name to store any in-flight choice list.
        /// </summary>
        private readonly string CHOICE_LIST_COOKIE = "choiceList";

        /// <summary>
        /// Property name to store the user's selected city.
        /// </summary>
        private readonly string CITY_COOKIE = "city";


        private readonly BotData botData;
        private readonly StateClient stateClient;
        private readonly string channelId;
        private readonly string userId;

        public UserData(Activity activity)
        {
            stateClient = activity.GetStateClient();
            this.channelId = activity.ChannelId;
            this.userId = activity.From.Id;
            botData = stateClient.BotState.GetUserData(channelId, userId);
        }

        public CameraChoiceList GetChoiceList()
        {
            if (botData == null)
            {
                return null;
            }
            return botData.GetProperty<CameraChoiceList>(CHOICE_LIST_COOKIE);
        }

        public void SetCity(string selectedServiceName)
        {
            if (botData != null)
            {
                botData.SetProperty(CITY_COOKIE, selectedServiceName);
                stateClient.BotState.SetUserData(channelId, userId, botData);
            }
        }

        public string GetCity()
        {
            if (botData == null)
            {
                return null;
            }
            return botData.GetProperty<string>(CITY_COOKIE);
        }

        public void SetChoiceList(CameraChoiceList cameraChoiceList)
        {
            if (botData != null)
            {
                botData.SetProperty(CHOICE_LIST_COOKIE, cameraChoiceList);
                stateClient.BotState.SetUserData(channelId, userId, botData);
            }
        }
    }
}