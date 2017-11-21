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
        /// Uris value
        /// </summary>
        public List<Uri> Value { get; set; } = new List<Uri>();

    }
}
