using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Alarm
    /// </summary>
    public abstract class Alarm<TAttendeeProp, TStatus> : CalComponent
        where TStatus : struct
        where TAttendeeProp : AttendeeProperty<TStatus>
    {
        /// <summary>
        /// Create a new alarm
        /// </summary>
        public Alarm()
        {
            Attachments = new CalProperties<AttachProperty>(Constants.ATTACH, this);
            Attendees = new CalProperties<TAttendeeProp>(Constants.ATTENDEE, this);
        }

        /// <summary>
        /// Process the properties
        /// </summary>
        protected override bool ProcessProperty(ICalReader reader, ContentLine line)
        {
            switch (line.Name.ToUpper())
            {
                case Constants.ACTION: SetProperty(reader.MakeProperty<EnumProperty<AlarmActions>>(line), Constants.ACTION); return true;
                case Constants.TRIGGER: SetProperty(reader.MakeProperty<TriggerProperty>(line), Constants.TRIGGER); return true;
                case Constants.DURATION: SetProperty(reader.MakeProperty<DurationProperty>(line), Constants.DURATION); return true;
                case Constants.REPEAT: SetProperty(reader.MakeProperty<IntegerProperty>(line), Constants.REPEAT); return true;
                case Constants.SUMMARY: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.SUMMARY); return true;
                case Constants.DESCRIPTION: SetProperty(reader.MakeProperty<ExtendedTextProperty>(line), Constants.DESCRIPTION); return true;

                case Constants.ATTACH: AddProperty(reader.MakeProperty<AttachProperty>(line)); return true;
                case Constants.ATTENDEE: AddProperty(reader.MakeProperty<TAttendeeProp>(line)); return true;
                default: return false;
            }
        }

        /// <summary>
        /// Get the attendees
        /// </summary>
        public virtual IEnumerable<TAttendeeProp> GetAttendees()
        {
            return FindProperties<TAttendeeProp>(Constants.ATTENDEE);
        }

        /// <summary>
        /// Name of the object
        /// </summary>
        public override string Name => Constants.VALARM;

        /// <summary>
        /// ACTION
        /// </summary>
        public EnumProperty<AlarmActions> Action
        {
            get { return FindProperty<EnumProperty<AlarmActions>>(Constants.ACTION); }
            set { SetProperty(value, Constants.ACTION); }
        }

        /// <summary>
        /// TRIGGER
        /// </summary>
        public TriggerProperty Trigger
        {
            get { return FindProperty<TriggerProperty>(Constants.TRIGGER); }
            set { SetProperty(value, Constants.TRIGGER); }
        }

        /// <summary>
        /// DURATION
        /// </summary>
        public DurationProperty Duration
        {
            get { return FindProperty<DurationProperty>(Constants.DURATION); }
            set { SetProperty(value, Constants.DURATION); }
        }

        /// <summary>
        /// REPEAT
        /// </summary>
        public IntegerProperty Repeat
        {
            get { return FindProperty<IntegerProperty>(Constants.REPEAT); }
            set { SetProperty(value, Constants.REPEAT); }
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
        /// SUMMARY
        /// </summary>
        public ExtendedTextProperty Summary
        {
            get { return FindProperty<ExtendedTextProperty>(Constants.SUMMARY); }
            set { SetProperty(value, Constants.SUMMARY); }
        }

        /// <summary>
        /// List of attachments
        /// </summary>
        public CalProperties<AttachProperty> Attachments { get; private set; }

        /// <summary>
        /// List of attendees
        /// </summary>
        public CalProperties<TAttendeeProp> Attendees { get; private set; }
    }

    /// <summary>
    /// ALARM for event
    /// </summary>
    public class EventAlarm : Alarm<EventAttendeeProperty, EventPartStatuses>
    {
    }

    /// <summary>
    /// ALARM for todo
    /// </summary>
    public class TodoAlarm : Alarm<TodoAttendeeProperty, TodoPartStatuses>
    {
    }

}
