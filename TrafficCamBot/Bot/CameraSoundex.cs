using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficCamBot.Bot
{
    public class CameraSoundex
    {
        private readonly IDictionary<string, HashSet<string>> soundexToCameraNames;

        public CameraSoundex(IList<string> cameraNames, AlternateNameGenerator altNameGenerator)
        {
            soundexToCameraNames = new Dictionary<string, HashSet<string>>();
            foreach (var cameraName in cameraNames)
            {
                AddToIndex(cameraName, altNameGenerator);
            }
        }

        public IList<string> Search(string normalizedQuery)
        {
            ISet<string> results = new HashSet<string>();

            var parts = normalizedQuery.Split();
            foreach (var part in parts)
            {
                var soundex = ComputeSoundex(part);
                if (soundex != null)
                {
                    if (soundexToCameraNames.ContainsKey(soundex))
                    {
                        results.UnionWith(soundexToCameraNames[soundex]);
                    }
                }
            }

            return results.ToList();
        }

            private void AddToIndex(string cameraName, AlternateNameGenerator altNameGenerator)
        {
            var parts = cameraName.Split();
            foreach (var part in parts)
            {
                if (altNameGenerator.Alternates.ContainsKey(part))
                {
                    continue;
                }

                var soundex = ComputeSoundex(part);
                if (soundex != null)
                {
                    if (!soundexToCameraNames.ContainsKey(soundex))
                    {
                        soundexToCameraNames[soundex] = new HashSet<string>();
                    }
                    soundexToCameraNames[soundex].Add(cameraName);
                }
            }
        }

        private string ComputeSoundex(string cameraName)
        {
            var sb = new StringBuilder();
            if (cameraName == null || cameraName.Length == 0)
            {
                return null;
            }

            var current = string.Empty;
            var previous = string.Empty;

            foreach (var c in cameraName)
            {
                if (sb.Length == 0)
                {
                    sb.Append(c);
                }
                else
                {
                    var ch = Char.ToLower(c);
                    current = string.Empty;
                    if ("bfpv".IndexOf(ch) > -1)
                    {
                        current = "1";
                    }
                    else if ("cgjkqsxz".IndexOf(ch) > -1)
                    {
                        current = "2";
                    }
                    else if ("dt".IndexOf(ch) > -1)
                    {
                        current = "3";
                    }
                    else if (ch == 'l')
                    {
                        current = "4";
                    }
                    else if (ch == 'm' || ch == 'n')
                    {
                        current = "5";
                    }
                    else if (ch == 'r')
                    {
                        current = "6";
                    }

                    if (current != previous)
                    {
                        sb.Append(current);
                    }
                    if (sb.Length == 4)
                    {
                        break;
                    }
                    if (current.Length > 0)
                    {
                        previous = current;
                    }
                }
            }

            var result = sb.ToString();
            if (result.Length < 4)
            {
                return null;
            }
            return result.ToUpper();
        }
    }
}