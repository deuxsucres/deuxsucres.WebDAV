using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// RECURRENCE property
    /// </summary>
    public class RecurrenceProperty : CalProperty
    {
        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeRecur(Value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            Value = reader.Parser.ParseRecur(line.Value);
            return Value != null;
        }

        #endregion

        /// <summary>
        /// Cast to recurrence
        /// </summary>
        public static implicit operator Recurrence(RecurrenceProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from recurrence
        /// </summary>
        public static implicit operator RecurrenceProperty(Recurrence value) { return value != null ? new RecurrenceProperty { Value = value } : null; }

        /// <summary>
        /// Recurrence value
        /// </summary>
        public virtual Recurrence Value { get; set; }

    }
}
