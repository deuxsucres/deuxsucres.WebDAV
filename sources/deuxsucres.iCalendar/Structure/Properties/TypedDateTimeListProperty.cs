using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// A list of datetime with a type (date or datetime)
    /// </summary>
    public class TypedDateTimeListProperty : CalProperty
    {
        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = null;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            var p = writer.Parser;
            bool id = IsDate;
            return p.EncodeList(Value, v => id ? p.EncodeDate(v) : p.EncodeDateTime(v));
        }

        /// <summary>
        /// Deserialize a parameter
        /// </summary>
        protected override ICalPropertyParameter DeserializeParameter(ICalReader reader, ContentLine line, string name, string value)
        {
            if (name.IsEqual(Constants.VALUE))
                return reader.MakePropertyParameter<EnumParameter<ValueTypes>>(name, value);
            return base.DeserializeParameter(reader, line, name, value);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            bool id = reader.Parser.ParseEnum<ValueTypes>(line.GetParam(Constants.VALUE)) == ValueTypes.Date;
            var dts = reader.Parser
                .ParseList<DateTime?>(line.Value, n => id ? reader.Parser.ParseDate(n) : reader.Parser.ParseDateTime(n));
            Value = dts
                .Where(d => d.HasValue)
                .Select(d => d.Value)
                .ToList();
            return true;
        }

        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Value != null ? string.Join(", ", Value.Select(d => d.ToString())) : null;
        }

        /// <summary>
        /// Define the property as a date
        /// </summary>
        public void SetAsDate(IEnumerable<DateTime> value = null)
        {
            if (value != null)
                Value = value.Select(d => d.Date).ToList();
            else if (Value != null)
                Value = Value.Select(d => d.Date).ToList();
            SetParameter(new EnumParameter<ValueTypes>() { Name = Constants.VALUE, Value = ValueTypes.Date });
        }

        /// <summary>
        /// Define the property as a datetime
        /// </summary>
        public void SetAsDateTime(IEnumerable<DateTime> value = null)
        {
            if (value != null)
                Value = value.ToList();
            RemoveParameter(Constants.VALUE);
        }

        /// <summary>
        /// Cast to array date
        /// </summary>
        public static implicit operator DateTime[] (TypedDateTimeListProperty prop) { return prop?.Value?.ToArray(); }

        /// <summary>
        /// Cast from array date
        /// </summary>
        public static implicit operator TypedDateTimeListProperty(DateTime[] value) { return value != null ? new TypedDateTimeListProperty { Value = new List<DateTime>(value) } : null; }

        /// <summary>
        /// Indicates if the values are date. If false it's datetime.
        /// </summary>
        public bool IsDate => FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE)?.Value == ValueTypes.Date;

        /// <summary>
        /// Date values
        /// </summary>
        public List<DateTime> Value { get; set; }

        /// <summary>
        /// The time zone ID
        /// </summary>
        public TextParameter TimeZoneID
        {
            get { return FindParameter<TextParameter>(Constants.TZID); }
            set { SetParameter(value, Constants.TZID); }
        }
    }
}
