using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficCamBot.Data;

namespace TrafficCamBot.UnitTests
{
    [TestClass]
    public class SeattleCameraDataTest
    {
        [TestMethod]
        public void TestList()
        {
            var data = new SeattleCameraDataService();
            var result = data.ListCameras();
            Assert.IsTrue(result.Count > 0);
        }
    }
}
