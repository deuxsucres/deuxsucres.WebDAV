using deuxsucres.iCalendar.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Serialization
{
    /// <summary>
    /// Writer
    /// </summary>
    public interface ICalWriter
    {
        /// <summary>
        /// Write a BEGIN line
        /// </summary>
        ICalWriter WriteBegin(string value);

        /// <summary>
        /// Write a END line
        /// </summary>
        ICalWriter WriteEnd(string value);

        /// <summary>
        /// Write a content line
        /// </summary>
        ICalWriter WriteLine(ContentLine line);

        /// <summary>
        /// Current line number
        /// </summary>
        int CurrentLineNumber { get; }

        /// <summary>
        /// Current parser used
        /// </summary>
        CalendarParser Parser { get; }
    }
}
