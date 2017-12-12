using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// UTC-OFFSET
    /// </summary>
    public class UtcOffsetProperty : CalProperty, IFormattable
    {

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = TimeSpan.Zero;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeUtcOffset(Value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var du = reader.Parser.ParseUtcOffset(line.Value);
            if (!du.HasValue) return false;
            Value = du.Value;
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
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return Value.ToString(format, formatProvider);
        }

        /// <summary>
        /// Cast to TimeSpan
        /// </summary>
        public static implicit operator TimeSpan? (UtcOffsetProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from TimeSpan
        /// </summary>
        public static implicit operator UtcOffsetProperty(TimeSpan? value) { return value != null ? new UtcOffsetProperty { Value = value.Value } : null; }

        /// <summary>
        /// Cast to TimeSpan
        /// </summary>
        public static implicit operator TimeSpan(UtcOffsetProperty prop) { return prop?.Value ?? TimeSpan.Zero; }

        /// <summary>
        /// Cast from TimeSpan
        /// </summary>
        public static implicit operator UtcOffsetProperty(TimeSpan value) { return new UtcOffsetProperty { Value = value }; }

        /// <summary>
        /// TimeSpan value
        /// </summary>
        public virtual TimeSpan Value { get; set; }

    }
}
