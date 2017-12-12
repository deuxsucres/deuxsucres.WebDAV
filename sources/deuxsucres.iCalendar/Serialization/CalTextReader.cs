using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace deuxsucres.iCalendar.Serialization
{
    /// <summary>
    /// Calendar reader to a TextReader
    /// </summary>
    public class CalTextReader : ICalReader
    {
        List<string> _currentContentLine = new List<string>();

        /// <summary>
        /// Create a new reader
        /// </summary>
        public CalTextReader(CalendarParser parser, TextReader source, bool strict)
        {
            Parser = parser ?? throw new ArgumentNullException(nameof(parser));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            CurrentLineNumber = 1;
            CurrentLine = null;
            StrictMode = strict;
        }

        /// <summary>
        /// Read the next content line
        /// </summary>
        public virtual ContentLine ReadNextLine()
        {
            CurrentLineNumber += _currentContentLine.Count;
            _currentContentLine.Clear();
            CurrentLine = Parser.ReadContentLine(Source, _currentContentLine);
            //if (CurrentLine == null && _currentContentLine.Count > 0)
            //    throw new CalSyntaxError(string.Format(SR.Err_InvalidContentLine, CurrentLineNumber, string.Join("\n", _currentContentLine)));
            return CurrentLine;
        }

        #region Properties

        /// <summary>
        /// Create a default parameter from his name
        /// </summary>
        public virtual ICalPropertyParameter CreateDefaultParameter(string name)
        {
            return SerializationHelpers.DefaultCreateParameter(name);
        }

        /// <summary>
        /// Create an instance of a typed property
        /// </summary>
        protected virtual T CreatePropertyInstance<T>() where T : ICalProperty
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Create a default property from his name
        /// </summary>
        public virtual ICalProperty CreateDefaultProperty(string name)
        {
            return SerializationHelpers.DefaultCreateProperty(name);
        }

        /// <summary>
        /// Deserialize a property
        /// </summary>
        protected virtual void DeserializeProperty(ICalProperty property, ContentLine line)
        {
            property.Deserialize(this, line);
        }

        /// <summary>
        /// Make a default property
        /// </summary>
        public ICalProperty MakeProperty(ContentLine line)
        {
            if (line == null) return null;
            var prop = CreateDefaultProperty(line.Name);
            DeserializeProperty(prop, line);
            return prop;
        }

        /// <summary>
        /// Make a property from a content line
        /// </summary>
        public virtual T MakeProperty<T>(ContentLine line) where T : ICalProperty
        {
            if (line == null) return default(T);
            var prop = CreatePropertyInstance<T>();
            DeserializeProperty(prop, line);
            return prop;
        }

        #endregion

        #region Components

        /// <summary>
        /// Create an instance of a component
        /// </summary>
        protected virtual T CreateComponentInstance<T>() where T : CalComponent
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// Create a default component from his name
        /// </summary>
        public virtual CalComponent CreateDefaultComponent(string name)
        {
            return SerializationHelpers.DefaultCreateComponent(name);
        }

        /// <summary>
        /// Read a component
        /// </summary>
        protected virtual void ReadComponent(CalComponent component)
        {
            component.Deserialize(this);
        }

        /// <summary>
        /// Deserialize a default component
        /// </summary>
        public CalComponent ReadComponent(ContentLine line)
        {
            if (line == null) return null;
            var comp = CreateDefaultComponent(line.Value);
            ReadComponent(comp);
            return comp;
        }

        /// <summary>
        /// Deserialize a component
        /// </summary>
        public virtual T ReadComponent<T>(ContentLine line) where T : CalComponent
        {
            if (line == null) return null;
            var comp = CreateComponentInstance<T>();
            ReadComponent(comp);
            return comp;
        }

        #endregion

        /// <summary>
        /// Parser
        /// </summary>
        public CalendarParser Parser { get; private set; }

        /// <summary>
        /// Source
        /// </summary>
        public TextReader Source { get; private set; }

        /// <summary>
        /// Current line
        /// </summary>
        public ContentLine CurrentLine { get; private set; }

        /// <summary>
        /// Current number line
        /// </summary>
        public int CurrentLineNumber { get; private set; }

        /// <summary>
        /// Indicates if the reader is strict on the format
        /// </summary>
        public bool StrictMode { get; private set; }

    }
}
