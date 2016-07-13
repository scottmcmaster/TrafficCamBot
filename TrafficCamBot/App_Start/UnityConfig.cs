using Microsoft.Practices.Unity;
using System.Web.Http;
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
            container.RegisterType<ICameraDataService, SeattleCameraDataService>(typeof(SeattleCameraDataService).Name, new ContainerControlledLifetimeManager());
            container.RegisterType<ICameraDataService, BayAreaCameraDataService>(typeof(BayAreaCameraDataService).Name, new ContainerControlledLifetimeManager());

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