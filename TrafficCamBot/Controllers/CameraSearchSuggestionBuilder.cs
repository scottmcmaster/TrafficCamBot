using System;
using System.Text;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    /// <summary>
    /// Gives some ideas for searches to try for a given camera data service.
    /// </summary>
    public class CameraSearchSuggestionBuilder
    {
        readonly ICameraDataService cameraData;
        readonly Random rnd = new Random();

        public CameraSearchSuggestionBuilder(ICameraDataService cameraData)
        {
            this.cameraData = cameraData;
        }

        /// <summary>
        /// Creates a bullet list with two suggestions.
        /// </summary>
        public string BuildSearchSuggestionList()
        {
            var sb = new StringBuilder();
            var allCameras = cameraData.ListCameras();
            var camera1 = allCameras[rnd.Next(allCameras.Count)];
            var camera2 = allCameras[rnd.Next(allCameras.Count)];
            var prompt1 = GetRandomTryPrompt();
            var prompt2 = GetRandomTryPrompt();

            sb.Append("Why not try\n");
            sb.Append("* ");
            sb.Append(prompt1);
            sb.Append(camera1);
            sb.Append("\n");
            sb.Append("* ");
            sb.Append(prompt2);
            sb.Append(camera2);

            return sb.ToString();
        }

        /// <summary>
        /// Creates a single search suggestion with no extra prompt.
        /// </summary>
        /// <returns></returns>
        public string BuildSearchSuggestion()
        {
            var allCameras = cameraData.ListCameras();
            var camera1 = allCameras[rnd.Next(allCameras.Count)];
            var prompt1 = GetRandomTryPrompt();
            return prompt1 + camera1;
        }

        string GetRandomTryPrompt()
        {
            return CameraSearcher.TryPrompts[rnd.Next(CameraSearcher.TryPrompts.Count)];
        }
    }
}