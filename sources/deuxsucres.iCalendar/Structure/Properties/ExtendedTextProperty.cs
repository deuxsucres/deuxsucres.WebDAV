using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Structure
{
    /// <summary>
    /// Extended text property
    /// </summary>
    public class ExtendedTextProperty : TextProperty
    {

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string(ExtendedTextProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator ExtendedTextProperty(string value) { return value != null ? new ExtendedTextProperty { Value = value } : null; }

        /// <summary>
        /// Altrep
        /// </summary>
        public TextParameter AltRep
        {
            get { return FindParameter<TextParameter>(Constants.ALTREP); }
            set { SetParameter(value, Constants.ALTREP); }
        }

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
