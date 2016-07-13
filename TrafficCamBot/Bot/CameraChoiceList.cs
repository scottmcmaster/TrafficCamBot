using System;
using System.Collections.Generic;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Represents a list of possible cameras for the user to choose from.
    /// </summary>
    [Serializable]
    public class CameraChoiceList : ICameraLookupData
    {
        public IList<string> CameraNames
        {
            get;
        }

        public CameraChoiceList(IList<string> cameraNames)
        {
            CameraNames = cameraNames;
        }
    }
}