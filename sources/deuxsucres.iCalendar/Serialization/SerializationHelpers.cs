using deuxsucres.iCalendar.Locales;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar.Serialization
{

    /// <summary>
    /// Somme common serialization helpers
    /// </summary>
    public static class SerializationHelpers
    {
        /// <summary>
        /// Default parameter creation from his name
        /// </summary>
        public static ICalPropertyParameter DefaultCreateParameter(string name)
        {
            switch ((name ?? string.Empty).ToUpper())
            {
                case Constants.ALTREP: return new UriParameter() { Name = name.ToUpper() };
                case Constants.CN: return new TextParameter() { Name = name.ToUpper() };
                case Constants.CUTYPE: return new EnumParameter<CalUserTypes>() { Name = name.ToUpper() };
                case Constants.DELEGATED_FROM: return new CalAddressesParameter() { Name = name.ToUpper() };
                case Constants.DELEGATED_TO: return new CalAddressesParameter() { Name = name.ToUpper() };
                case Constants.DIR: return new UriParameter() { Name = name.ToUpper() };
                case Constants.ENCODING: return new TextParameter() { Name = name.ToUpper() };
                case Constants.FMTTYPE: return new TextParameter() { Name = name.ToUpper() };
                case Constants.FBTYPE: return new EnumParameter<FreeBusyTypes>() { Name = name.ToUpper() };
                case Constants.LANGUAGE: return new TextParameter() { Name = name.ToUpper() };
                case Constants.MEMBER: return new CalAddressesParameter() { Name = name.ToUpper() };
                //case Constants.PARTSTAT: return new EnumParameter<PartStatuses>(){ Name = name.ToUpper() };
                case Constants.RANGE: return new EnumParameter<Ranges>() { Name = name.ToUpper() };
                case Constants.RELATED: return new EnumParameter<Relateds>() { Name = name.ToUpper() };
                case Constants.RELTYPE: return new EnumParameter<RelationTypes>() { Name = name.ToUpper() };
                case Constants.ROLE: return new EnumParameter<Roles>() { Name = name.ToUpper() };
                case Constants.RSVP: return new BooleanParameter() { Name = name.ToUpper() };
                case Constants.SENT_BY: return new CalAddressParameter() { Name = name.ToUpper() };
                case Constants.TZID: return new TextParameter() { Name = name.ToUpper() };
                case Constants.VALUE: return new EnumParameter<ValueTypes>() { Name = name.ToUpper() };
                default:
                    return new TextParameter() { Name = name };
            }
        }

        /// <summary>
        /// Default property creation from his name
        /// </summary>
        public static ICalProperty DefaultCreateProperty(string name)
        {
            switch ((name ?? string.Empty).ToUpper())
            {
                case Constants.UID: throw new NotImplementedException("Property creation for Constants.UID not implemented"); 
                    //TODO Implements : return new UidProperty();
                default:
                    return new TextProperty() { Name = name };
            }
        }

        /// <summary>
        /// Default component creation from his name
        /// </summary>
        public static CalComponent DefaultCreateComponent(string name)
        {
            switch (name.ToUpper())
            {
                case Constants.VEVENT:    throw new NotImplementedException("Component creation for Constants.VEVENT not implemented");   // TODO Implements : return new Objects.Event();
                case Constants.VTODO:     throw new NotImplementedException("Component creation for Constants.VTODO not implemented");   // TODO Implements : return new Objects.Todo();
                case Constants.VJOURNAL:  throw new NotImplementedException("Component creation for Constants.VJOURNAL not implemented");   // TODO Implements : return new Objects.Journal();
                case Constants.VFREEBUSY: throw new NotImplementedException("Component creation for Constants.VFREEBUSY not implemented");   // TODO Implements : return new Objects.FreeBusy();
                case Constants.VTIMEZONE: throw new NotImplementedException("Component creation for Constants.VTIMEZONE not implemented");   // TODO Implements : return new Objects.TimeZone();
                default:
                    throw new CalSyntaxError(string.Format(SR.Err_UnknownComponentName, name));
            }
        }

    }

}
