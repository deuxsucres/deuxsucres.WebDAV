using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{

    /// <summary>
    /// Uri parameter
    /// </summary>
    public class UriParameter : CalPropertyParameter
    {

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeUri(Value);
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            Value = reader.Parser.ParseUri(value, true);
            return Value != null;
        }

        /// <summary>
        /// Cast to Uri
        /// </summary>
        public static implicit operator Uri(UriParameter param) => param?.Value;

        /// <summary>
        /// Cast from Uri
        /// </summary>
        public static implicit operator UriParameter(Uri uri) => uri != null ? new UriParameter { Value = uri } : null;

        /// <summary>
        /// Cast to string
        /// </summary>
        public static implicit operator string(UriParameter param) => param?.Value?.ToString();

        /// <summary>
        /// Cast from string
        /// </summary>
        public static implicit operator UriParameter(string str) => str != null ? new UriParameter { Value = new Uri(str) } : null;

        /// <summary>
        /// Uri value
        /// </summary>
        public Uri Value { get; set; }
    }

}
