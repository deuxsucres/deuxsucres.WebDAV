using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Base of property parameter
    /// </summary>
    public abstract class CalPropertyParameter : ICalPropertyParameter
    {

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <returns>True if the deserialization succeed.</returns>
        protected abstract bool InternalDeserialize(ICalReader reader, string name, string value);

        /// <summary>
        /// Deserialize the parameter
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <returns>True if the deserialization succeed.</returns>
        public bool Deserialize(ICalReader reader, string name, string value)
        {
            Name = null;
            bool r = InternalDeserialize(reader, name, value);
            if (r) Name = name;
            return r;
        }

        /// <summary>
        /// Serialize the value of the parameter
        /// </summary>
        protected virtual string SerializeValue(ICalWriter writer, ContentLine line)
        {
            throw new NotImplementedException($"{GetType().Name}.{nameof(SerializeValue)}");
        }

        /// <summary>
        /// Serialize the parameter
        /// </summary>
        public virtual bool Serialize(ICalWriter writer, ContentLine line)
        {
            if (string.IsNullOrWhiteSpace(Name)) return false;
            string val = SerializeValue(writer, line);
            if (val == null) return false;
            line.SetParam(Name.ToUpper(), val);
            return true;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

    }
}
