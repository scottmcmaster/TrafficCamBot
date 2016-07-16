using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Helper class for implementing <see cref="ICameraDataService"/>.
    /// </summary>
    public abstract class CameraDataServiceBase : ICameraDataService, IDisposable
    {
        public abstract string Name { get; }

        public IList<string> ListCameras()
        {
            return cameraNames.ToList();
        }

        /// <summary>
        /// Returns the camera image URL for the give camera name. Assumes
        /// the camera name is valid and normalized.
        /// </summary>
        /// <param name="cameraName"></param>
        /// <returns></returns>
        protected abstract CameraImage GetImageUrlForCamera(string cameraName);

        /// <summary>
        /// Searcher to fall back on when direct lookup of a given camera returns no results.
        /// </summary>
        protected CameraSearcher searcher;

        /// <summary>
        /// Set of camera names, suitable for lookup.
        /// </summary>
        protected ImmutableSortedSet<string> cameraNames;

        /// <summary>
        /// Implements the common camera lookup logic.
        /// </summary>
        /// <param name="cameraNameQuery">The text to search for. Ideally it is a camera name.</param>
        /// <returns></returns>
        public ICameraLookupData Lookup(string cameraNameQuery)
        {
            string cameraName = null;
            if (cameraNames.Contains(cameraNameQuery))
            {
                // We got an exact camera name.
                cameraName = cameraNameQuery;
            }
            else
            {
                // Try the searcher.
                var results = searcher.Search(cameraNameQuery);
                if (results.Count == 1)
                {
                    cameraName = results.First();
                }
                else if (results.Count > 1)
                {
                    return HandleChoiceResult(results);
                }
            }

            if (cameraName == null)
            {
                return new CameraLookupError("Not found");
            }

            return GetImageUrlForCamera(cameraName);
        }


        /// <summary>
        /// Called to set the list of camera names for the searcher.
        /// </summary>
        /// <param name="cameraNames"></param>
        protected void SetCameraNames(IList<string> cameraNames)
        {
            ((IDisposable) searcher)?.Dispose();
            this.cameraNames = ImmutableSortedSet.CreateRange(cameraNames);
            searcher = new CameraSearcher(cameraNames);
        }

        public void Dispose()
        {
            ((IDisposable) searcher)?.Dispose();
        }

        private ICameraLookupData HandleChoiceResult(IList<string> choices)
        {
            return new CameraChoiceList(choices);
        }


    }
}