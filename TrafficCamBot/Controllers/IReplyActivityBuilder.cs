using Microsoft.Bot.Connector;

namespace TrafficCamBot.Controllers
{
    /// <summary>
    /// Generates replies for a given activity.
    /// </summary>
    interface IReplyActivityBuilder
    {
        Activity BuildReplyActivity(Activity messageActivity, IUserData userData);
    }
}
