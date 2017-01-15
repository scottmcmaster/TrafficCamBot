using System.Collections.Generic;

namespace TrafficCamBot.Bot
{
    public interface ICameraDataServiceManager
    {
        ICameraDataService GetCameraDataService(string serviceName);
        IList<string> GetCameraDataServiceNames();

    }
}