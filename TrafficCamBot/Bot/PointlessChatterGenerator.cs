using log4net;
using System.Linq;
using TrafficCamBot.Utils;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Responds to off-topic messages in the most reasonable and minimal way possible.
    /// </summary>
    public class PointlessChatterGenerator
    {
        // Requests
        public const string Hi = "hi";
        public const string Hey = "hey";
        public const string Hello = "hello";
        public const string HowAreYou = "how are you";
        public const string WhatsUp = "whats up";
        public const string WhatsYourName = "whats your name";
        public const string WhatIsYourName = "what is your name";
        public const string Bye = "bye";
        public const string Goodbye = "goodbye";
        public const string SeeYouLater = "see you";
        public const string Thanks = "thanks";
        public const string ThankYou = "thank you";

        // Replies
        public const string NotMuch = "Not much.";
        public const string Fine = "I am fine.";
        public const string GenericGreeting = "Hi!";
        public const string NameReply = "I am the Traffic Cam Bot.";
        public const string ByeReply = "Bye.";
        public const string ThanksReply = "You're welcome.";

        readonly ILog logger = LogManager.GetLogger(typeof(PointlessChatterGenerator));

        public static readonly string[] PointlessPhrases = {
           Hi,
           Hey,
           Hello,
           HowAreYou,
           WhatsUp,
           WhatsYourName,
           WhatIsYourName,
           Bye,
           Goodbye,
           SeeYouLater,
           Thanks,
           ThankYou
        };

        /// <summary>
        /// If the user has indicated that the conversation is over for now,
        /// we don't want to suggest things.
        /// </summary>
        public static bool ShouldMakeSuggestion(string pointlessResponse)
        {
            return pointlessResponse != ByeReply && pointlessResponse != ThanksReply;
        }

        /// <summary>
        /// Helper method to identify pointless chatter in a query phrase.
        /// </summary>
        /// <param name="queryPhrase"></param>
        /// <returns></returns>
        public static bool IsPointlessChatter(string queryPhrase)
        {
            return SearchForPointlessPhrase(queryPhrase) != null;
        }

        public string GeneratePointlessResponse(string request)
        {
            var pointlessRequest = SearchForPointlessPhrase(request);

            // If it happens to be null for some reason, do a default.
            // But ideally we've verified earlier that this is the right
            // response type to be using and therefore there ought to be one.
            if (pointlessRequest == null)
            {
                logger.Warn("Incorrectly using pointless response, input is: " + request);
                pointlessRequest = PointlessPhrases[0];
            }

            switch (pointlessRequest)
            {
                case WhatsUp:
                    return NotMuch;

                case HowAreYou:
                    return Fine;

                case Bye:
                case Goodbye:
                case SeeYouLater:
                    return ByeReply;

                case WhatsYourName:
                case WhatIsYourName:
                    return NameReply;

                case Thanks:
                case ThankYou:
                    return ThanksReply;

                default:
                    return GenericGreeting;
            }
        }

        /// <summary>
        /// Looks for a pointless phrase in the query. The phrase may be either a prefix
        /// or a suffix of the well-known pointless phrases.
        /// </summary>
        static string SearchForPointlessPhrase(string queryPhrase)
        {
            var normalizedQuery = new string(queryPhrase
                .ToLower()
                .Where(c => !char.IsPunctuation(c))
                .ToArray())
                .Split(' ');

            foreach (var pointlessPhrase in PointlessPhrases)
            {
                if (ArrayUtils.SamePrefix<string>(normalizedQuery, pointlessPhrase.Split(' ')))
                {
                    return pointlessPhrase;
                }
            }
            return null;
        }

    }
}