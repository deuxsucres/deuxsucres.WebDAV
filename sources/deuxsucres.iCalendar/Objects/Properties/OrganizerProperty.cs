using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// ORGANIZER
    /// </summary>
    public class OrganizerProperty : CalAddressProperty
    {
        /// <summary>
        /// New property
        /// </summary>
        public OrganizerProperty()
        {
            Name = Constants.ORGANIZER;
        }

        /// <summary>
        /// Cast to uri
        /// </summary>
        public static implicit operator Uri(OrganizerProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast from uri
        /// </summary>
        public static implicit operator OrganizerProperty(Uri value) { return value != null ? new OrganizerProperty { Value = value } : null; }

        /// <summary>
        /// Cast to string
        /// </summary>
        public static implicit operator string(OrganizerProperty prop) { return prop?.Value?.ToString(); }

        /// <summary>
        /// Cast from string
        /// </summary>
        public static implicit operator OrganizerProperty(string value) { return value != null ? new OrganizerProperty { Value = new Uri(value) } : null; }

        /// <summary>
        /// CN
        /// </summary>
        public TextParameter CN
        {
            get { return FindParameter<TextParameter>(Constants.CN); }
            set { SetParameter(value, Constants.CN); }
        }

        /// <summary>
        /// Dir
        /// </summary>
        public UriParameter Dir
        {
            get { return FindParameter<UriParameter>(Constants.DIR); }
            set { SetParameter(value, Constants.DIR); }
        }

        /// <summary>
        /// Sent by
        /// </summary>
        public CalAddressParameter SentBy
        {
            get { return FindParameter<CalAddressParameter>(Constants.SENT_BY); }
            set { SetParameter(value, Constants.SENT_BY); }
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
