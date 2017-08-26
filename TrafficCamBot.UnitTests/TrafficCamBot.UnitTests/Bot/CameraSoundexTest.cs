using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficCamBot.Bot;
using System.Collections.Generic;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class CameraSoundexTest
    {
        private CameraSoundex index;

        [TestInitialize]
        public void Setup()
        {
            var cameraNames = new List<string>
            {
                "NE 85th st",
                "bickford ave",
                "116th ave ne"
            };
            index = new CameraSoundex(cameraNames, new AlternateNameGenerator());
        }

        [TestMethod]
        public void TestExactMatch()
        {
            var result = index.Search("bickford avenue");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestNoMatch()
        {
            var result = index.Search("bi ave");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestFuzzyMatch()
        {
            var result = index.Search("bckfrd ave");
            Assert.AreEqual(1, result.Count);
        }

    }
}
