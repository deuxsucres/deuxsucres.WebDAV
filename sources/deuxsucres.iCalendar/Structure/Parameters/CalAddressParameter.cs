using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{

    /// <summary>
    /// CAL-ADDRESS parameter
    /// </summary>
    public class CalAddressParameter : CalPropertyParameter
    {

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeCalAddress(Value);
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            Value = reader.Parser.ParseCalAddress(value, true);
            return Value != null;
        }

        /// <summary>
        /// Cast to Uri
        /// </summary>
        public static implicit operator Uri(CalAddressParameter param) => param?.Value;

        /// <summary>
        /// Cast from Uri
        /// </summary>
        public static implicit operator CalAddressParameter(Uri uri) => uri != null ? new CalAddressParameter { Value = uri } : null;

        /// <summary>
        /// Cast to string
        /// </summary>
        public static implicit operator string(CalAddressParameter param) => param?.Value?.ToString();

        /// <summary>
        /// Cast from string
        /// </summary>
        public static implicit operator CalAddressParameter(string str) => str != null ? new CalAddressParameter { Value = new Uri(str) } : null;

        /// <summary>
        /// Uri value
        /// </summary>
        public Uri Value { get; set; }
    }

}
