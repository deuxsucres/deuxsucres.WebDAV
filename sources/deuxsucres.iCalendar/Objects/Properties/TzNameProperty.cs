using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// TZNAME
    /// </summary>
    public class TzNameProperty : TextProperty
    {

        /// <summary>
        /// Create a new property
        /// </summary>
        public TzNameProperty()
        {
            Name = Constants.TZNAME;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string(TzNameProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator TzNameProperty(string value) { return value != null ? new TzNameProperty { Value = value } : null; }

        /// <summary>
        /// Language
        /// </summary>
        public TextParameter Language
        {
            get { return FindParameter<TextParameter>(Constants.LANGUAGE); }
            set { SetParameter(value, Constants.LANGUAGE); }
        }

    }
}
