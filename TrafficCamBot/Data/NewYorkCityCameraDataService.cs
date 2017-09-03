using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Data
{
    public class NewYorkCityCameraDataService : CameraDataServiceBase
    {
        readonly ILog logger = LogManager.GetLogger(typeof(NewYorkCityCameraDataService));

        const string CameraListUrl = "http://dotsignals.org/multiview2.php";

        const string CameraTableRowId = "repCam__ctl0_trCam";

        const string CameraDetailsUrl = "http://dotsignals.org/multiview2.php?listcam={0}";

        const string CameraOnlyImgId = "repCamView__ct0_imgLink";

        readonly HashSet<string> alternateNames =
            new HashSet<string> { "nyc", "new york", "queens", "manhattan",
                "the bronx", "long island", "ny", "brooklyn", "staten island" };

        /// <summary>
        /// Map of camera title to href of the page with the camera on it. Gets fully populated at initialization-time.
        /// </summary>
        Dictionary<string, string> cameraPages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Map of camera title to the image url. Gets populated eagerly.
        /// </summary>
        readonly ConcurrentDictionary<string, string> cameras = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public NewYorkCityCameraDataService()
        {
            RefreshCameras();
        }

        public override void RefreshCameras()
        {
            logger.Info("Refreshing NYC cameras");

            var newCameraPages = cameraPages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var listPageDoc = new HtmlWeb().Load(CameraListUrl);
            var tableRows = listPageDoc.DocumentNode
                .Descendants("tr")
                .Where(d => d.Id == CameraTableRowId);
            foreach (var tableRow in tableRows)
            {
                var name = tableRow
                    .Descendants("span")
                    .First(d => d.Attributes["class"].Value == "OTopTitle");
                var checkbox = tableRow
                    .Descendants("input")
                    .FirstOrDefault(d => d.Attributes["type"].Value == "checkbox");
                if (checkbox == null)
                {
                    continue;
                }

                var title = name.InnerText;
                var cameraNumber = checkbox.Attributes["value"].Value;
                var href = string.Format(CameraDetailsUrl, cameraNumber);
                cameraPages[title] = href;
            }

            cameraPages = newCameraPages;
            SetCameraNames(newCameraPages.Keys.ToList());
            cameras.Clear();
        }

        public override HashSet<string> AlternateNames
        {
            get
            {
                return alternateNames;
            }
        }

        public override string Name
        {
            get
            {
                return "New York City";
            }
        }

        /// <summary>
        /// Scrapes the page at the given URL to get just the URL to the camera image.
        /// </summary>
        /// <param name="pageUrl">Page URL to scrape</param>
        /// <returns>The direct URL to the camera image.</returns>
        string GetCameraImageUrlFrom(String pageUrl)
        {
            var cameraPageDoc = new HtmlWeb().Load(pageUrl);
            var cameraElem = cameraPageDoc.GetElementbyId(CameraOnlyImgId);
            return cameraElem.GetAttributeValue("src", null);
        }

        protected override CameraImage GetImageUrlForCamera(string cameraName)
        {
            // Load the camera image url if we don't already have it cached.
            if (!cameras.ContainsKey(cameraName))
            {
                var href = cameraPages[cameraName];
                cameras[cameraName] = GetCameraImageUrlFrom(href);
            }

            if (!cameras.ContainsKey(cameraName))
            {
                throw new CameraNotFoundException("Expected camera not found: " + cameraName);
            }

            // Append the current time as a cache-buster.
            var cameraUrl = cameras[cameraName] + "?rand=" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return new CameraImage(cameraName, cameraUrl);
        }
    }
}