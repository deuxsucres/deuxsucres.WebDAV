using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// List of CAL-ADDRESS parameter
    /// </summary>
    public class CalAddressesParameter : CalPropertyParameter
    {

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeList(Value, u => writer.Parser.EncodeCalAddress(u));
        }

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        protected override bool InternalDeserialize(ICalReader reader, string name, string value)
        {
            (Value ?? (Value = new List<Uri>())).Clear();
            var prs = reader.Parser;
            Value.AddRange(prs.ParseList(value, s => prs.ParseCalAddress(s, true)).Where(a => a != null));
            return Value.Count > 0;
        }

        /// <summary>
        /// Cast to string array
        /// </summary>
        public static implicit operator string[] (CalAddressesParameter param)
            => param?.Value?.Select(u => u.ToString())?.ToArray();

        /// <summary>
        /// Cast from string array
        /// </summary>
        public static implicit operator CalAddressesParameter(string[] uris) => uris != null ? new CalAddressesParameter
        {
            Value = (uris ?? Enumerable.Empty<string>()).Where(s => s != null).Select(s => new Uri(s)).ToList()
        } : null;

        /// <summary>
        /// Cast to string list
        /// </summary>
        public static implicit operator List<string>(CalAddressesParameter param)
            => param?.Value?.Select(u => u.ToString())?.ToList();

        /// <summary>
        /// Cast from string list
        /// </summary>
        public static implicit operator CalAddressesParameter(List<string> uris) => uris != null ? new CalAddressesParameter
        {
            Value = (uris ?? Enumerable.Empty<string>()).Where(s => s != null).Select(s => new Uri(s)).ToList()
        } : null;

        /// <summary>
        /// Cast to uri array
        /// </summary>
        public static implicit operator Uri[] (CalAddressesParameter param)
            => param?.Value?.ToArray();

        /// <summary>
        /// Cast from uri array
        /// </summary>
        public static implicit operator CalAddressesParameter(Uri[] uris) => uris != null ? new CalAddressesParameter
        {
            Value = (uris ?? Enumerable.Empty<Uri>()).Where(u => u != null).ToList()
        } : null;

        /// <summary>
        /// Cast to uri list
        /// </summary>
        public static implicit operator List<Uri>(CalAddressesParameter param)
            => param?.Value?.ToList();

        /// <summary>
        /// Cast from uri list
        /// </summary>
        public static implicit operator CalAddressesParameter(List<Uri> uris) => uris != null ? new CalAddressesParameter
        {
            Value = (uris ?? Enumerable.Empty<Uri>()).Where(u => u != null).ToList()
        } : null;

        /// <summary>
        /// Uris value
        /// </summary>
        public List<Uri> Value { get; set; } = new List<Uri>();

    }
}
