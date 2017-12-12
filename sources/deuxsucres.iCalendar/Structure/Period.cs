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
        /// Equality
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Period p)
            {
                return
                    object.Equals(p.DateStart, DateStart)
                    && object.Equals(p.DateEnd, DateEnd)
                    && object.Equals(p.Duration, Duration)
                    ;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Hash code
        /// </summary>
        public override int GetHashCode()
        {
            return DateStart.GetHashCode() ^ DateEnd?.GetHashCode() ?? 0 ^ Duration?.GetHashCode() ?? 0;
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
