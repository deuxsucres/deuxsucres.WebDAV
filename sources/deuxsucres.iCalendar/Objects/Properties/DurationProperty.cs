using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// DURATION property
    /// </summary>
    public class DurationProperty : CalProperty
    {

        /// <summary>
        /// New property
        /// </summary>
        public DurationProperty()
        {
            Name = Constants.DURATION;
        }

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
        /// Serialize this value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeDuration(Value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var du = reader.Parser.ParseDuration(line.Value);
            if (!du.HasValue) return false;
            Value = du.Value;
            return true;
        }

        #endregion

        /// <summary>
        /// Cast to TimeSpan
        /// </summary>
        public static implicit operator TimeSpan? (DurationProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from TimeSpan
        /// </summary>
        public static implicit operator DurationProperty(TimeSpan? value) { return value != null ? new DurationProperty { Value = value.Value } : null; }

        /// <summary>
        /// Cast to TimeSpan
        /// </summary>
        public static implicit operator TimeSpan(DurationProperty prop) { return prop?.Value ?? TimeSpan.Zero; }

        /// <summary>
        /// Cast from TimeSpan
        /// </summary>
        public static implicit operator DurationProperty(TimeSpan value) { return new DurationProperty { Value = value }; }

        /// <summary>
        /// TimeSpan value
        /// </summary>
        public virtual TimeSpan Value { get; set; }
    }
}
