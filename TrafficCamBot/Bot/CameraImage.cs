using System;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Represents an individual camera image to be displayed to the user.
    /// </summary>
    [Serializable]
    public struct CameraImage : ICameraLookupData
    {
        public string Url
        {
            get;
        }

        public CameraImage(string url)
        {
            Url = url;
        }
    }
}