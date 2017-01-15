using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    /// <summary>
    /// Wraps the conversational data.
    /// </summary>
    public interface IUserData
    {
        CameraChoiceList GetChoiceList();

        void SetCity(string selectedServiceName);

        string GetCity();

        void SetChoiceList(CameraChoiceList cameraChoiceList);
    }
}