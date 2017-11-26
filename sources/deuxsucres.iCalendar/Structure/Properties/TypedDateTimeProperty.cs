using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// A datetime with a type (date or datetime)
    /// </summary>
    public class TypedDateTimeProperty : DateTimeProperty
    {

        #region Serialization

        /// <summary>
        /// Serialize value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            if (IsDate)
                return writer.Parser.EncodeDate(Value);
            return base.SerializeValue(writer, line);
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
            var dt = reader.Parser.ParseDateTime(line.Value, false);
            if (dt.HasValue)
            {
                SetAsDateTime(dt);
                return true;
            }
            dt = reader.Parser.ParseDate(line.Value);
            if (!dt.HasValue) return false;
            SetAsDate(dt);
            return true;
        }

        #endregion

        /// <summary>
        /// Define the property as a date
        /// </summary>
        public void SetAsDate(DateTime? value = null)
        {
            if (value.HasValue)
            {
                base.Value = value.Value.Date;
            }
            else
            {
                base.Value = base.Value.Date;
            }
            SetParameter(new EnumParameter<ValueTypes>() { Name = Constants.VALUE, Value = ValueTypes.Date });
        }

        /// <summary>
        /// Define the property as a datetime
        /// </summary>
        public void SetAsDateTime(DateTime? value = null)
        {
            if (value.HasValue)
            {
                base.Value = value.Value;
            }
            RemoveParameter(Constants.VALUE);
        }

        /// <summary>
        /// Cast to date
        /// </summary>
        public static implicit operator DateTime? (TypedDateTimeProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from date
        /// </summary>
        public static implicit operator TypedDateTimeProperty(DateTime? value) { return value != null ? new TypedDateTimeProperty { Value = value.Value } : null; }

        /// <summary>
        /// Cast to date
        /// </summary>
        public static implicit operator DateTime(TypedDateTimeProperty prop) { return prop?.Value ?? DateTime.MinValue; }

        /// <summary>
        /// Cast from date
        /// </summary>
        public static implicit operator TypedDateTimeProperty(DateTime value) { return new TypedDateTimeProperty { Value = value }; }

        /// <summary>
        /// Indicates if the value is a date. If false it's a datetime.
        /// </summary>
        public bool IsDate => FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE)?.Value == ValueTypes.Date;

        /// <summary>
        /// Date value
        /// </summary>
        public override DateTime Value
        {
            get { return IsDate ? base.Value.Date : base.Value; }
            set
            {
                base.Value = value;
                if (Value.Date == Value)
                    SetAsDate();
                else
                    SetAsDateTime();
            }
        }

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
