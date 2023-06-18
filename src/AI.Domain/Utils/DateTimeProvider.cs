namespace AI.Domain.Utils
{
    /// <summary>
    /// DateTimeProvider implementation
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        private Func<DateTimeOffset> current;

        /// <summary>
        /// DateTimeProvider impl
        /// </summary>
        public DateTimeProvider()
        {
            current = GetDynamicCurrent;
        }

        /// <summary>
        /// The actual date and time
        /// </summary>
        public DateTimeOffset Now => current();

        public DateTimeOffset Minimum => new DateTimeOffset(2010, 01, 01, 00, 00, 00, TimeSpan.Zero);
        public DateTime MinimumDate => new DateTime(2010, 01, 01, 00, 00, 00);


        private DateTimeOffset GetDynamicCurrent()
        {
            return DateTimeOffset.Now;
        }
        private DateTimeOffset GetFixed()
        {
            return fixedDt;
        }

        private DateTimeOffset fixedDt;

        /// <summary>
        /// SetTime
        /// </summary>
        /// <param name="dateTime"></param>
        void IDateTimeProvider.SetTime(DateTimeOffset dateTime)
        {
            fixedDt = dateTime;
            current = GetFixed;
        }
    }
}