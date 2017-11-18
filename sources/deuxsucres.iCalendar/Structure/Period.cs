using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Period
    /// </summary>
    public class Period
    {
        /// <summary>
        /// Create a new period between two dates
        /// </summary>
        public Period(DateTime start, DateTime end)
        {
            if (start < end)
            {
                DateStart = start;
                DateEnd = end;
            }
            else
            {
                DateStart = end;
                DateEnd = start;
            }
        }

        /// <summary>
        /// Create a new period
        /// </summary>
        public Period(DateTime start, TimeSpan duration)
        {
            DateStart = start;
            Duration = duration;
        }

        /// <summary>
        /// Calculate the date end
        /// </summary>
        public DateTime GetDateEnd()
        {
            return DateEnd ?? DateStart + Duration.Value;
        }

        /// <summary>
        /// Calculate the duration
        /// </summary>
        public TimeSpan GetDuration()
        {
            return Duration ?? DateEnd.Value - DateStart;
        }

        /// <summary>
        /// Date start
        /// </summary>
        public DateTime DateStart { get; private set; }

        /// <summary>
        /// Date end
        /// </summary>
        public DateTime? DateEnd { get; private set; }

        /// <summary>
        /// Duration
        /// </summary>
        public TimeSpan? Duration { get; private set; }
    }
}
