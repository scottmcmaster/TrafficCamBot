using System;
using System.Runtime.Serialization;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Thrown when we can't find a camera that we otherwise expect to find.
    /// </summary>
    public class CameraNotFoundException : Exception
    {
        public CameraNotFoundException() : base()
        {
        }

        public CameraNotFoundException(string message) : base(message)
        {
        }

        public CameraNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // This constructor is needed for serialization.
        protected CameraNotFoundException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}