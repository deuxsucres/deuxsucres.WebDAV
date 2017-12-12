using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// INTEGER property
    /// </summary>
    public class IntegerProperty : CalProperty, IFormattable
    {
        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = 0;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeInt(Value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var i = reader.Parser.ParseInt(line.Value);
            if (!i.HasValue) return false;
            Value = i.Value;
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
        /// Cast to int
        /// </summary>
        public static implicit operator int? (IntegerProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from int
        /// </summary>
        public static implicit operator IntegerProperty(int? value) { return value != null ? new IntegerProperty { Value = value.Value } : null; }

        /// <summary>
        /// Cast to int
        /// </summary>
        public static implicit operator int(IntegerProperty prop) { return prop?.Value ?? 0; }

        /// <summary>
        /// Cast from int
        /// </summary>
        public static implicit operator IntegerProperty(int value) { return new IntegerProperty { Value = value }; }

        /// <summary>
        /// Int value
        /// </summary>
        public virtual int Value { get; set; }

    }
}
