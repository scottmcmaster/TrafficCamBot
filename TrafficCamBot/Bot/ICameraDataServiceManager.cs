using System.Collections.Generic;

namespace TrafficCamBot.Bot
{
    public interface ICameraDataServiceManager
    {
        ICameraDataService GetCameraDataService(string serviceName);
        IList<string> GetCameraDataServiceNames();
        ICameraDataService FindCameraDataService(string serviceQuery);
        IEnumerable<ICameraDataService> Services { get; }
    }
}