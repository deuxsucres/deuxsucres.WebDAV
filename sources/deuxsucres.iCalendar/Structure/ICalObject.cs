using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Object representation
    /// </summary>
    public interface ICalObject
    {
        /// <summary>
        /// Serialize the object
        /// </summary>
        void Serialize(ICalWriter writer);

        /// <summary>
        /// Deserialize the object
        /// </summary>
        void Deserialize(ICalReader reader);

        /// <summary>
        /// Name of the object
        /// </summary>
        string Name { get; }
    }
}
