using System.Collections.Generic;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Looks up camera information.
    /// </summary>
    public interface ICameraDataService
    {
        ICameraLookupData Lookup(string desc);

        IList<string> ListCameras();

        string Name { get; }
    }
}