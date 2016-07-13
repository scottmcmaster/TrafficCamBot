namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Encapsulates an error that occurred while searching for the user-specified camera.
    /// </summary>
    public struct CameraLookupError : ICameraLookupData
    {
        public string Message
        {
            get;
        }

        public CameraLookupError(string message)
        {
            Message = message;
        }
    }
}