using System.Collections.Generic;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Base class for CameraDataServiceManager implementations.
    /// </summary>
    public abstract class CameraDataServiceManagerBase : ICameraDataServiceManager
    {
        public abstract IEnumerable<ICameraDataService> Services { get; }

        public ICameraDataService FindCameraDataService(string serviceQuery)
        {
            var normalizedText = serviceQuery.ToLower();
            foreach (var service in Services)
            {
                if (normalizedText.Contains(service.Name.ToLower()))
                {
                    return service;
                }
                foreach (string altName in service.AlternateNames)
                {
                    if (normalizedText.Contains(altName))
                    {
                        return service;
                    }
                }
            }
            return null;
        }

        public abstract ICameraDataService GetCameraDataService(string serviceName);
        public abstract IList<string> GetCameraDataServiceNames();
    }
}