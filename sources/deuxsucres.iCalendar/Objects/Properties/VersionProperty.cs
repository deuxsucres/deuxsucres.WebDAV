using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// VERSION
    /// </summary>
    public class VersionProperty : TextProperty
    {

        /// <summary>
        /// New property
        /// </summary>
        public VersionProperty()
        {
            Name = Constants.VERSION;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            MinVersion = null;
        }

        /// <summary>
        /// Serialize a value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            string v = writer.Parser.EncodeText(Value);
            string mv = writer.Parser.EncodeText(MinVersion);
            if (v == null && mv != null)
            {
                v = mv;
                mv = null;
            }
            if (v == null) return null;
            return mv != null ? $"{mv};{v}" : v;
        }

        /// <summary>
        /// Deszrialize value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            if (base.DeserializeValue(reader, line))
            {
                int idx = Value?.IndexOf(';') ?? -1;
                if (idx >= 0)
                {
                    MinVersion = Value.Substring(0, idx);
                    Value = Value.Substring(idx + 1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string(VersionProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator VersionProperty(string value) { return value != null ? new VersionProperty { Value = value } : null; }

        /// <summary>
        /// Min version
        /// </summary>
        public string MinVersion { get; set; }

    }
}
