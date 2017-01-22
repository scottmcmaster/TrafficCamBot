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
        /// <summary>
        /// Set of known prompts. If the query starts with any of these,
        /// we strip it first.
        /// If any of these are substrings, put the most specific one first.
        /// </summary>
        public static readonly IList<string> TryPrompts = ImmutableList.Create(
            "How about ",
            "Show the camera at ",
            "Show me ",
            "Show ",
            "What about ",
            "Let's see ",
            "See the camera at ",
            "See camera ",
            "See ");

        readonly IList<string> cameraNames;

        readonly CameraSearchIndex searchIndex;

        /// <summary>
        /// Creates a searcher for the given set of camera names.
        /// </summary>
        /// <param name="cameraNames">The camera names</param>
        public CameraSearcher(IList<string> cameraNames)
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
            var normalizedQuery = desc.ToLower();

            // Part 1: Preprocess: Look for the well-known and suggested prompts.
            foreach (var tryPrompt in TryPrompts)
            {
                var normalizedTryPrompt = tryPrompt.ToLower();
                if (normalizedQuery.StartsWith(normalizedTryPrompt, StringComparison.CurrentCulture))
                {
                    normalizedQuery = normalizedQuery.Replace(normalizedTryPrompt, "");
                    break;
                }
            }

            // Part 2: Look for the name directly.
            var result = from cameraName in cameraNames
                         where cameraName.ToLower().StartsWith(normalizedQuery, System.StringComparison.Ordinal)
                         select cameraName;
            if (result.Any())
            {
                return result.ToList();
            }

            // Part 3: Use the search index.
            return searchIndex.Search(normalizedQuery);
        }
    }
}