using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// DATE-TIME property
    /// </summary>
    public class DateTimeProperty : CalProperty, IFormattable
    {
        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = DateTime.MinValue;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeDateTime(Value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var dt = reader.Parser.ParseDateTime(line.Value);
            if (!dt.HasValue) return false;
            Value = dt.Value;
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
        /// Cast to date
        /// </summary>
        public static implicit operator DateTime? (DateTimeProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from date
        /// </summary>
        public static implicit operator DateTimeProperty(DateTime? value) { return value != null ? new DateTimeProperty { Value = value.Value } : null; }

        /// <summary>
        /// Cast to date
        /// </summary>
        public static implicit operator DateTime(DateTimeProperty prop) { return prop?.Value ?? DateTime.MinValue; }

        /// <summary>
        /// Cast from date
        /// </summary>
        public static implicit operator DateTimeProperty(DateTime value) { return new DateTimeProperty { Value = value }; }

        /// <summary>
        /// Datetime value
        /// </summary>
        public virtual DateTime Value { get; set; }
    }
}
