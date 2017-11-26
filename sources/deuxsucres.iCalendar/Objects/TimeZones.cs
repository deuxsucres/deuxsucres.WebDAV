using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    ///// <summary>
    ///// Time zone
    ///// </summary>
    //public class TimeZone : CalComponent
    //{
    //    /// <summary>
    //    /// Create a new component
    //    /// </summary>
    //    public TimeZone()
    //    {

    //    }

    //    /// <summary>
    //    /// Process the components
    //    /// </summary>
    //    protected override bool ProcessComponent(CalComponent component)
    //    {
    //        if (component is TimeZoneComponent)
    //        {
    //            ZoneComponents.Add((TimeZoneComponent)component);
    //            return true;
    //        }
    //        return base.ProcessComponent(component);
    //    }

    //    /// <summary>
    //    /// Read a child component
    //    /// </summary>
    //    protected override CalComponent ReadComponent(ICalReader reader, ContentLine line)
    //    {
    //        if (line.Value.IsEqual(Constants.STANDARD))
    //        {
    //            var comp = TimeZoneComponent.CreateStandard();
    //            comp.Deserialize(reader);
    //            return comp;
    //        }
    //        else if (line.Value.IsEqual(Constants.DAYLIGHT))
    //        {
    //            var comp = TimeZoneComponent.CreateDaylight();
    //            comp.Deserialize(reader);
    //            return comp;
    //        }
    //        return base.ReadComponent(reader, line);
    //    }

    //    /// <summary>
    //    /// Process the properties
    //    /// </summary>
    //    protected override bool ProcessProperty(ICalReader reader, ContentLine line)
    //    {
    //        switch (line.Name.ToUpper())
    //        {
    //            case Constants.TZID: SetProperty(reader.MakeProperty<TzidProperty>(line), Constants.TZID); return true;
    //            case Constants.LAST_MODIFIED: SetProperty(reader.MakeProperty<DateTimeProperty>(line), Constants.LAST_MODIFIED); return true;
    //            case Constants.TZURL: SetProperty(reader.MakeProperty<TzUrlProperty>(line), Constants.TZURL); return true;
    //            default: return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Name of the object
    //    /// </summary>
    //    public override string Name => Constants.VTIMEZONE;

    //    /// <summary>
    //    /// TZID
    //    /// </summary>
    //    public TzidProperty TzId
    //    {
    //        get { return FindProperty<TzidProperty>(Constants.TZID); }
    //        set { SetProperty(value, Constants.TZID); }
    //    }

    //    /// <summary>
    //    /// LAST-MODIFIED
    //    /// </summary>
    //    public DateTimeProperty LastModified
    //    {
    //        get { return FindProperty<DateTimeProperty>(Constants.LAST_MODIFIED); }
    //        set { SetProperty(value, Constants.LAST_MODIFIED); }
    //    }

    //    /// <summary>
    //    /// TZURL
    //    /// </summary>
    //    public TzUrlProperty TzUrl
    //    {
    //        get { return FindProperty<TzUrlProperty>(Constants.TZURL); }
    //        set { SetProperty(value, Constants.TZURL); }
    //    }

    //    /// <summary>
    //    /// List of components of the time zone
    //    /// </summary>
    //    public List<TimeZoneComponent> ZoneComponents { get; private set; } = new List<TimeZoneComponent>();

    //}

    ///// <summary>
    ///// TimeZone component
    ///// </summary>
    //public class TimeZoneComponent : CalComponent
    //{

    //    /// <summary>
    //    /// Create a new component
    //    /// </summary>
    //    public TimeZoneComponent()
    //    {
    //        Comments = new CalProperties<CommentProperty>(Constants.COMMENT, this);
    //        RecurDates = new CalProperties<RecurDateProperty>(Constants.RDATE, this);
    //        RecurRules = new CalProperties<RecurRuleProperty>(Constants.RRULE, this);
    //        TimeZoneNames = new CalProperties<TzNameProperty>(Constants.TZNAME, this);
    //    }

    //    /// <summary>
    //    /// Create a standard timezone
    //    /// </summary>
    //    public static TimeZoneComponent CreateStandard() { return new TimeZoneComponent { IsDayLight = false }; }

    //    /// <summary>
    //    /// Create a daylight timezone
    //    /// </summary>
    //    public static TimeZoneComponent CreateDaylight() { return new TimeZoneComponent { IsDayLight = true }; }

    //    /// <summary>
    //    /// Indicates if this component is a daylight definition
    //    /// </summary>
    //    public bool IsDayLight { get; set; }

    //    /// <summary>
    //    /// Process the properties
    //    /// </summary>
    //    protected override bool ProcessProperty(ICalReader reader, ContentLine line)
    //    {
    //        switch (line.Name.ToUpper())
    //        {
    //            case Constants.DTSTART: SetProperty(reader.MakeProperty<DtStartProperty>(line), Constants.DTSTART); return true;
    //            case Constants.TZOFFSETFROM: SetProperty(reader.MakeProperty<TzOffsetFromProperty>(line), Constants.TZOFFSETFROM); return true;
    //            case Constants.TZOFFSETTO: SetProperty(reader.MakeProperty<TzOffsetToProperty>(line), Constants.TZOFFSETTO); return true;

    //            case Constants.COMMENT: AddProperty(reader.MakeProperty<CommentProperty>(line)); return true;
    //            case Constants.RDATE: AddProperty(reader.MakeProperty<RecurDateProperty>(line)); return true;
    //            case Constants.RRULE: AddProperty(reader.MakeProperty<RecurRuleProperty>(line)); return true;
    //            case Constants.TZNAME: AddProperty(reader.MakeProperty<TzNameProperty>(line)); return true;
    //            default: return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Name
    //    /// </summary>
    //    public override string Name => IsDayLight ? Constants.DAYLIGHT : Constants.STANDARD;

    //    /// <summary>
    //    /// DTSTART
    //    /// </summary>
    //    public DtStartProperty DateStart
    //    {
    //        get { return FindProperty<DtStartProperty>(Constants.DTSTART); }
    //        set { SetProperty(value, Constants.DTSTART); }
    //    }

    //    /// <summary>
    //    /// TZOFFSETFROM
    //    /// </summary>
    //    public TzOffsetFromProperty OffsetFrom
    //    {
    //        get { return FindProperty<TzOffsetFromProperty>(Constants.TZOFFSETFROM); }
    //        set { SetProperty(value, Constants.TZOFFSETFROM); }
    //    }

    //    /// <summary>
    //    /// TZOFFSETTO
    //    /// </summary>
    //    public TzOffsetToProperty OffsetTo
    //    {
    //        get { return FindProperty<TzOffsetToProperty>(Constants.TZOFFSETTO); }
    //        set { SetProperty(value, Constants.TZOFFSETTO); }
    //    }

    //    /// <summary>
    //    /// List of comments
    //    /// </summary>
    //    public CalProperties<CommentProperty> Comments { get; private set; }

    //    /// <summary>
    //    /// List of recurrence dates
    //    /// </summary>
    //    public CalProperties<RecurDateProperty> RecurDates { get; private set; }

    //    /// <summary>
    //    /// List of recurrence rules
    //    /// </summary>
    //    public CalProperties<RecurRuleProperty> RecurRules { get; private set; }

    //    /// <summary>
    //    /// List of the names
    //    /// </summary>
    //    public CalProperties<TzNameProperty> TimeZoneNames { get; private set; }

    //}
}
