using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// FLOAT property
    /// </summary>
    public class FloatProperty : CalProperty, IFormattable
    {
        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = 0.0;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeFloat(Value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var fv = reader.Parser.ParseFloat(line.Value);
            if (!fv.HasValue) return false;
            Value = fv.Value;
            return true;
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
        /// To string
        /// </summary>
        public virtual string ToString(string format)
        {
            return Value.ToString(format);
        }

        /// <summary>
        /// To string
        /// </summary>
        public virtual string ToString(IFormatProvider formatProvider)
        {
            return Value.ToString(formatProvider);
        }

        /// <summary>
        /// To string
        /// </summary>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return Value.ToString(format, formatProvider);
        }

        /// <summary>
        /// Cast to double
        /// </summary>
        public static implicit operator double? (FloatProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from double
        /// </summary>
        public static implicit operator FloatProperty(double? value) { return value != null ? new FloatProperty { Value = value.Value } : null; }

        /// <summary>
        /// Cast to double
        /// </summary>
        public static implicit operator double(FloatProperty prop) { return prop?.Value ?? 0.0; }

        /// <summary>
        /// Cast from double
        /// </summary>
        public static implicit operator FloatProperty(double value) { return new FloatProperty { Value = value }; }

        /// <summary>
        /// Float value
        /// </summary>
        public virtual double Value { get; set; }

    }
}
