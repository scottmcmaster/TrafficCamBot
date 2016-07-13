using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Generates alternate camera names that the user might type instead of the "official" camera name.
    /// </summary>
    public class AlternateNameGenerator
    {
        private readonly IList<string> STREET_ALTS = ImmutableList.Create("street", "st", "st." );
        private readonly IList<string> DRIVE_ALTS = ImmutableList.Create("drive", "dr", "dr.");
        private readonly IList<string> HIGHWAY_ALTS = ImmutableList.Create("highway", "hwy", "hwy.");
        private readonly IList<string> ROAD_ALTS = ImmutableList.Create("road", "rd", "rd.");
        private readonly IList<string> AVENUE_ALTS = ImmutableList.Create("avenue", "ave", "ave.");
        private readonly IList<string> BOULEVARD_ALTS = ImmutableList.Create("boulevard", "blvd", "blvd.");
        private readonly IList<string> NORTH_ALTS = ImmutableList.Create("north", "n", "n.");
        private readonly IList<string> SOUTH_ALTS = ImmutableList.Create("south", "s", "s.");
        private readonly IList<string> EAST_ALTS = ImmutableList.Create("east", "e", "e.");
        private readonly IList<string> WEST_ALTS = ImmutableList.Create("west", "w", "w.");
        private readonly IList<string> NORTHWEST_ALTS = ImmutableList.Create("northwest", "nw", "nw.");
        private readonly IList<string> SOUTHWEST_ALTS = ImmutableList.Create("southwest", "sw", "sw.");
        private readonly IList<string> SOUTHEAST_ALTS = ImmutableList.Create("southeast", "se", "se.");
        private readonly IList<string> NORTHEAST_ALTS = ImmutableList.Create("northeast", "ne", "ne.");
        private readonly IList<string> ROUTE_ALTS = ImmutableList.Create("route", "rte", "rte.");

        private readonly IDictionary<string, IList<string>> alternates;

        public AlternateNameGenerator()
        {
            var allAlternateSets = new List<IList<string>> { STREET_ALTS, DRIVE_ALTS, HIGHWAY_ALTS, ROAD_ALTS, AVENUE_ALTS,
                BOULEVARD_ALTS, NORTH_ALTS, SOUTH_ALTS, EAST_ALTS, WEST_ALTS, NORTHWEST_ALTS, SOUTHWEST_ALTS,
                NORTHEAST_ALTS, SOUTHEAST_ALTS };
            var builder = ImmutableDictionary.CreateBuilder<string, IList<string>>();
            foreach (var alternateSet in allAlternateSets)
            {
                foreach (var alternate in alternateSet)
                {
                    builder.Add(alternate, alternateSet);
                }
            }
            alternates = builder.ToImmutable();
        }

        /// <summary>
        /// Gets a list of alternative reasonable names for the given camera name.
        /// </summary>
        /// <param name="cameraName"></param>
        /// <returns></returns>
        public IEnumerable<string> GenerateAlternateCameraNames(string cameraName)
        {
            var result = new List<string>();
            if (cameraName == null || cameraName.Trim().Length == 0)
            {
                yield break;
            }

            var tokens = cameraName.ToLower().Split(' ');

            // Build a list of sets of all alternates for each token in the given camera name search term.
            var tokenAlternates = new List<IList<string>>();
            foreach (var token in tokens)
            {
                if (alternates.ContainsKey(token))
                {
                    tokenAlternates.Add(alternates[token]);
                }
                else
                {
                    tokenAlternates.Add(new List<string> { token });
                }
            }

            // Enumerate all of the possible combinations of alternate tokens.
            var combinations = AllCombinationsOf<string>(tokenAlternates);
            foreach (List<string> combination in combinations)
            {
                yield return string.Join(" ", combination);
            }
        }

        /// <summary>
        /// See http://stackoverflow.com/questions/545703/combination-of-listlistint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sets"></param>
        /// <returns></returns>
        private List<List<T>> AllCombinationsOf<T>(List<IList<T>> sets)
        {
            var combinations = new List<List<T>>();

            // Prime the data
            foreach (var value in sets[0])
            {
                combinations.Add(new List<T> { value });
            }

            foreach (var set in sets.Skip(1))
            {
                combinations = AddExtraSet(combinations, set);
            }

            return combinations;
        }

        private static List<List<T>> AddExtraSet<T>
             (List<List<T>> combinations, IList<T> set)
        {
            var newCombinations = from value in set
                                  from combination in combinations
                                  select new List<T>(combination) { value };

            return newCombinations.ToList();
        }

    }
}