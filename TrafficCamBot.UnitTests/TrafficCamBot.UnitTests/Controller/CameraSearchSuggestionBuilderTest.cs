using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using TrafficCamBot.Bot;
using TrafficCamBot.Controllers;

namespace TrafficCamBot.UnitTests.Controller
{
    /// <summary>
    /// Tests for the CameraSearchSuggestionBuilder class.
    /// </summary>
    [TestClass]
    public class CameraSearchSuggestionBuilderTest
    {
        readonly string[] TestCameras = { "Camera 1", "Camera2" };

        Mock<ICameraDataService> cameraData;

        [TestInitialize]
        public void Setup()
        {
            cameraData = new Mock<ICameraDataService>();
            cameraData.Setup(
                c => c.ListCameras()).Returns(TestCameras);
        }

        [TestMethod]
        public void BuildSearchSuggestion()
        {
            var builder = new CameraSearchSuggestionBuilder(cameraData.Object);
            var suggestion = builder.BuildSearchSuggestion();
            Assert.IsTrue(StartsWithOneOf(suggestion, CameraSearcher.TryPrompts));
            Assert.IsTrue(EndsWithOneOf(suggestion, TestCameras));
        }

        bool EndsWithOneOf(string text, IEnumerable<string> choices)
        {
            foreach (var choice in choices)
            {
                if ((text ?? string.Empty).EndsWith(choice, StringComparison.CurrentCulture))
                {
                    return true;
                }
            }
            return false;
        }

        bool StartsWithOneOf(string text, IEnumerable<string> choices)
        {
            foreach (var choice in choices)
            {
                if ((text ?? string.Empty).StartsWith(choice, StringComparison.CurrentCulture))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
