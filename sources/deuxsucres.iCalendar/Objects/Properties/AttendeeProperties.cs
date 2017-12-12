using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// ATTENDEE
    /// </summary>
    public abstract class AttendeeProperty<T> : CalAddressProperty where T : struct
    {
        /// <summary>
        /// New property
        /// </summary>
        public AttendeeProperty()
        {
            Name = Constants.ATTENDEE;
        }

        /// <summary>
        /// Deserialize parameter
        /// </summary>
        protected override ICalPropertyParameter DeserializeParameter(ICalReader reader, ContentLine line, string name, string value)
        {
            if (name.IsEqual(Constants.PARTSTAT))
                return reader.MakePropertyParameter<EnumParameter<T>>(name, value);
            return base.DeserializeParameter(reader, line, name, value);
        }

        /// <summary>
        /// Type of user
        /// </summary>
        public EnumParameter<CalUserTypes> CuType
        {
            get { return FindParameter<EnumParameter<CalUserTypes>>(Constants.CUTYPE); }
            set { SetParameter(value, Constants.CUTYPE); }
        }

        /// <summary>
        /// Member of
        /// </summary>
        public CalAddressesParameter Member
        {
            get { return FindParameter<CalAddressesParameter>(Constants.MEMBER); }
            set { SetParameter(value, Constants.MEMBER); }
        }

        /// <summary>
        /// Participation
        /// </summary>
        public EnumParameter<T> PartStat
        {
            get { return FindParameter<EnumParameter<T>>(Constants.PARTSTAT); }
            set { SetParameter(value, Constants.PARTSTAT); }
        }

        /// <summary>
        /// RSVP
        /// </summary>
        public BooleanParameter Rsvp
        {
            get { return FindParameter<BooleanParameter>(Constants.RSVP); }
            set { SetParameter(value, Constants.RSVP); }
        }

        /// <summary>
        /// Delegated From
        /// </summary>
        public CalAddressesParameter DelegatedFrom
        {
            get { return FindParameter<CalAddressesParameter>(Constants.DELEGATED_FROM); }
            set { SetParameter(value, Constants.DELEGATED_FROM); }
        }

        /// <summary>
        /// Delegated to
        /// </summary>
        public CalAddressesParameter DelegatedTo
        {
            get { return FindParameter<CalAddressesParameter>(Constants.DELEGATED_TO); }
            set { SetParameter(value, Constants.DELEGATED_TO); }
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
        /// Language
        /// </summary>
        public TextParameter Language
        {
            get { return FindParameter<TextParameter>(Constants.LANGUAGE); }
            set { SetParameter(value, Constants.LANGUAGE); }
        }
    }

    /// <summary>
    /// ATTENDEE for journal
    /// </summary>
    public class JournalAttendeeProperty : AttendeeProperty<JournalPartStatuses>
    {
        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator Uri(JournalAttendeeProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator JournalAttendeeProperty(Uri value) { return value != null ? new JournalAttendeeProperty { Value = value } : null; }
    }

    /// <summary>
    /// ATTENDEE for event
    /// </summary>
    public class EventAttendeeProperty : AttendeeProperty<EventPartStatuses>
    {
        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator Uri(EventAttendeeProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator EventAttendeeProperty(Uri value) { return value != null ? new EventAttendeeProperty { Value = value } : null; }
    }

    /// <summary>
    /// ATTENDEE for todo
    /// </summary>
    public class TodoAttendeeProperty : AttendeeProperty<TodoPartStatuses>
    {
        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator Uri(TodoAttendeeProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator TodoAttendeeProperty(Uri value) { return value != null ? new TodoAttendeeProperty { Value = value } : null; }
    }

    /// <summary>
    /// ATTENDEE for free/busy
    /// </summary>
    public class FreeBusyAttendeeProperty : AttendeeProperty<EventPartStatuses>
    {
        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator Uri(FreeBusyAttendeeProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator FreeBusyAttendeeProperty(Uri value) { return value != null ? new FreeBusyAttendeeProperty { Value = value } : null; }
    }

}
