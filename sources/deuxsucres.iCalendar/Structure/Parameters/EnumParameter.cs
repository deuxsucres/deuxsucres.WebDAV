using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Enum parameter
    /// </summary>
    public class EnumParameter<T> : CalPropertyParameter where T : struct
    {

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeEnum(Value) ?? StringValue;
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            StringValue = null;
            Value = null;
            if (string.IsNullOrWhiteSpace(value)) return false;
            StringValue = value;
            Value = reader.Parser.ParseEnum<T>(value);
            return true;
        }

        /// <summary>
        /// Cast to string
        /// </summary>
        public static implicit operator string(EnumParameter<T> prop) { return prop?.StringValue ?? prop?.Value?.ToString(); }

        /// <summary>
        /// Cast from string
        /// </summary>
        public static implicit operator EnumParameter<T>(string value) { return value != null ? new EnumParameter<T> { StringValue = value } : null; }

        /// <summary>
        /// Cast to enum
        /// </summary>
        public static implicit operator T? (EnumParameter<T> prop) { return prop?.Value; }

        /// <summary>
        /// Cast from enum
        /// </summary>
        public static implicit operator EnumParameter<T>(T? value) { return value != null ? new EnumParameter<T> { Value = value.Value } : null; }

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
                if (_value.HasValue)
                    _stringValue = CalendarParser.DefaultEncodeEnum(_value.Value);
            }
        }
        private T? _value;

    }
}
