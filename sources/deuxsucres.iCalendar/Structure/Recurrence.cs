using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Reccurence rule
    /// </summary>
    public class Recurrence
    {
        /// <summary>
        /// Frequencies
        /// </summary>
        public enum Frequencies
        {
            /// <summary>
            /// Per second
            /// </summary>
            Secondly,
            /// <summary>
            /// Per minute
            /// </summary>
            Minutely,
            /// <summary>
            /// Per hour
            /// </summary>
            Hourly,
            /// <summary>
            /// Per day
            /// </summary>
            Daily,
            /// <summary>
            /// Per week
            /// </summary>
            Weekly,
            /// <summary>
            /// Per month
            /// </summary>
            Monthly,
            /// <summary>
            /// Per year
            /// </summary>
            Yearly
        }

        /// <summary>
        /// Weekdays
        /// </summary>
        public enum Weekdays
        {
            /// <summary>
            /// Sunday
            /// </summary>
            SU,
            /// <summary>
            /// Monday
            /// </summary>
            MO,
            /// <summary>
            /// Tuesday
            /// </summary>
            TU,
            /// <summary>
            /// Wednesday
            /// </summary>
            WE,
            /// <summary>
            /// Thursday
            /// </summary>
            TH,
            /// <summary>
            /// Friday
            /// </summary>
            FR,
            /// <summary>
            /// Saturday
            /// </summary>
            SA
        }

        /// <summary>
        /// Weekday with order
        /// </summary>
        public struct WeekdayNum
        {
            public Weekdays Weekday { get; set; }
            public int? Order { get; set; }
        }

        /// <summary>
        /// Add an other part
        /// </summary>
        public void AddOther(string name, string value)
        {
            if (Others == null)
                Others = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Others[name] = value;
        }

        /// <summary>
        /// Frequency
        /// </summary>
        public Frequencies Frequency { get; set; }

        /// <summary>
        /// Recurrence until
        /// </summary>
        public DateTime? Until { get; set; }

        /// <summary>
        /// Recurrence count
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Interval
        /// </summary>
        public int? Interval { get; set; }

        /// <summary>
        /// By second
        /// </summary>
        public IList<int> BySecond { get; set; }

        /// <summary>
        /// By minute
        /// </summary>
        public IList<int> ByMinute { get; set; }

        /// <summary>
        /// By hour
        /// </summary>
        public IList<int> ByHour { get; set; }

        /// <summary>
        /// By week day
        /// </summary>
        public IList<WeekdayNum> ByDay { get; set; }

        /// <summary>
        /// By month day
        /// </summary>
        public IList<int> ByMonthDay { get; set; }

        /// <summary>
        /// By year day
        /// </summary>
        public IList<int> ByYearDay { get; set; }

        /// <summary>
        /// By week number
        /// </summary>
        public IList<int> ByWeekNo { get; set; }

        /// <summary>
        /// By month number
        /// </summary>
        public IList<int> ByMonth { get; set; }

        /// <summary>
        /// By set position
        /// </summary>
        public IList<int> BySetPos { get; set; }

        /// <summary>
        /// Start of week
        /// </summary>
        public Weekdays StartWeek { get; set; } = Weekdays.MO;

        /// <summary>
        /// Other non standard parts
        /// </summary>
        public IDictionary<string, string> Others { get; set; }
    }
}
