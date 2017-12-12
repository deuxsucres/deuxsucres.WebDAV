using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// List of TEXT property
    /// </summary>
    public class TextListProperty : CalProperty
    {

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeList(Value, v => v);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            Value = reader.Parser.ParseTextList(line.Value)?.ToList();
            return Value != null;
        }

        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Value != null ? string.Join(", ", Value.Select(v => v.ToString())) : null;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string[] (TextListProperty prop) { return prop?.Value?.ToArray(); }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator TextListProperty(string[] value) { return value != null ? new TextListProperty { Value = value.ToList() } : null; }

        /// <summary>
        /// Text values
        /// </summary>
        public List<string> Value { get; set; }

    }
}
