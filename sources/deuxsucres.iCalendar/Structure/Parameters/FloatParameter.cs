using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Float parameter
    /// </summary>
    public class FloatParameter : CalPropertyParameter
    {

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeFloat(Value);
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            Value = 0;
            var b = reader.Parser.ParseFloat(value);
            if (b == null) return false;
            Value = b.Value;
            return true;
        }

        /// <summary>
        /// Cast to bool
        /// </summary>
        public static implicit operator double? (FloatParameter prop) { return prop?.Value; }

        /// <summary>
        /// Cast from bool
        /// </summary>
        public static implicit operator FloatParameter(double? value) { return value != null ? new FloatParameter { Value = value.Value } : null; }

        /// <summary>
        /// Cast to bool
        /// </summary>
        public static implicit operator double(FloatParameter prop) { return prop?.Value ?? 0; }

        /// <summary>
        /// Cast from bool
        /// </summary>
        public static implicit operator FloatParameter(double value) { return new FloatParameter { Value = value }; }

        /// <summary>
        /// Float value
        /// </summary>
        public double Value { get; set; }
    }
}
