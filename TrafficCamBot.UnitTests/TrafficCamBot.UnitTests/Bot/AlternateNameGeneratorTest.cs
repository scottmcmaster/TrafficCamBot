using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TrafficCamBot.Bot;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class AlternateNameGeneratorTest
    {
        private AlternateNameGenerator gen;

        [TestInitialize]
        public void Setup()
        {
            gen = new AlternateNameGenerator();
        }

        [TestMethod]
        public void TestInvalidNames()
        {
            Assert.IsFalse(gen.GenerateAlternateCameraNames(" ").Any());
            Assert.IsFalse(gen.GenerateAlternateCameraNames(null).Any());
        }

        [TestMethod]
        public void TestStreet()
        {
            var result = gen.GenerateAlternateCameraNames("85th St");
            Assert.IsTrue(result.Contains("85th street"));
            Assert.IsTrue(result.Contains("85th st."));
            Assert.IsTrue(result.Contains("85th st"));
        }
    }
}
