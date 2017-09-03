using System;
using System.Collections.Generic;
using TrafficCamBot.Bot;

namespace TrafficCamBot.UnitTests.Bot
{
    class TestCameraDataService : ICameraDataService
    {
        public TestCameraDataService(string name)
        {
            Name = name;
        }

        public HashSet<string> AlternateNames
        {
            get
            {
                return new HashSet<string> { "foo", "bar" };
            }
        }

        public string Name
        {
            get; private set;
        }

        public IList<string> ListCameras()
        {
            return new List<string>();
        }

        public ICameraLookupData Lookup(string desc)
        {
            return null;
        }

        public void RefreshCameras()
        {
        }
    }
}
