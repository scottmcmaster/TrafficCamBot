using Microsoft.Bot.Connector;
using System;
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
            HELP_REQUEST,
            LIST_CAMERAS,
            QUERY,
            CAMERA_CHOICE,
            SELECT_CITY
        }

        public MessageType InterpretMessage(Activity activity)
        {
            var normalizedText = new string(activity.Text.Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();

            if (QueryContainsWord(normalizedText, "list"))
            {
                return MessageType.LIST_CAMERAS;
            } else if (QueryContainsWord(normalizedText, "help"))
            {
                return MessageType.HELP_REQUEST;
            } else if (normalizedText.StartsWith("view ", StringComparison.Ordinal))
            {
                return MessageType.SELECT_CITY;
            }

            return MessageType.QUERY;
        }

        private bool QueryContainsWord(string queryPhrase, string word)
        {
            return queryPhrase.Split(' ').Contains(word);
        }

    }
}