using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// RDATE
    /// </summary>
    public class RecurDateProperty : CalProperty
    {

        /// <summary>
        /// New property
        /// </summary>
        public RecurDateProperty()
        {
            Name = Constants.RDATE;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _periodsValue = null;
            _datesValue = null;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            switch (ValueType)
            {
                case ValueTypes.Period:
                    return writer.Parser.EncodeList(PeriodsValue, p => writer.Parser.EncodePeriod(p));
                case ValueTypes.Date:
                    return writer.Parser.EncodeList(DatesValue, d => writer.Parser.EncodeDate(d));
                case ValueTypes.Date_Time:
                default:
                    return writer.Parser.EncodeList(DatesValue, d => writer.Parser.EncodeDateTime(d));
            }
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            var vt = reader.Parser.ParseEnum<ValueTypes>(line.GetParam(Constants.VALUE)) ?? ValueTypes.Date_Time;
            //if (vt != ValueTypes.Date_Time && vt != ValueTypes.Date && vt != ValueTypes.Period)
            //    vt = ValueTypes.Date_Time;
            if (vt == ValueTypes.Period)
            {
                PeriodsValue = reader.Parser
                    .ParseList(line.Value, n => reader.Parser.ParsePeriod(n))
                    .Where(d => d != null)
                    .Select(d => d)
                    .ToList();
            }
            else
            {
                var dts = vt == ValueTypes.Date
                    ? reader.Parser.ParseList(line.Value, n => reader.Parser.ParseDate(n))
                    : reader.Parser.ParseList(line.Value, n => reader.Parser.ParseDateTime(n));
                DatesValue = dts
                    .Where(d => d.HasValue)
                    .Select(d => d.Value)
                    .ToList();
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Define the property as a date
        /// </summary>
        public void SetAsDate(IEnumerable<DateTime> dates = null)
        {
            SetParameter(new EnumParameter<ValueTypes>() { Name = Constants.VALUE, Value = ValueTypes.Date });
            _periodsValue = null;
            _datesValue = dates?.ToList() ?? _datesValue;
        }

        /// <summary>
        /// Define the property as a datetime
        /// </summary>
        public void SetAsDateTime(IEnumerable<DateTime> dates = null)
        {
            RemoveParameter(Constants.VALUE);
            _periodsValue = null;
            _datesValue = dates?.ToList() ?? _datesValue;
        }

        /// <summary>
        /// Define the property as a period
        /// </summary>
        public void SetAsPeriod(IEnumerable<Period> periods = null)
        {
            SetParameter(new EnumParameter<ValueTypes>() { Name = Constants.VALUE, Value = ValueTypes.Period });
            _datesValue = null;
            _periodsValue = periods?.ToList() ?? _periodsValue;
        }

        /// <summary>
        /// Cast to DateTime array
        /// </summary>
        public static implicit operator DateTime[] (RecurDateProperty prop) => prop?.DatesValue?.ToArray();

        /// <summary>
        /// Cast from DateTime array
        /// </summary>
        public static implicit operator RecurDateProperty(DateTime[] dates) => new RecurDateProperty { DatesValue = dates?.ToList() };

        /// <summary>
        /// Cast to Period array
        /// </summary>
        public static implicit operator Period[] (RecurDateProperty prop) => prop?.PeriodsValue?.ToArray();

        /// <summary>
        /// Cast from Period array
        /// </summary>
        public static implicit operator RecurDateProperty (Period[] periods) => new RecurDateProperty { PeriodsValue = periods?.ToList() };

        /// <summary>
        /// Indicates the value types
        /// </summary>
        public ValueTypes ValueType => FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE)?.Value ?? ValueTypes.Date_Time;

        /// <summary>
        /// Date values
        /// </summary>
        public List<DateTime> DatesValue
        {
            get { return _datesValue; }
            set
            {
                _datesValue = value;
                if (_datesValue != null)
                {
                    _periodsValue = null;
                    RemoveParameter(Constants.VALUE);
                }
            }
        }
        private List<DateTime> _datesValue;

        /// <summary>
        /// Period values
        /// </summary>
        public List<Period> PeriodsValue
        {
            get { return _periodsValue; }
            set
            {
                _periodsValue = value;
                if (_periodsValue != null)
                {
                    _datesValue = null;
                    SetParameter(new EnumParameter<ValueTypes>() { Name = Constants.VALUE, Value = ValueTypes.Period });
                }
            }
        }
        private List<Period> _periodsValue;

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
