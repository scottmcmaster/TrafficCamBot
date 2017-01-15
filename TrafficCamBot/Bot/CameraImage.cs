using System;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Represents an individual camera image to be displayed to the user.
    /// </summary>
    [Serializable]
    public struct CameraImage : ICameraLookupData
    {
        public string Url { get; private set; }
        public string Name { get; private set; }

        public CameraImage(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}