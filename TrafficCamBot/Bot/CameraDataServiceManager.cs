using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Holds and dishes out all instances of <see cref="ICameraDataService"/>
    /// </summary>
    public class CameraDataServiceManager : CameraDataServiceManagerBase
    {
        private readonly ConcurrentDictionary<string, ICameraDataService> services =
            new ConcurrentDictionary<string, ICameraDataService>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// TODO: I would expect to be able to inject UnityContainer here. But when I try, it gives me
        /// not the instance I expect (e.g. the one with the types registered).
        /// So instead I'm using the global...
        /// </summary>
        public CameraDataServiceManager()
        {
            var managers = UnityConfig.GetConfiguredContainer().ResolveAll(typeof(ICameraDataService));
            foreach (ICameraDataService manager in managers)
            {
                var service = manager as ICameraDataService;
                services[service.Name] = service;
            }
        }

        public override IEnumerable<ICameraDataService> Services
        {
            get
            {
                return services.Values;
            }
        }

        public override ICameraDataService GetCameraDataService(string serviceName)
        {
            if (services.ContainsKey(serviceName))
            {
                return services[serviceName];
            }
            return null;
        }

        public override IList<string> GetCameraDataServiceNames()
        {
            return services.Keys.ToList();
        }
    }
}