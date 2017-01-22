using Microsoft.Bot.Connector;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Figures out from the raw message text what the user is actually trying to do.
    /// </summary>
    public class MessageInterpreter
    {
        public enum MessageType
        {
            HelpRequest,
            ListCameras,
            Query,
            CameraChoice,
            SelectCity,
            PointlessChatter
        }

        public MessageType InterpretMessage(Activity activity)
        {
            var normalizedText = new string(activity.Text.Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();

            if (PointlessChatterGenerator.IsPointlessChatter(normalizedText))
            {
                return MessageType.PointlessChatter;
            }

            if (QueryContainsWord(normalizedText, "list"))
            {
                return MessageType.ListCameras;
            }

            if (QueryContainsWord(normalizedText, "help"))
            {
                return MessageType.HelpRequest;
            }

            if (normalizedText.StartsWith("view ", StringComparison.Ordinal))
            {
                return MessageType.SelectCity;
            }

            return MessageType.Query;
        }

        bool QueryContainsWord(string queryPhrase, string word)
        {
            return queryPhrase.Split(' ').Contains(word);
        }

    }
}