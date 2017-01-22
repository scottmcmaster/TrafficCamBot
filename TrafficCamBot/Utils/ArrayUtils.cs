using System;

namespace TrafficCamBot.Utils
{
    /// <summary>
    /// Extension metbods for .NET arrays.
    /// </summary>
    public static class ArrayUtils
    {
        /// <summary>
        /// Determines if two arrays have the same prefix.
        /// </summary>
        public static bool SamePrefix<T>(T[] first, T[] second)
            where T : IComparable<T>
        {
            if (first == null || second == null)
            {
                return true;
            }

            var i = 0;
            while (i < first.Length && i < second.Length)
            {
                if (first[i].CompareTo(second[i]) != 0)
                {
                    return false;
                }
                ++i;
            }
            return true;
        }
    }
}