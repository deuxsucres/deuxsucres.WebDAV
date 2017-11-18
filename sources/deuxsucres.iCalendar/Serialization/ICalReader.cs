using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Serialization
{
    /// <summary>
    /// Reader
    /// </summary>
    public interface ICalReader
    {

        /// <summary>
        /// Read the next contentline
        /// </summary>
        /// <returns>The content line or null if eod of file.</returns>
        ContentLine ReadNextLine();

        /// <summary>
        /// Create a default parameter from his name
        /// </summary>
        ICalPropertyParameter CreateDefaultParameter(string name);

        /// <summary>
        /// Make a property from a content line
        /// </summary>
        ICalProperty MakeProperty(ContentLine line);

        /// <summary>
        /// Make a typed property from a content line
        /// </summary>
        T MakeProperty<T>(ContentLine line) where T : ICalProperty;

        // TODO Uncomment when CalComponent will be created
        ///// <summary>
        ///// Read a component from a "BEGIN" line
        ///// </summary>
        //CalComponent ReadComponent(ContentLine line);

        ///// <summary>
        ///// Read a typed component
        ///// </summary>
        //T ReadComponent<T>(ContentLine line) where T : CalComponent;

        /// <summary>
        /// Current line
        /// </summary>
        ContentLine CurrentLine { get; }

        /// <summary>
        /// Current line number
        /// </summary>
        int CurrentLineNumber { get; }

        /// <summary>
        /// Indicates if the reader is strict on the format
        /// </summary>
        bool StrictMode { get; }

        /// <summary>
        /// Current parser used
        /// </summary>
        CalendarParser Parser { get; }
    }
}
