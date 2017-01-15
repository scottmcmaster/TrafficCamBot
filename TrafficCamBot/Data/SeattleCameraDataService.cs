using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Data
{
    /// <summary>
    /// Serves up all of the Seattle-area traffic cameras.
    /// </summary>
    public class SeattleCameraDataService : CameraDataServiceBase
    {
        /// <summary>
        /// Map of camera title to href of the page with the camera on it. Gets fully populated at initialization-time.
        /// </summary>
        private readonly Dictionary<String, String> cameraPages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Map of camera title to the image url. Gets populated lazily.
        /// </summary>
        private readonly ConcurrentDictionary<String, String> cameras = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public override string Name
        {
            get
            {
                return "Seattle";
            }
        }

        public SeattleCameraDataService()
        {
            String mapPageUrl = "http://www.wsdot.com/traffic/seattle/default.aspx";
            var mapPageDoc = new HtmlWeb().Load(mapPageUrl);
            var mapElem = mapPageDoc.GetElementbyId("SeattleNav");
            foreach (var areaNode in mapElem.Elements("area"))
            {
                var title = areaNode.GetAttributeValue("title", null);
                var href = areaNode.GetAttributeValue("href", null);
                if (title != null && href != null && href.EndsWith("#cam", StringComparison.Ordinal))
                {
                    cameraPages[title] = href;
                }
            }
            SetCameraNames(cameraPages.Keys.ToList());
        }

        /// <summary>
        /// Scrapes the page at the given URL to get just the URL to the camera image.
        /// </summary>
        /// <param name="pageUrl">Page URL to scrape</param>
        /// <returns>The direct URL to the camera image.</returns>
        private string GetCameraImageUrlFrom(String pageUrl)
        {
            var cameraPageDoc = new HtmlWeb().Load(pageUrl);
            var cameraElem = cameraPageDoc.GetElementbyId("ILPTrafficCamera");
            return cameraElem.GetAttributeValue("src", null);
        }

        protected override CameraImage GetImageUrlForCamera(string cameraName)
        {
            Debug.Assert(cameraNames.Contains(cameraName));

            // Load the camera image url if we don't already have it cached.
            if (!cameras.ContainsKey(cameraName))
            {
                String href = cameraPages[cameraName];
                String url = "http://www.wsdot.com/traffic/seattle/" + href;
                cameras[cameraName] = GetCameraImageUrlFrom(url);
            }
            Debug.Assert(cameras.ContainsKey(cameraName), "Should have been populated by now");

            // Append the current time as a cache-buster.
            var cameraUrl = cameras[cameraName] + "?" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return new CameraImage(cameraName, cameraUrl);
        }
    }
}
