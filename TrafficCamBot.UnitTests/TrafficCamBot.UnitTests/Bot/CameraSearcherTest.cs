using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrafficCamBot.Bot;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class CameraSearcherTest
    {
        private CameraSearcher searcher;

        [TestInitialize]
        public void Setup()
        {
            var data = new List<string> { "ne 85th st", "124th ave ne" };
            searcher = new CameraSearcher(data);
        }

        [TestMethod]
        public void TestPrefix()
        {
            var result = searcher.Search("ne");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestPrefixMultiple()
        {
            var data = new List<string> { "ne 85th st", "124th ave ne", "ne 8th st" };
            searcher = new CameraSearcher(data);
            var result = searcher.Search("ne");
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestPrefixCaseSensitivity()
        {
            var result = searcher.Search("NE");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestNothingFound()
        {
            var result = searcher.Search("booger");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestIndexSearch()
        {
            var result = searcher.Search("85th");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestWithPrompt()
        {
            var result = searcher.Search(CameraSearcher.TryPrompts[0] + " ne 85th st");
            Assert.AreEqual(1, result.Count);
        }
    }
}
