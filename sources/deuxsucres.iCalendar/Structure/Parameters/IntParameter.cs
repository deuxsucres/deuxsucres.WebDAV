using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Integer parameter
    /// </summary>
    public class IntParameter : CalPropertyParameter
    {

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeInt(Value);
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            Value = 0;
            var b = reader.Parser.ParseInt(value);
            if (b == null) return false;
            Value = b.Value;
            return true;
        }

        /// <summary>
        /// Cast to bool
        /// </summary>
        public static implicit operator int? (IntParameter prop) { return prop?.Value; }

        /// <summary>
        /// Cast from bool
        /// </summary>
        public static implicit operator IntParameter(int? value) { return value != null ? new IntParameter { Value = value.Value } : null; }

        /// <summary>
        /// Cast to bool
        /// </summary>
        public static implicit operator int(IntParameter prop) { return prop?.Value ?? 0; }

        /// <summary>
        /// Cast from bool
        /// </summary>
        public static implicit operator IntParameter(int value) { return new IntParameter { Value = value }; }

        /// <summary>
        /// Integer value
        /// </summary>
        public int Value { get; set; }
    }
}
