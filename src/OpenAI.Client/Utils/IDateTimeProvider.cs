namespace OpenAI.Client.Utils
{
    /// <summary>
    /// Service for providing the Date Time
    /// </summary>
    public interface IDateTimeProvider
    {

        /// <summary>
        /// The actual date and time
        /// </summary>
        DateTimeOffset Now { get; }

        DateTimeOffset Minimum { get; }
        DateTime MinimumDate { get; }

        /// <summary>
        /// SetTime
        /// </summary>
        /// <param name="dateTime"></param>
        void SetTime(DateTimeOffset dateTime);


    }
}
