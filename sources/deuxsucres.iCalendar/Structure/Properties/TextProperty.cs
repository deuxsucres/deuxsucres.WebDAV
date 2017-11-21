using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// TEXT value property
    /// </summary>
    public class TextProperty : CalProperty
    {

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return Value != null
                ? writer.Parser.EncodeText(Value)
                : null;
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            Value = reader.Parser.ParseText(line.Value);
            return Value != null;
        }

        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string(TextProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator TextProperty(string value) { return value != null ? new TextProperty { Value = value } : null; }

        /// <summary>
        /// Text value
        /// </summary>
        public string Value { get; set; }

    }
}
