using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// FREEBUSY property
    /// </summary>
    public class FreeBusyProperty : CalProperty
    {
        /// <summary>
        /// New property
        /// </summary>
        public FreeBusyProperty()
        {
            Name = Constants.FREEBUSY;
        }

        #region Serialization


        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return writer.Parser.EncodeList(Periods, p => writer.Parser.EncodePeriod(p));
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            Periods = reader.Parser.ParseList(line.Value, v => reader.Parser.ParsePeriod(v)).ToArray();
            return Periods != null;
        }

        #endregion

        /// <summary>
        /// List of periods
        /// </summary>
        public Period[] Periods { get; set; }

        /// <summary>
        /// Type of free
        /// </summary>
        public EnumParameter<FreeBusyTypes> FreeBusyType
        {
            get { return FindParameter<EnumParameter<FreeBusyTypes>>(Constants.FBTYPE); }
            set { SetParameter(value, Constants.FBTYPE); }
        }

    }
}
