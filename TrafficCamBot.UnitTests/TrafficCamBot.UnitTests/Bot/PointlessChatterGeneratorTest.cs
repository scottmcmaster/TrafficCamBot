using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficCamBot.Bot;

namespace TrafficCamBot.UnitTests.Bot
{
    [TestClass]
    public class PointlessChatterGeneratorTest
    {
        [TestMethod]
        public void TestIsPointlessChatterSimple()
        {
            foreach (var phrase in PointlessChatterGenerator.PointlessPhrases)
            {
                Assert.IsTrue(PointlessChatterGenerator.IsPointlessChatter(phrase));
            }

            Assert.IsFalse(PointlessChatterGenerator.IsPointlessChatter("doo bee doo bee doo"));
            Assert.IsFalse(PointlessChatterGenerator.IsPointlessChatter("highrise"));
        }

        [TestMethod]
        public void TestIsPointlessChatterTrailingPunctuation()
        {
            foreach (var phrase in PointlessChatterGenerator.PointlessPhrases)
            {
                Assert.IsTrue(PointlessChatterGenerator.IsPointlessChatter(phrase + "!"));
            }
        }

        [TestMethod]
        public void TestIsPointlessChatterUppercase()
        {
            foreach (var phrase in PointlessChatterGenerator.PointlessPhrases)
            {
                Assert.IsTrue(PointlessChatterGenerator.IsPointlessChatter(phrase.ToUpper()));
            }
        }

        [TestMethod]
        public void TestIsPointlessChatterStartingLongerSentence()
        {
            foreach (var phrase in PointlessChatterGenerator.PointlessPhrases)
            {
                Assert.IsTrue(PointlessChatterGenerator.IsPointlessChatter(phrase + ", how are you?"));
            }
        }

        [TestMethod]
        public void TestGeneratePointlessResponse()
        {
            var generator = new PointlessChatterGenerator();
            Assert.AreEqual(PointlessChatterGenerator.GenericGreeting, generator.GeneratePointlessResponse("hi"));
            Assert.AreEqual(PointlessChatterGenerator.GenericGreeting, generator.GeneratePointlessResponse("Hi"));
            Assert.AreEqual(PointlessChatterGenerator.GenericGreeting, generator.GeneratePointlessResponse("hi "));
            Assert.AreEqual(PointlessChatterGenerator.GenericGreeting, generator.GeneratePointlessResponse("hi!"));
            Assert.AreEqual(PointlessChatterGenerator.GenericGreeting, generator.GeneratePointlessResponse("hi there"));
            Assert.AreEqual(PointlessChatterGenerator.GenericGreeting, generator.GeneratePointlessResponse("unknown"));
            Assert.AreEqual(PointlessChatterGenerator.Fine, generator.GeneratePointlessResponse("How are you?"));
        }

        [TestMethod]
        public void TestShouldMakeSuggestion()
        {
            Assert.IsTrue(PointlessChatterGenerator.ShouldMakeSuggestion(PointlessChatterGenerator.GenericGreeting));
            Assert.IsFalse(PointlessChatterGenerator.ShouldMakeSuggestion(PointlessChatterGenerator.ByeReply));
        }
    }
}
