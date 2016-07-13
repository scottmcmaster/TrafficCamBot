using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Data
{
    /// <summary>
    /// Serves up all of the Bay Area traffic cameras.
    /// </summary>
    public class BayAreaCameraDataService : ICameraDataService
    {
        /// <summary>
        /// Map of camera title to the image url.
        /// </summary>
        private readonly ConcurrentDictionary<String, String> cameras = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string Name
        {
            get
            {
                return "Bay Area";
            }
        }

        public BayAreaCameraDataService()
        {
            LoadCameras();
        }

        /// <summary>
        /// The Caltrans page at http://www.dot.ca.gov/d4/realtime.htm is a little
        /// annoying to parse, so for now we'll just hard-code all of the names and
        /// URLs in here.
        /// </summary>
        private void LoadCameras()
        {
            cameras["I80 SAS Tower"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVD33_E80atSASTower.jpg";
            cameras["S80@Carlson"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV503_W80atCarlson.jpg";
            cameras["S880@Paseo Grande"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV711_S880atPaseoGrande.jpg";
            cameras["W580 JWO 24"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV102_W580atJCT24.jpg";
            cameras["WB I-80@SFOBB Incline"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVD10_W80atIncline.jpg";
            cameras["E92 San Mateo"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVE03_E92atSub2.jpg";
            cameras["SB I-680 @ I-80 Fairfield"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV812_S680at80.jpg";
            cameras["W80@Fremont"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV519_W580atRegatta.jpg";
            cameras["SB880@66th"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVB11_S880atJCT84.jpg";
            cameras["NB101@580 Interchange"] = "http://www.dot.ca.gov/research/its/cctv/images/d04/TVE83_N101AtJCT580.jpg";
            cameras["EB80@101 SF"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV304_E80ATJCT101.jpg";
            cameras["SF 101 E Portal"] = "http://www.dot.ca.gov/cwwp2/data/d10/cctv/image/jctsr160rwis/jctsr160rwis.jpg";
            cameras["S101 AT IGNACIO BL"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVE86_N101atIGNACIO.jpg";
            cameras["W24@Telegraph"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV113_W24atTelegraph.jpg";
            cameras["W80 JEO Ashby"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV121_W80atAshby.jpg";
            cameras["N1@Presidio Tunnel"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV388_N1PRESIDIO.jpg";
            cameras["N880@101"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVC60_S880at101.jpg";
            cameras["80@6thStreet"] = "http://www.dot.ca.gov/research/its/cctv/images/d04/TV316_E80At6th.jpg";
            cameras["S680@PineValley"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TVF05_S680AtPineValley.jpg";
            cameras["S680@NorthMain"] = "http://www.dot.ca.gov/cwwp2/data/d4/cctv/image/TV216_S680atNMain.jpg";
        }

        public ICameraLookupData Lookup(string desc)
        {
            if (cameras.ContainsKey(desc))
            {
                return new CameraImage(cameras[desc]);
            }
            return new CameraLookupError("Not found");
        }

        public IList<string> ListCameras()
        {
            return cameras.Keys.ToList();
        }
    }
}