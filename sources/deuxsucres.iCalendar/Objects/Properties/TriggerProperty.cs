using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// TRIGGER property
    /// </summary>
    public class TriggerProperty : CalProperty
    {
        /// <summary>
        /// New property
        /// </summary>
        public TriggerProperty()
        {
            Name = Constants.TRIGGER;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            RelativeValue = null;
            AbsoluteValue = null;
        }

        #region Serialization

        /// <summary>
        /// Serialize a value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            var vt = GetParameter<EnumParameter<ValueTypes>>(Constants.VALUE)?.Value;
            if (vt == ValueTypes.Duration)
            {
                return RelativeValue == null ? null : writer.Parser.EncodeDuration(RelativeValue.Value);
            }
            else if (vt == ValueTypes.Date_Time)
            {
                return AbsoluteValue == null ? null : writer.Parser.EncodeDateTime(AbsoluteValue.Value);
            }
            else if (vt == ValueTypes.Date)
            {
                return AbsoluteValue == null ? null : writer.Parser.EncodeDate(AbsoluteValue.Value);
            }
            string val =
                (RelativeValue == null ? null : writer.Parser.EncodeDuration(RelativeValue.Value))
                ?? (AbsoluteValue == null ? null : writer.Parser.EncodeDateTime(AbsoluteValue.Value));
            return val;
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var vt = reader.Parser.ParseEnum<ValueTypes>(line.GetParam(Constants.VALUE));
            if (vt == ValueTypes.Duration)
            {
                RelativeValue = reader.Parser.ParseDuration(line.Value);
            }
            else if (vt == ValueTypes.Date_Time || vt == ValueTypes.Date)
            {
                AbsoluteValue = reader.Parser.ParseDateTime(line.Value);
            }
            else
            {
                RelativeValue = reader.Parser.ParseDuration(line.Value);
                if (RelativeValue == null)
                    AbsoluteValue = reader.Parser.ParseDateTime(line.Value);
            }
            return RelativeValue.HasValue || AbsoluteValue.HasValue;
        }

        #endregion

        #region Casts

        /// <summary>
        /// Cast to datetime
        /// </summary>
        public static implicit operator DateTime? (TriggerProperty prop) => prop?.AbsoluteValue;

        /// <summary>
        /// Cast from datetime
        /// </summary>
        public static implicit operator TriggerProperty(DateTime? date) => date != null ? new TriggerProperty { AbsoluteValue = date } : null;

        /// <summary>
        /// Cast to timespan
        /// </summary>
        public static implicit operator TimeSpan? (TriggerProperty prop) => prop?.RelativeValue;

        /// <summary>
        /// Cast from timespan
        /// </summary>
        public static implicit operator TriggerProperty(TimeSpan? time) => time != null ? new TriggerProperty { RelativeValue = time } : null;

        #endregion

        /// <summary>
        /// Relative value in duration
        /// </summary>
        public TimeSpan? RelativeValue
        {
            get { return _relativeValue; }
            set
            {
                RemoveParameter(Constants.VALUE);
                _relativeValue = value;
                if (_relativeValue != null)
                {
                    _absoluteValue = null;
                    SetParameter(new EnumParameter<ValueTypes> { Name = Constants.VALUE, Value = ValueTypes.Duration });
                }
            }
        }
        private TimeSpan? _relativeValue;

        /// <summary>
        /// Absolute value in date
        /// </summary>
        public DateTime? AbsoluteValue
        {
            get { return _absoluteValue; }
            set
            {
                RemoveParameter(Constants.VALUE);
                _absoluteValue = value;
                if (_absoluteValue != null)
                {
                    _relativeValue = null;
                    SetParameter(new EnumParameter<ValueTypes>
                    {
                        Name = Constants.VALUE,
                        Value = _absoluteValue.Value == _absoluteValue.Value.Date ? ValueTypes.Date : ValueTypes.Date_Time
                    });
                }
            }
        }
        private DateTime? _absoluteValue;

    }
}
