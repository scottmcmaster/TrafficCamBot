using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficCamBot.Bot;
using System.Collections.Generic;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class CameraSearchIndexTest
    {
        private CameraSearchIndex index;

        [TestInitialize]
        public void Setup()
        {
            var cameraNames = new List<string>
            {
                "NE 85th st",
                "bickford ave",
                "116th ave ne"
            };
            index = new CameraSearchIndex(cameraNames, new AlternateNameGenerator());
        }

        [TestMethod]
        public void TestExactMatchesWithAltNames()
        {
            var result = index.Search("northeast 85th Street");
            Assert.AreEqual(1, result.Count);

            result = index.Search("ne 85th st");
            Assert.AreEqual(1, result.Count);

            result = index.Search("bickford avenue");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestFuzzySearch()
        {
            var result = index.Search("bikcford");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestSoundexMatch()
        {
            var result = index.Search("bckfrd");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestNoResults()
        {
            var result = index.Search("asdfasdfasdf");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestPhraseSearch()
        {
            var result = index.Search("show me traffic for NE 85th st");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestExactSearchForMultiple()
        {
            var result = index.Search("northeast");
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestPhraseSearchForMultiple()
        {
            // Adjust the hit score because this is a particularly
            // contrived and weak match.
            index.HitScore = 0.01;
            var result = index.Search("show me northeast traffic");
            Assert.AreEqual(2, result.Count);
        }
    }
}
