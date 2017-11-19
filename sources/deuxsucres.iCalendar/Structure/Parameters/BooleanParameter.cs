using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Boolean parameter
    /// </summary>
    public class BooleanParameter : CalPropertyParameter
    {

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeBoolean(Value);
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            Value = false;
            var b = reader.Parser.ParseBoolean(value);
            if (b == null) return false;
            Value = b.Value;
            return true;
        }

        /// <summary>
        /// Cast to bool
        /// </summary>
        public static implicit operator bool? (BooleanParameter prop) { return prop?.Value; }

        /// <summary>
        /// Cast from bool
        /// </summary>
        public static implicit operator BooleanParameter(bool? value) { return value != null ? new BooleanParameter { Value = value.Value } : null; }

        /// <summary>
        /// Cast to bool
        /// </summary>
        public static implicit operator bool(BooleanParameter prop) { return prop?.Value ?? false; }

        /// <summary>
        /// Cast from bool
        /// </summary>
        public static implicit operator BooleanParameter(bool value) { return new BooleanParameter { Value = value }; }

        /// <summary>
        /// Boolean value
        /// </summary>
        public bool Value { get; set; }
    }
}
