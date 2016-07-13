using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Wraps potentially-multiple methods for searching camera data and provides a 
    /// (currently fixed) strategy for executing them.
    /// </summary>
    public class CameraSearcher : IDisposable
    {
        private readonly IList<string> cameraNames;

        private readonly CameraSearchIndex searchIndex;

        /// <summary>
        /// Creates a searcher for the given set of camera names.
        /// </summary>
        /// <param name="cameraNames">The camera names</param>
        public CameraSearcher(List<string> cameraNames)
        {
            this.cameraNames = ImmutableList.Create(cameraNames.ToArray());
            searchIndex = new CameraSearchIndex(cameraNames);
        }

        public void Dispose()
        {
            searchIndex.Dispose();
        }

        /// <summary>
        /// Searches for the given string using all supported methods. Currently, the approach is:
        /// 1. Look for camera names where the text matches the prefix.
        /// 2. Use the indexed search.
        /// Searches are case-insensitive.
        /// </summary>
        /// <param name="desc">The text to search for</param>
        /// <returns>A list which may be empty.</returns>
        public IList<string> Search(string desc)
        {
            var result = from cameraName in cameraNames
                         where cameraName.ToLower().StartsWith(desc.ToLower(), System.StringComparison.Ordinal)
                         select cameraName;
            if (result.Any())
            {
                return result.ToList();
            }
            return searchIndex.Search(desc.ToLower());
        }
    }
}