using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Data
{
    /// <summary>
    /// Serves up all of the Seattle-area traffic cameras.
    /// </summary>
    public class SeattleCameraDataService : CameraDataServiceBase
    {
        readonly ILog logger = LogManager.GetLogger(typeof(SeattleCameraDataService));

        /// <summary>
        /// Map of camera title to href of the page with the camera on it. Gets fully populated at initialization-time.
        /// </summary>
        Dictionary<string, string> cameraPages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        readonly HashSet<string> alternateNames =
            new HashSet<string> { "sea" };

        /// <summary>
        /// Map of camera title to the image url. Gets populated lazily.
        /// </summary>
        ConcurrentDictionary<string, string> cameras = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public override string Name
        {
            get
            {
                return "Seattle";
            }
        }

        public override HashSet<string> AlternateNames
        {
            get
            {
                return alternateNames;
            }
        }

        public SeattleCameraDataService()
        {
            RefreshCameras();
        }

        public override void RefreshCameras()
        {
            logger.Info("Refreshing Seattle cameras");

            var newCameraPages = cameraPages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var mapPageUrl = "http://www.wsdot.com/traffic/seattle/default.aspx";
            var mapPageDoc = new HtmlWeb().Load(mapPageUrl);
            var mapElem = mapPageDoc.GetElementbyId("SeattleNav");
            foreach (var areaNode in mapElem.Elements("area"))
            {
                var title = areaNode.GetAttributeValue("title", null);
                var href = areaNode.GetAttributeValue("href", null);
                if (title != null && href != null && href.EndsWith("#cam", StringComparison.Ordinal))
                {
                    newCameraPages[title] = href;
                }
            }

            cameraPages = newCameraPages;
            SetCameraNames(newCameraPages.Keys.ToList());
            cameras.Clear();
        }

        /// <summary>
        /// Scrapes the page at the given URL to get just the URL to the camera image.
        /// </summary>
        /// <param name="pageUrl">Page URL to scrape</param>
        /// <returns>The direct URL to the camera image.</returns>
        string GetCameraImageUrlFrom(string pageUrl)
        {
            var cameraPageDoc = new HtmlWeb().Load(pageUrl);
            var cameraElem = cameraPageDoc.GetElementbyId("ILPTrafficCamera");
            return cameraElem.GetAttributeValue("src", null);
        }

        protected override CameraImage GetImageUrlForCamera(string cameraName)
        {
            // Load the camera image url if we don't already have it cached.
            if (!cameras.ContainsKey(cameraName))
            {
                String href = cameraPages[cameraName];
                String url = "http://www.wsdot.com/traffic/seattle/" + href;
                cameras[cameraName] = GetCameraImageUrlFrom(url);
            }

            if (!cameras.ContainsKey(cameraName))
            {
                throw new CameraNotFoundException("Expected camera not found: " + cameraName);
            }

            // Append the current time as a cache-buster.
            var cameraUrl = cameras[cameraName] + "?" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return new CameraImage(cameraName, cameraUrl);
        }
    }
}
