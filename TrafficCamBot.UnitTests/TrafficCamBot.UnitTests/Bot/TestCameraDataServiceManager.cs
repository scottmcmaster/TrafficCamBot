using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrafficCamBot.Bot;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class TestCameraDataServiceManager : ICameraDataServiceManager
    {
        public ICameraDataService GetCameraDataService(string serviceName)
        {
            return new TestCameraDataService(serviceName);
        }

        public IList<string> GetCameraDataServiceNames()
        {
            return new List<string> { "City 1", "City 2" };
        }
    }
}
