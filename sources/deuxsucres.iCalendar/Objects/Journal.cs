using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Journal entry
    /// </summary>
    public class Journal : CalComponent
    {

        /// <summary>
        /// Create a journal entry
        /// </summary>
        public Journal()
        {
            //Attachments = new CalProperties<AttachProperty>(Constants.ATTACH, this);
            //Attendees = new CalProperties<JournalAttendeeProperty>(Constants.ATTENDEE, this);
            //Categories = new CalProperties<CategoriesProperty>(Constants.CATEGORIES, this);
            Comments = new CalProperties<CommentProperty>(Constants.COMMENT, this);
            Contacts = new CalProperties<ExtendedTextProperty>(Constants.CONTACT, this);
            //Exdates = new CalProperties<ExdateProperty>(Constants.EXDATE, this);
            //Exrules = new CalProperties<ExruleProperty>(Constants.EXRULE, this);
            //RequestStatuses = new CalProperties<RequestStatusProperty>(Constants.REQUEST_STATUS, this);
            //RelatedTos = new CalProperties<RelatedToProperty>(Constants.RELATED_TO, this);
            //Resources = new CalProperties<ResourcesProperty>(Constants.RESOURCES, this);
            RecurDates = new CalProperties<RecurDateProperty>(Constants.RDATE, this);
            RecurRules = new CalProperties<RecurRuleProperty>(Constants.RRULE, this);
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
                case Constants.DTSTART: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.DTSTART); return true;
                case Constants.CLASS: SetProperty(reader.MakeProperty<EnumProperty<Classes>>(line), Constants.CLASS); return true;
                case Constants.CREATED: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.CREATED); return true;
                case Constants.DESCRIPTION: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.DESCRIPTION); return true;
                case Constants.LAST_MODIFIED: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.LAST_MODIFIED); return true;
                case Constants.ORGANIZER: SetProperty(reader.MakeProperty<OrganizerProperty>(line), Constants.ORGANIZER); return true;
                case Constants.RECURRENCE_ID: SetProperty(reader.MakeProperty<TypedDateTimeProperty>(line), Constants.RECURRENCE_ID); return true;
                case Constants.SEQUENCE: SetProperty(reader.MakeProperty<IntegerProperty>(line), Constants.SEQUENCE); return true;
                case Constants.STATUS: SetProperty(reader.MakeProperty<EnumProperty<JournalStatuses>>(line), Constants.STATUS); return true;
                case Constants.SUMMARY: SetProperty(reader.MakeProperty<TextProperty>(line), Constants.SUMMARY); return true;
                case Constants.URL: SetProperty(reader.MakeProperty<UriProperty>(line), Constants.URL); return true;

                //case Constants.ATTACH: AddProperty(reader.MakeProperty<AttachProperty>(line)); return true;
                //case Constants.ATTENDEE: AddProperty(reader.MakeProperty<JournalAttendeeProperty>(line)); return true;
                //case Constants.CATEGORIES: AddProperty(reader.MakeProperty<CategoriesProperty>(line)); return true;
                case Constants.COMMENT: AddProperty(reader.MakeProperty<CommentProperty>(line)); return true;
                case Constants.CONTACT: AddProperty(reader.MakeProperty<ExtendedTextProperty>(line)); return true;
                //case Constants.EXDATE: AddProperty(reader.MakeProperty<ExdateProperty>(line)); return true;
                //case Constants.EXRULE: AddProperty(reader.MakeProperty<ExruleProperty>(line)); return true;
                //case Constants.REQUEST_STATUS: AddProperty(reader.MakeProperty<RequestStatusProperty>(line)); return true;
                //case Constants.RELATED_TO: AddProperty(reader.MakeProperty<RelatedToProperty>(line)); return true;
                //case Constants.RESOURCES: AddProperty(reader.MakeProperty<ResourcesProperty>(line)); return true;
                case Constants.RDATE: AddProperty(reader.MakeProperty<RecurDateProperty>(line)); return true;
                case Constants.RRULE: AddProperty(reader.MakeProperty<RecurRuleProperty>(line)); return true;
                default: return false;
            }
        }

        /// <summary>
        /// Name of the object
        /// </summary>
        public override string Name => Constants.VJOURNAL;

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
        public DateTimeProperty DateStart
        {
            get { return FindProperty<DateTimeProperty>(Constants.DTSTART); }
            set { SetProperty(value, Constants.DTSTART); }
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
        /// SUMMARY
        /// </summary>
        public TextProperty Summary
        {
            get { return FindProperty<TextProperty>(Constants.SUMMARY); }
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
        /// CREATED
        /// </summary>
        public DateTimeProperty Created
        {
            get { return FindProperty<DateTimeProperty>(Constants.CREATED); }
            set { SetProperty(value, Constants.CREATED); }
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
        public EnumProperty<JournalStatuses> Status
        {
            get { return FindProperty<EnumProperty<JournalStatuses>>(Constants.STATUS); }
            set { SetProperty(value, Constants.STATUS); }
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

        ///// <summary>
        ///// List of attachments
        ///// </summary>
        //public CalProperties<AttachProperty> Attachments { get; private set; }

        ///// <summary>
        ///// List of attendees
        ///// </summary>
        //public CalProperties<JournalAttendeeProperty> Attendees { get; private set; }

        ///// <summary>
        ///// List of categories
        ///// </summary>
        //public CalProperties<CategoriesProperty> Categories { get; private set; }

        /// <summary>
        /// List of comments
        /// </summary>
        public CalProperties<CommentProperty> Comments { get; private set; }

        /// <summary>
        /// List of contacts
        /// </summary>
        public CalProperties<ExtendedTextProperty> Contacts { get; private set; }

        ///// <summary>
        ///// List of exception dates
        ///// </summary>
        //public CalProperties<ExdateProperty> Exdates { get; private set; }

        ///// <summary>
        ///// List of rules exceptions
        ///// </summary>
        //public CalProperties<ExruleProperty> Exrules { get; private set; }

        ///// <summary>
        ///// List of rules exceptions
        ///// </summary>
        //public CalProperties<RequestStatusProperty> RequestStatuses { get; private set; }

        ///// <summary>
        ///// List of related tos
        ///// </summary>
        //public CalProperties<RelatedToProperty> RelatedTos { get; private set; }

        ///// <summary>
        ///// List of resources
        ///// </summary>
        //public CalProperties<ResourcesProperty> Resources { get; private set; }

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
