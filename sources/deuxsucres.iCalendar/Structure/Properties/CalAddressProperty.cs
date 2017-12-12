using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{

    /// <summary>
    /// CAL-ADDRESS value property
    /// </summary>
    public class CalAddressProperty : CalProperty
    {

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Value = null;
        }

        #region Serialization

        /// <summary>
        /// Serialize a value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeCalAddress(Value, false);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            Value = reader.Parser.ParseUri(line.Value, false);
            return Value != null;
        }

        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Value?.ToString();
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator Uri(CalAddressProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator CalAddressProperty(Uri value) { return value != null ? new CalAddressProperty { Value = value } : null; }

        /// <summary>
        /// Cast to string
        /// </summary>
        public static implicit operator string(CalAddressProperty param) => param?.Value?.ToString();

        /// <summary>
        /// Cast from string
        /// </summary>
        public static implicit operator CalAddressProperty(string str) => str != null ? new CalAddressProperty { Value = new Uri(str) } : null;

        /// <summary>
        /// Uri value
        /// </summary>
        public Uri Value { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public EnumParameter<Roles> Role
        {
            get { return FindParameter<EnumParameter<Roles>>(Constants.ROLE); }
            set { SetParameter(value, Constants.ROLE); }
        }
    }

}