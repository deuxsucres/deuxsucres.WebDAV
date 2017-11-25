using deuxsucres.iCalendar.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace deuxsucres.iCalendar.Serialization
{
    /// <summary>
    /// Calendar writer to a TextWriter
    /// </summary>
    public class CalTextWriter : ICalWriter
    {
        
        /// <summary>
        /// Create a new writer
        /// </summary>
        public CalTextWriter(CalendarParser parser, TextWriter source)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            CurrentLineNumber = 1;
        }

        /// <summary>
        /// Write a line
        /// </summary>
        protected virtual void WriteLine(string line)
        {
            CurrentLineNumber += Parser.WriteLine(Source, line);
        }

        /// <summary>
        /// Write a BEGIN line
        /// </summary>
        public ICalWriter WriteBegin(string value)
        {
            WriteLine($"{Constants.BEGIN}:{value.ToUpper()}");
            return this;
        }

        /// <summary>
        /// Write a END line
        /// </summary>
        public ICalWriter WriteEnd(string value)
        {
            WriteLine($"{Constants.END}:{value.ToUpper()}");
            return this;
        }

        /// <summary>
        /// Write a content line
        /// </summary>
        public ICalWriter WriteLine(ContentLine line)
        {
            CurrentLineNumber += Parser.WriteContentLine(Source, line);
            return this;
        }

        /// <summary>
        /// Parser
        /// </summary>
        public CalendarParser Parser { get; private set; }

        /// <summary>
        /// Source
        /// </summary>
        public TextWriter Source { get; private set; }

        /// <summary>
        /// Current number line
        /// </summary>
        public int CurrentLineNumber { get; private set; }
    }
}
