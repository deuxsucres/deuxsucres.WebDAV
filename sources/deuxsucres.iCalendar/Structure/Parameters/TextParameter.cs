using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Text parameter
    /// </summary>
    public class TextParameter : CalPropertyParameter
    {

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeTextParameter(Value);
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            Value = reader.Parser.ParseText(value);
            return Value != null;
        }

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
        public static implicit operator string(TextParameter prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator TextParameter(string value) { return value != null ? new TextParameter { Value = value } : null; }

        /// <summary>
        /// Text value
        /// </summary>
        public string Value { get; set; }
    }
}
