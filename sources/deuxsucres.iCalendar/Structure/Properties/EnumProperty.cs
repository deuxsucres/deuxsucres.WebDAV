using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Enum property
    /// </summary>
    public class EnumProperty<T> : CalProperty where T : struct
    {

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = null;
            StringValue = null;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeEnum(Value) ?? StringValue;
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            if (string.IsNullOrWhiteSpace(line.Value)) return false;
            StringValue = line.Value;
            Value = reader.Parser.ParseEnum<T>(line.Value);
            return true;
        }

        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return StringValue ?? Value?.ToString();
        }

        /// <summary>
        /// Cast to string
        /// </summary>
        public static implicit operator string(EnumProperty<T> prop) { return prop?.StringValue ?? prop?.Value?.ToString(); }

        /// <summary>
        /// Cast from string
        /// </summary>
        public static implicit operator EnumProperty<T>(string value) { return value != null ? new EnumProperty<T> { StringValue = value } : null; }

        /// <summary>
        /// Cast to enum
        /// </summary>
        public static implicit operator T? (EnumProperty<T> prop) { return prop?.Value; }

        /// <summary>
        /// Cast from enum
        /// </summary>
        public static implicit operator EnumProperty<T>(T? value) { return value != null ? new EnumProperty<T> { Value = value.Value } : null; }

        /// <summary>
        /// String value
        /// </summary>
        public string StringValue
        {
            get { return _stringValue; }
            set
            {
                _stringValue = value;
                if (_stringValue != null)
                    _value = CalendarParser.DefaultParseEnum<T>(_stringValue);
            }
        }
        private string _stringValue;

        /// <summary>
        /// Value
        /// </summary>
        public T? Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value != null)
                    _stringValue = CalendarParser.DefaultEncodeEnum(_value.Value);
            }
        }
        private T? _value;

    }
}
