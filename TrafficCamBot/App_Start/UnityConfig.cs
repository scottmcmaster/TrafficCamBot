using Microsoft.Practices.Unity;
using System.Reflection;
using System.Web.Http;
using System.Linq;
using System.Web.Mvc;
using TrafficCamBot.Bot;
using TrafficCamBot.Data;

namespace TrafficCamBot
{
    public static class UnityConfig
    {
        private static UnityContainer container;

        public static void RegisterComponents()
        {
			container = new UnityContainer();
            
            var cameraDataServiceTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                         where t.IsClass && !t.IsAbstract && t.GetInterface(typeof(ICameraDataService).Name) != null
                                         select t;
            foreach (var cameraDataServiceType in cameraDataServiceTypes)
            {
                container.RegisterType(typeof(ICameraDataService), cameraDataServiceType,
                    cameraDataServiceType.Name, new ContainerControlledLifetimeManager(),
                    new InjectionMember[] { });
            }

            container.RegisterType<CameraDataServiceManager, CameraDataServiceManager>(new ContainerControlledLifetimeManager());

            DependencyResolver.SetResolver(new Microsoft.Practices.Unity.Mvc.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        internal static UnityContainer GetConfiguredContainer()
        {
            return container;
        }
    }
}