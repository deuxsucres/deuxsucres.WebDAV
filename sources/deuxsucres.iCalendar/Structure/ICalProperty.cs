using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Property representation
    /// </summary>
    public interface ICalProperty
    {
        // TODO Uncomment when serialisation is ready
        ///// <summary>
        ///// Serialize the property
        ///// </summary>
        //void Serialize(ICalWriter writer);

        ///// <summary>
        ///// Deserialize the property
        ///// </summary>
        ///// <param name="reader">Reader</param>
        ///// <param name="line">Line</param>
        //void Deserialize(ICalReader reader, ContentLine line);

        /// <summary>
        /// Name of the property
        /// </summary>
        string Name { get; set; }

    }
}
