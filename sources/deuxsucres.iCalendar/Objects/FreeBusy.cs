using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Free/Busy
    /// </summary>
    public class FreeBusy : CalComponent
    {

        /// <summary>
        /// Create a new component
        /// </summary>
        public FreeBusy()
        {
            Attendees = new CalProperties<FreeBusyAttendeeProperty>(Constants.ATTENDEE, this);
            Comments = new CalProperties<CommentProperty>(Constants.COMMENT, this);
            FreeBusies = new CalProperties<FreeBusyProperty>(Constants.FREEBUSY, this);
            RequestStatuses = new CalProperties<RequestStatusProperty>(Constants.REQUEST_STATUS, this);
        }

        /// <summary>
        /// Process the properties
        /// </summary>
        protected override bool ProcessProperty(ICalReader reader, ContentLine line)
        {
            switch (line.Name.ToUpper())
            {
                case Constants.UID: SetProperty(reader.MakeProperty<TextProperty>(line), Constants.UID); return true;
                case Constants.CONTACT: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.CONTACT); return true;
                case Constants.DTSTART: SetProperty(reader.MakeProperty<TypedDateTimeProperty>(line), Constants.DTSTART); return true;
                case Constants.DTEND: SetProperty(reader.MakeProperty<TypedDateTimeProperty>(line), Constants.DTEND); return true;
                case Constants.DURATION: SetProperty(reader.MakeProperty<DurationProperty>(line), Constants.DURATION); return true;
                case Constants.DTSTAMP: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.DTSTAMP); return true;
                case Constants.ORGANIZER: SetProperty(reader.MakeProperty<OrganizerProperty>(line), Constants.ORGANIZER); return true;
                case Constants.URL: SetProperty(reader.MakeProperty<UriProperty>(line), Constants.URL); return true;

                case Constants.ATTENDEE: AddProperty(reader.MakeProperty<FreeBusyAttendeeProperty>(line)); return true;
                case Constants.FREEBUSY: AddProperty(reader.MakeProperty<FreeBusyProperty>(line)); return true;
                case Constants.COMMENT: AddProperty(reader.MakeProperty<CommentProperty>(line)); return true;
                case Constants.REQUEST_STATUS: AddProperty(reader.MakeProperty<RequestStatusProperty>(line)); return true;
                default: return false;
            }
        }

        /// <summary>
        /// Name of the object
        /// </summary>
        public override string Name => Constants.VFREEBUSY;

        /// <summary>
        /// UID
        /// </summary>
        public TextProperty UID
        {
            get { return FindProperty<TextProperty>(Constants.UID); }
            set { SetProperty(value, Constants.UID); }
        }

        /// <summary>
        /// DTSTAMP
        /// </summary>
        public DateTimeProperty DtStamp
        {
            get { return FindProperty<DateTimeProperty>(Constants.DTSTAMP); }
            set { SetProperty(value, Constants.DTSTAMP); }
        }

        /// <summary>
        /// DTSTART
        /// </summary>
        public TypedDateTimeProperty DateStart
        {
            get { return FindProperty<TypedDateTimeProperty>(Constants.DTSTART); }
            set { SetProperty(value, Constants.DTSTART); }
        }

        /// <summary>
        /// DTEND
        /// </summary>
        public TypedDateTimeProperty DateEnd
        {
            get { return FindProperty<TypedDateTimeProperty>(Constants.DTEND); }
            set
            {
                SetProperty(value, Constants.DTEND);
                if (value != null)
                    Duration = null;
            }
        }

        /// <summary>
        /// DURATION
        /// </summary>
        public DurationProperty Duration
        {
            get { return FindProperty<DurationProperty>(Constants.DURATION); }
            set
            {
                SetProperty(value, Constants.DURATION);
                if (value != null)
                    DateEnd = null;
            }
        }

        /// <summary>
        /// Contact
        /// </summary>
        public ExtendedTextProperty Contact
        {
            get { return FindProperty<ExtendedTextProperty>(Constants.CONTACT); }
            set { SetProperty(value, Constants.CONTACT); }
        }

        /// <summary>
        /// ORGANIZER
        /// </summary>
        public OrganizerProperty Organizer
        {
            get { return FindProperty<OrganizerProperty>(Constants.ORGANIZER); }
            set { SetProperty(value, Constants.ORGANIZER); }
        }

        /// <summary>
        /// URL
        /// </summary>
        public UriProperty Url
        {
            get { return FindProperty<UriProperty>(Constants.URL); }
            set { SetProperty(value, Constants.URL); }
        }

        /// <summary>
        /// List of attendees
        /// </summary>
        public CalProperties<FreeBusyAttendeeProperty> Attendees { get; private set; }

        /// <summary>
        /// List of comments
        /// </summary>
        public CalProperties<CommentProperty> Comments { get; private set; }

        /// <summary>
        /// List of free/busy
        /// </summary>
        public CalProperties<FreeBusyProperty> FreeBusies { get; private set; }

        /// <summary>
        /// List of rules exceptions
        /// </summary>
        public CalProperties<RequestStatusProperty> RequestStatuses { get; private set; }

    }
}
