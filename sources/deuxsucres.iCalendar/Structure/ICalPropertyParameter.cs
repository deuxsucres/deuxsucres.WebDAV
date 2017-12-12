using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Parameter of a property representation
    /// </summary>
    public interface ICalPropertyParameter
    {
        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <returns>True if the deserialization succeed.</returns>
        bool Deserialize(ICalReader reader, string name, string value);

        /// <summary>
        /// Serialize the parameter in the line
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="line">Line to serialize to</param>
        /// <returns>True if the serialisation succeed.</returns>
        bool Serialize(ICalWriter writer, ContentLine line);

        /// <summary>
        /// Name of the parameter
        /// </summary>
        string Name { get; set; }
    }
}
