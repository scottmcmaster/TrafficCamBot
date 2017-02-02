using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrafficCamBot.Bot;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class TestCameraDataServiceManager : CameraDataServiceManagerBase
    {
        readonly ICameraDataService service1 = new TestCameraDataService("City 1");
        readonly ICameraDataService service2 = new TestCameraDataService("City 2");

        public override IEnumerable<ICameraDataService> Services
        {
            get
            {
                return new List<ICameraDataService> { service1, service2 };
            }
        }

        public override ICameraDataService GetCameraDataService(string serviceName)
        {
            return new TestCameraDataService(serviceName);
        }

        public override IList<string> GetCameraDataServiceNames()
        {
            return new List<string> { "City 1", "City 2" };
        }
    }
}
