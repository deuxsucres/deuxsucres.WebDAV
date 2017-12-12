using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Event
    /// </summary>
    public class Event : CalComponent
    {
        /// <summary>
        /// Create an event
        /// </summary>
        public Event()
        {
            Attachments = new CalProperties<AttachProperty>(Constants.ATTACH, this);
            Attendees = new CalProperties<EventAttendeeProperty>(Constants.ATTENDEE, this);
            Categories = new CalProperties<CategoriesProperty>(Constants.CATEGORIES, this);
            Comments = new CalProperties<CommentProperty>(Constants.COMMENT, this);
            Contacts = new CalProperties<ExtendedTextProperty>(Constants.CONTACT, this);
            Exdates = new CalProperties<TypedDateTimeListProperty>(Constants.EXDATE, this);
            Exrules = new CalProperties<RecurrenceProperty>(Constants.EXRULE, this);
            RequestStatuses = new CalProperties<RequestStatusProperty>(Constants.REQUEST_STATUS, this);
            RelatedTos = new CalProperties<RelatedToProperty>(Constants.RELATED_TO, this);
            Resources = new CalProperties<ExtendedTextProperty>(Constants.RESOURCES, this);
            RecurDates = new CalProperties<RecurDateProperty>(Constants.RDATE, this);
            RecurRules = new CalProperties<RecurRuleProperty>(Constants.RRULE, this);
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Alarms.Clear();
        }

        /// <summary>
        /// Process the properties
        /// </summary>
        protected override bool ProcessProperty(ICalReader reader, ContentLine line)
        {
            switch (line.Name.ToUpper())
            {
                case Constants.UID: SetProperty(reader.MakeProperty<TextProperty>(line), Constants.UID); return true;
                case Constants.DTSTAMP: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.DTSTAMP); return true;
                case Constants.DTSTART: SetProperty(reader.MakeProperty<TypedDateTimeProperty>(line), Constants.DTSTART); return true;
                case Constants.DTEND: SetProperty(reader.MakeProperty<TypedDateTimeProperty>(line), Constants.DTEND); return true;
                case Constants.DURATION: SetProperty(reader.MakeProperty<DurationProperty>(line), Constants.DURATION); return true;
                case Constants.SUMMARY: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.SUMMARY); return true;
                case Constants.CLASS: SetProperty(reader.MakeProperty<EnumProperty<Classes>>(line), Constants.CLASS); return true;
                case Constants.CREATED: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.CREATED); return true;
                case Constants.DESCRIPTION: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.DESCRIPTION); return true;
                case Constants.LOCATION: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.LOCATION); return true;
                case Constants.PRIORITY: SetProperty(reader.MakeProperty<IntegerProperty>(line), Constants.PRIORITY); return true;
                case Constants.GEO: SetProperty(reader.MakeProperty<GeoPositionProperty>(line), Constants.GEO); return true;
                case Constants.LAST_MODIFIED: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.LAST_MODIFIED); return true;
                case Constants.ORGANIZER: SetProperty(reader.MakeProperty<OrganizerProperty>(line), Constants.ORGANIZER); return true;
                case Constants.SEQUENCE: SetProperty(reader.MakeProperty<IntegerProperty>(line), Constants.SEQUENCE); return true;
                case Constants.STATUS: SetProperty(reader.MakeProperty<EnumProperty<EventStatuses>>(line), Constants.STATUS); return true;
                case Constants.TRANSP: SetProperty(reader.MakeProperty<EnumProperty<TransparentStates>>(line), Constants.TRANSP); return true;
                case Constants.URL: SetProperty(reader.MakeProperty<UriProperty>(line), Constants.URL); return true;
                case Constants.RECURRENCE_ID: SetProperty(reader.MakeProperty<TypedDateTimeProperty>(line), Constants.RECURRENCE_ID); return true;

                case Constants.ATTACH: AddProperty(reader.MakeProperty<AttachProperty>(line)); return true;
                case Constants.ATTENDEE: AddProperty(reader.MakeProperty<EventAttendeeProperty>(line)); return true;
                case Constants.CATEGORIES: AddProperty(reader.MakeProperty<CategoriesProperty>(line)); return true;
                case Constants.COMMENT: AddProperty(reader.MakeProperty<CommentProperty>(line)); return true;
                case Constants.CONTACT: AddProperty(reader.MakeProperty<ExtendedTextProperty>(line)); return true;
                case Constants.EXDATE: AddProperty(reader.MakeProperty<TypedDateTimeListProperty>(line)); return true;
                case Constants.EXRULE: AddProperty(reader.MakeProperty<RecurrenceProperty>(line)); return true;
                case Constants.REQUEST_STATUS: AddProperty(reader.MakeProperty<RequestStatusProperty>(line)); return true;
                case Constants.RELATED_TO: AddProperty(reader.MakeProperty<RelatedToProperty>(line)); return true;
                case Constants.RESOURCES: AddProperty(reader.MakeProperty<ExtendedTextProperty>(line)); return true;
                case Constants.RDATE: AddProperty(reader.MakeProperty<RecurDateProperty>(line)); return true;
                case Constants.RRULE: AddProperty(reader.MakeProperty<RecurRuleProperty>(line)); return true;
                default: return false;
            }
        }

        /// <summary>
        /// Read a component
        /// </summary>
        protected override CalComponent ReadComponent(ICalReader reader, ContentLine line)
        {
            if (line.Value.IsEqual(Constants.VALARM))
                return reader.ReadComponent<EventAlarm>(line);
            return base.ReadComponent(reader, line);
        }

        /// <summary>
        /// Process the components
        /// </summary>
        protected override bool ProcessComponent(CalComponent component)
        {
            if (component is EventAlarm)
            {
                Alarms.Add((EventAlarm)component);
                return true;
            }
            return base.ProcessComponent(component);
        }

        /// <summary>
        /// Serialize alarms
        /// </summary>
        protected override void SerializeComponents(ICalWriter writer)
        {
            base.SerializeComponents(writer);
            foreach (var alarm in Alarms)
                alarm.Serialize(writer);
        }

        /// <summary>
        /// Name of the object
        /// </summary>
        public override string Name => Constants.VEVENT;

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
        /// Represents a full day event
        /// </summary>
        /// <remarks>
        /// DTSTART is a DATE
        /// </remarks>
        public bool IsAllDay
        {
            get { return DateStart?.IsDate ?? false; }
            set
            {
                if (value)
                    DateStart?.SetAsDate();
                else
                    DateStart?.SetAsDateTime();
            }
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
        /// SUMMARY
        /// </summary>
        public ExtendedTextProperty Summary
        {
            get { return FindProperty<ExtendedTextProperty>(Constants.SUMMARY); }
            set { SetProperty(value, Constants.SUMMARY); }
        }

        /// <summary>
        /// DESCRIPTION
        /// </summary>
        public ExtendedTextProperty Description
        {
            get { return FindProperty<ExtendedTextProperty>(Constants.DESCRIPTION); }
            set { SetProperty(value, Constants.DESCRIPTION); }
        }

        /// <summary>
        /// LOCATION
        /// </summary>
        public ExtendedTextProperty Location
        {
            get { return FindProperty<ExtendedTextProperty>(Constants.LOCATION); }
            set { SetProperty(value, Constants.LOCATION); }
        }

        /// <summary>
        /// Class
        /// </summary>
        public EnumProperty<Classes> Class
        {
            get { return FindProperty<EnumProperty<Classes>>(Constants.CLASS); }
            set { SetProperty(value, Constants.CLASS); }
        }

        /// <summary>
        /// CREATED
        /// </summary>
        public DateTimeProperty Created
        {
            get { return FindProperty<DateTimeProperty>(Constants.CREATED); }
            set { SetProperty(value, Constants.CREATED); }
        }

        /// <summary>
        /// PRIORITY
        /// </summary>
        public IntegerProperty Priority
        {
            get { return FindProperty<IntegerProperty>(Constants.PRIORITY); }
            set { SetProperty(value, Constants.PRIORITY); }
        }

        /// <summary>
        /// GEO
        /// </summary>
        public GeoPositionProperty Geo
        {
            get { return FindProperty<GeoPositionProperty>(Constants.GEO); }
            set { SetProperty(value, Constants.GEO); }
        }

        /// <summary>
        /// LAST-MODIFIED
        /// </summary>
        public DateTimeProperty LastModified
        {
            get { return FindProperty<DateTimeProperty>(Constants.LAST_MODIFIED); }
            set { SetProperty(value, Constants.LAST_MODIFIED); }
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
        /// SEQUENCE
        /// </summary>
        public IntegerProperty Sequence
        {
            get { return FindProperty<IntegerProperty>(Constants.SEQUENCE); }
            set { SetProperty(value, Constants.SEQUENCE); }
        }

        /// <summary>
        /// STATUS
        /// </summary>
        public EnumProperty<EventStatuses> Status
        {
            get { return FindProperty<EnumProperty<EventStatuses>>(Constants.STATUS); }
            set { SetProperty(value, Constants.STATUS); }
        }

        /// <summary>
        /// TRANSP
        /// </summary>
        public EnumProperty<TransparentStates> Transp
        {
            get { return FindProperty<EnumProperty<TransparentStates>>(Constants.TRANSP); }
            set { SetProperty(value, Constants.TRANSP); }
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
        /// RECURRENCE-ID
        /// </summary>
        public TypedDateTimeProperty RecurrenceId
        {
            get { return FindProperty<TypedDateTimeProperty>(Constants.RECURRENCE_ID); }
            set { SetProperty(value, Constants.RECURRENCE_ID); }
        }

        /// <summary>
        /// Alarms
        /// </summary>
        public List<EventAlarm> Alarms { get; private set; } = new List<EventAlarm>();

        /// <summary>
        /// List of attachments
        /// </summary>
        public CalProperties<AttachProperty> Attachments { get; private set; }

        /// <summary>
        /// List of attendees
        /// </summary>
        public CalProperties<EventAttendeeProperty> Attendees { get; private set; }

        /// <summary>
        /// List of categories
        /// </summary>
        public CalProperties<CategoriesProperty> Categories { get; private set; }

        /// <summary>
        /// List of comments
        /// </summary>
        public CalProperties<CommentProperty> Comments { get; private set; }

        /// <summary>
        /// List of contacts
        /// </summary>
        public CalProperties<ExtendedTextProperty> Contacts { get; private set; }

        /// <summary>
        /// List of exception dates
        /// </summary>
        public CalProperties<TypedDateTimeListProperty> Exdates { get; private set; }

        /// <summary>
        /// List of rules exceptions
        /// </summary>
        public CalProperties<RecurrenceProperty> Exrules { get; private set; }

        /// <summary>
        /// List of rules exceptions
        /// </summary>
        public CalProperties<RequestStatusProperty> RequestStatuses { get; private set; }

        /// <summary>
        /// List of related tos
        /// </summary>
        public CalProperties<RelatedToProperty> RelatedTos { get; private set; }

        /// <summary>
        /// List of resources
        /// </summary>
        public CalProperties<ExtendedTextProperty> Resources { get; private set; }

        /// <summary>
        /// List of recurrence dates
        /// </summary>
        public CalProperties<RecurDateProperty> RecurDates { get; private set; }

        /// <summary>
        /// List of recurrence rules
        /// </summary>
        public CalProperties<RecurRuleProperty> RecurRules { get; private set; }

    }
}
