using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// URI value property
    /// </summary>
    public class UriProperty : CalProperty
    {
        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = null;
        }

        #region Serialization

        /// <summary>
        /// Serialize a value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeUri(Value, false);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            Value = reader.Parser.ParseUri(line.Value, false);
            return Value != null;
        }

        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Cast to Uri
        /// </summary>
        public static implicit operator Uri(UriProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from Uri
        /// </summary>
        public static implicit operator UriProperty(Uri value) { return value != null ? new UriProperty { Value = value } : null; }

        /// <summary>
        /// Cast to String
        /// </summary>
        public static implicit operator String(UriProperty prop) { return prop?.Value?.ToString(); }

        /// <summary>
        /// Cast from String
        /// </summary>
        public static implicit operator UriProperty(string value) { return value != null ? new UriProperty { Value = new Uri(value) } : null; }

        /// <summary>
        /// Uri value
        /// </summary>
        public Uri Value { get; set; }

    }
}
