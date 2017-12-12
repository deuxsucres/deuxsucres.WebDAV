using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Some constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Default prod ID
        /// </summary>
        public const string DefaultProdID = "-//deuxsucres//deuxsucres.iCalendar v1.0//EN";

        /// <summary>
        /// BEGIN
        /// </summary>
        public const string BEGIN = "BEGIN";

        /// <summary>
        /// END
        /// </summary>
        public const string END = "END";

        #region Objects

        /// <summary>
        /// VCALENDAR
        /// </summary>
        public const string VCALENDAR = "VCALENDAR";

        /// <summary>
        /// VEVENT
        /// </summary>
        public const string VEVENT = "VEVENT";

        /// <summary>
        /// VTODO
        /// </summary>
        public const string VTODO = "VTODO";

        /// <summary>
        /// VALARM
        /// </summary>
        public const string VALARM = "VALARM";

        /// <summary>
        /// VJOURNAL
        /// </summary>
        public const string VJOURNAL = "VJOURNAL";

        /// <summary>
        /// VFREEBUSY
        /// </summary>
        public const string VFREEBUSY = "VFREEBUSY";

        /// <summary>
        /// VTIMEZONE
        /// </summary>
        public const string VTIMEZONE = "VTIMEZONE";

        #endregion

        /// <summary>
        /// GREGORIAN
        /// </summary>
        public const string GREGORIAN = "GREGORIAN";

        /// <summary>
        /// STANDARD
        /// </summary>
        public const string STANDARD = "STANDARD";

        /// <summary>
        /// DAYLIGHT
        /// </summary>
        public const string DAYLIGHT = "DAYLIGHT";

        #region Properties

        /// <summary>
        /// ATTACH
        /// </summary>
        public const string ATTACH = "ATTACH";

        /// <summary>
        /// ATTENDEE
        /// </summary>
        public const string ATTENDEE = "ATTENDEE";

        /// <summary>
        /// CATEGORIES
        /// </summary>
        public const string CATEGORIES = "CATEGORIES";

        /// <summary>
        /// COMMENT
        /// </summary>
        public const string COMMENT = "COMMENT";

        /// <summary>
        /// PRODID
        /// </summary>
        public const string PRODID = "PRODID";

        /// <summary>
        /// VERSION
        /// </summary>
        public const string VERSION = "VERSION";

        /// <summary>
        /// CALSCALE
        /// </summary>
        public const string CALSCALE = "CALSCALE";

        /// <summary>
        /// ALARM
        /// </summary>
        public const string ALARM = "ALARM";

        /// <summary>
        /// CLASS
        /// </summary>
        public const string CLASS = "CLASS";

        /// <summary>
        /// COMPLETED
        /// </summary>
        public const string COMPLETED = "COMPLETED";

        /// <summary>
        /// CONTACT
        /// </summary>
        public const string CONTACT = "CONTACT";

        /// <summary>
        /// CREATED
        /// </summary>
        public const string CREATED = "CREATED";

        /// <summary>
        /// DESCRIPTION
        /// </summary>
        public const string DESCRIPTION = "DESCRIPTION";

        /// <summary>
        /// DTSTAMP
        /// </summary>
        public const string DTSTAMP = "DTSTAMP";

        /// <summary>
        /// DTSTART
        /// </summary>
        public const string DTSTART = "DTSTART";

        /// <summary>
        /// DUE
        /// </summary>
        public const string DUE = "DUE";

        /// <summary>
        /// DTEND
        /// </summary>
        public const string DTEND = "DTEND";

        /// <summary>
        /// DURATION
        /// </summary>
        public const string DURATION = "DURATION";

        /// <summary>
        /// EXDATE
        /// </summary>
        public const string EXDATE = "EXDATE";

        /// <summary>
        /// EXRULE
        /// </summary>
        public const string EXRULE = "EXRULE";

        /// <summary>
        /// FREEBUSY
        /// </summary>
        public const string FREEBUSY = "FREEBUSY";

        /// <summary>
        /// GEO
        /// </summary>
        public const string GEO = "GEO";

        /// <summary>
        /// LAST-MODIFIED
        /// </summary>
        public const string LAST_MODIFIED = "LAST-MODIFIED";

        /// <summary>
        /// LOCATION
        /// </summary>
        public const string LOCATION = "LOCATION";

        /// <summary>
        /// ORGANIZER
        /// </summary>
        public const string ORGANIZER = "ORGANIZER";

        /// <summary>
        /// METHOD
        /// </summary>
        public const string METHOD = "METHOD";

        /// <summary>
        /// PERCENT-COMPLETE
        /// </summary>
        public const string PERCENT_COMPLETE = "PERCENT-COMPLETE";

        /// <summary>
        /// PRIORITY
        /// </summary>
        public const string PRIORITY = "PRIORITY";

        /// <summary>
        /// RECURRENCE-ID
        /// </summary>
        public const string RECURRENCE_ID = "RECURRENCE-ID";

        /// <summary>
        /// RELATED  
        /// </summary>
        public const string RELATED = "RELATED";

        /// <summary>
        /// RELATED-TO
        /// </summary>
        public const string RELATED_TO = "RELATED-TO";

        /// <summary>
        /// REQUEST-STATUS
        /// </summary>
        public const string REQUEST_STATUS = "REQUEST-STATUS";

        /// <summary>
        /// RESOURCES 
        /// </summary>
        public const string RESOURCES = "RESOURCES";

        /// <summary>
        /// RDATE 
        /// </summary>
        public const string RDATE = "RDATE";

        /// <summary>
        /// RRULE 
        /// </summary>
        public const string RRULE = "RRULE";

        /// <summary>
        /// SEQUENCE
        /// </summary>
        public const string SEQUENCE = "SEQUENCE";

        /// <summary>
        /// STATUS
        /// </summary>
        public const string STATUS = "STATUS";

        /// <summary>
        /// SUMMARY
        /// </summary>
        public const string SUMMARY = "SUMMARY";

        /// <summary>
        /// TRANSP
        /// </summary>
        public const string TRANSP = "TRANSP";

        /// <summary>
        /// UID
        /// </summary>
        public const string UID = "UID";

        /// <summary>
        /// URL
        /// </summary>
        public const string URL = "URL";

        /// <summary>
        /// TZNAME
        /// </summary>
        public const string TZNAME = "TZNAME";

        /// <summary>
        /// TZOFFSETFROM
        /// </summary>
        public const string TZOFFSETFROM = "TZOFFSETFROM";

        /// <summary>
        /// TZOFFSETTO
        /// </summary>
        public const string TZOFFSETTO = "TZOFFSETTO";

        /// <summary>
        /// TZURL
        /// </summary>
        public const string TZURL = "TZURL";

        #endregion

        #region Parameters

        /// <summary>
        /// ACTION
        /// </summary>
        public const string ACTION = "ACTION";

        /// <summary>
        /// ALTREP
        /// </summary>
        public const string ALTREP = "ALTREP";

        /// <summary>
        /// CN
        /// </summary>
        public const string CN = "CN";

        /// <summary>
        /// CUTYPE
        /// </summary>
        public const string CUTYPE = "CUTYPE";

        /// <summary>
        /// DELEGATED-FROM
        /// </summary>
        public const string DELEGATED_FROM = "DELEGATED-FROM";

        /// <summary>
        /// DELEGATED-TO
        /// </summary>
        public const string DELEGATED_TO = "DELEGATED-TO";

        /// <summary>
        /// DIR
        /// </summary>
        public const string DIR = "DIR";

        /// <summary>
        /// ENCODING
        /// </summary>
        public const string ENCODING = "ENCODING";

        /// <summary>
        /// FMTTYPE
        /// </summary>
        public const string FMTTYPE = "FMTTYPE";

        /// <summary>
        /// FBTYPE
        /// </summary>
        public const string FBTYPE = "FBTYPE";

        /// <summary>
        /// LANGUAGE
        /// </summary>
        public const string LANGUAGE = "LANGUAGE";

        /// <summary>
        /// MEMBER
        /// </summary>
        public const string MEMBER = "MEMBER";

        /// <summary>
        /// PARTSTAT
        /// </summary>
        public const string PARTSTAT = "PARTSTAT";

        /// <summary>
        /// RANGE
        /// </summary>
        public const string RANGE = "RANGE";

        ///// <summary>
        ///// RELATED
        ///// </summary>
        //public const string RELATED = "RELATED";

        /// <summary>
        /// RELTYPE
        /// </summary>
        public const string RELTYPE = "RELTYPE";

        /// <summary>
        /// REPEAT
        /// </summary>
        public const string REPEAT = "REPEAT";

        /// <summary>
        /// ROLE
        /// </summary>
        public const string ROLE = "ROLE";

        /// <summary>
        /// RSVP
        /// </summary>
        public const string RSVP = "RSVP";

        /// <summary>
        /// SENT-BY
        /// </summary>
        public const string SENT_BY = "SENT-BY";

        /// <summary>
        /// TZID
        /// </summary>
        public const string TZID = "TZID";

        /// <summary>
        /// TRIGGER
        /// </summary>
        public const string TRIGGER = "TRIGGER";

        /// <summary>
        /// VALUE
        /// </summary>
        public const string VALUE = "VALUE";

        #endregion

    }
}
