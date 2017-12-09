using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects
{
    public class EventTest
    {
        [Fact]
        public void Create()
        {
            var evnt = new Event();
            Assert.Equal("VEVENT", evnt.Name);
            Assert.Empty(evnt.Alarms);
            Assert.Empty(evnt.Attachments);
            Assert.Empty(evnt.Attendees);
            Assert.Empty(evnt.Categories);
            Assert.Empty(evnt.Comments);
            Assert.Empty(evnt.Contacts);
            Assert.Empty(evnt.Exdates);
            Assert.Empty(evnt.Exrules);
            Assert.Empty(evnt.RequestStatuses);
            Assert.Empty(evnt.RelatedTos);
            Assert.Empty(evnt.Resources);
            Assert.Empty(evnt.RecurDates);
            Assert.Empty(evnt.RecurRules);

            Assert.Null(evnt.UID);
            Assert.Null(evnt.DtStamp);
            Assert.Null(evnt.DateStart);
            Assert.Null(evnt.DateEnd);
            Assert.Null(evnt.Class);
            Assert.Null(evnt.Summary);
            Assert.Null(evnt.Description);
            Assert.Null(evnt.Created);
            Assert.Null(evnt.LastModified);
            Assert.Null(evnt.Organizer);
            Assert.Null(evnt.Sequence);
            Assert.Null(evnt.Status);
            Assert.Null(evnt.Url);
            Assert.Null(evnt.RecurrenceId);
            Assert.Null(evnt.Transp);
            Assert.Null(evnt.Duration);
            Assert.Null(evnt.Geo);
            Assert.Null(evnt.Location);
            Assert.Null(evnt.Priority);
            Assert.False(evnt.IsAllDay);
        }

        [Fact]
        public void Serialization()
        {
            DateTime dt = new DateTime(2017, 12, 2, 8, 15, 12);

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var evnt = new Event()
                {
                    UID = "Event-ID",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Class = Classes.Public,
                    Summary = "This is a event entry",
                    Description = "Description of the event entry",
                    Created = dt,
                    LastModified = dt.AddMinutes(3),
                    Organizer = "mailto:yan@deuxsucres.com",
                    Sequence = 1,
                    Status = EventStatuses.Tentative,
                    Url = "http://www.deuxsucres.com",
                    RecurrenceId = dt,
                    //Duration = TimeSpan.FromDays(2.3),
                    DateEnd = dt.AddDays(10),
                    Geo = new GeoPositionProperty { Latitude = 12.34, Longitude = 34.56 },
                    Location = "Here I am",
                    Priority = 3,
                    Transp = TransparentStates.Opaque,
                    IsAllDay = false
                };
                evnt.Alarms.Add(new EventAlarm
                {
                    Trigger = dt,
                    Duration = TimeSpan.FromMinutes(5)
                });
                evnt.Attachments.Add(new AttachProperty { UriValue = new Uri("http://www.deuxsucres.com") });
                evnt.Attachments.Add(new AttachProperty { UriValue = new Uri("https://tools.ietf.org/html/rfc2425") });
                evnt.Attendees.Add(new EventAttendeeProperty { Value = new Uri("mailto:contact@deuxsucres.com"), Role = Roles.Opt_Participant });
                evnt.Categories.Add(new string[] { "test", "event" });
                evnt.Comments.Add("This is a test of event");
                evnt.Contacts.Add("Yanos");
                evnt.Contacts.Add("deuxsucres");
                evnt.Exdates.Add(new DateTime[] { dt, dt.AddDays(1), dt.AddDays(1.5) });
                evnt.Exrules.Add(new iCalendar.Structure.RecurrenceProperty { Value = new Recurrence { ByMonth = new int[] { 2, 5 } } });
                evnt.RequestStatuses.Add(new RequestStatusProperty { StatusCode = "1.2", StatusDescription = "Success", ExtraData = "Good success", Language = "EN" });
                evnt.RelatedTos.Add(new RelatedToProperty { Value = "Test", RelationType = RelationTypes.Parent });
                evnt.Resources.Add("Some resource");
                evnt.RecurDates.Add(new RecurDateProperty { DatesValue = new List<DateTime> { dt, dt.AddYears(1) } });
                evnt.RecurRules.Add(new RecurRuleProperty { Value = new Recurrence { ByMinute = new List<int> { 12, 17, 32 } } });

                evnt.Serialize(writer);

                evnt = new Event()
                {
                    UID = "Event-ID-2",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Class = Classes.Public,
                    Summary = "This is a event entry",
                    Description = "Description of the event entry",
                    Created = dt,
                    LastModified = dt.AddMinutes(3),
                    Organizer = "mailto:yan@deuxsucres.com",
                    Sequence = 1,
                    Status = EventStatuses.Confirmed,
                    Url = "http://www.deuxsucres.com",
                    RecurrenceId = dt,
                    Duration = TimeSpan.FromDays(2.3),
                    Geo = new GeoPositionProperty { Latitude = 12.34, Longitude = 34.56 },
                    Location = "Here I am",
                    Priority = 3,
                    Transp = TransparentStates.Transparent,
                    IsAllDay = true
                };
                evnt.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:VEVENT")
                .AppendLine("UID:Event-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a event entry")
                .AppendLine("DESCRIPTION:Description of the event entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:TENTATIVE")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("DTEND:20171212T081512")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PRIORITY:3")
                .AppendLine("TRANSP:OPAQUE")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTACH:https://tools.ietf.org/html/rfc2425")
                .AppendLine("ATTENDEE;ROLE=OPT-PARTICIPANT:mailto:contact@deuxsucres.com")
                .AppendLine("CATEGORIES:test,event")
                .AppendLine("COMMENT:This is a test of event")
                .AppendLine("CONTACT:Yanos")
                .AppendLine("CONTACT:deuxsucres")
                .AppendLine("EXDATE:20171202T081512,20171203T081512,20171203T201512")
                .AppendLine("EXRULE:FREQ=SECONDLY;BYMONTH=2,5")
                .AppendLine("REQUEST-STATUS;LANGUAGE=EN:1.2;Success;Good success")
                .AppendLine("RELATED-TO;RELTYPE=PARENT:Test")
                .AppendLine("RESOURCES:Some resource")
                .AppendLine("RDATE:20171202T081512,20181202T081512")
                .AppendLine("RRULE:FREQ=SECONDLY;BYMINUTE=12,17,32")
                .AppendLine("BEGIN:VALARM")
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171202T081512")
                .AppendLine("DURATION:PT5M")
                .AppendLine("END:VALARM")
                .AppendLine("END:VEVENT")
                .AppendLine("BEGIN:VEVENT")
                .AppendLine("UID:Event-ID-2")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART;VALUE=DATE:20171202")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a event entry")
                .AppendLine("DESCRIPTION:Description of the event entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:CONFIRMED")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("DURATION:P2DT7H12M")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PRIORITY:3")
                .AppendLine("TRANSP:TRANSPARENT")
                .AppendLine("END:VEVENT")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("BEGIN:VEVENT")
                .AppendLine("UID:Event-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a event entry")
                .AppendLine("DESCRIPTION:Description of the event entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:TENTATIVE")
                .AppendLine("Other:Other property value")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("DTEND:20171212T081512")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PRIORITY:3")
                .AppendLine("TRANSP:OPAQUE")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTACH:https://tools.ietf.org/html/rfc2425")
                .AppendLine("ATTENDEE;ROLE=OPT-PARTICIPANT:mailto:contact@deuxsucres.com")
                .AppendLine("CATEGORIES:test,event")
                .AppendLine("COMMENT:This is a test of event")
                .AppendLine("CONTACT:Yanos")
                .AppendLine("CONTACT:deuxsucres")
                .AppendLine("EXDATE:20171202T081512,20171203T081512,20171203T201512")
                .AppendLine("EXRULE:FREQ=SECONDLY;BYMONTH=2,5")
                .AppendLine("REQUEST-STATUS;LANGUAGE=EN:1.2;Success;Good success")
                .AppendLine("RELATED-TO;RELTYPE=PARENT:Test")
                .AppendLine("RESOURCES:Some resource")
                .AppendLine("RDATE:20171202T081512,20181202T081512")
                .AppendLine("RRULE:FREQ=SECONDLY;BYMINUTE=12,17,32")
                .AppendLine("BEGIN:VALARM")
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171202T081512")
                .AppendLine("DURATION:PT5M")
                .AppendLine("END:VALARM")
                .AppendLine("END:VEVENT")
                .AppendLine("BEGIN:VEVENT")
                .AppendLine("UID:Event-ID-2")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART;VALUE=DATE:20171202")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a event entry")
                .AppendLine("DESCRIPTION:Description of the event entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:CONFIRMED")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("DURATION:P2DT7H12M")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PRIORITY:3")
                .AppendLine("TRANSP:TRANSPARENT")
                .AppendLine("BEGIN:VJOURNAL")
                .AppendLine("END:VJOURNAL")
                .AppendLine("END:VEVENT")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var evnt = new Event();
                evnt.Deserialize(reader);

                Assert.Equal(33, evnt.PropertyCount);

                Assert.Equal("Event-ID", evnt.UID);
                Assert.Equal(dt, (DateTime)evnt.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45), (DateTime)evnt.DateStart);
                Assert.Equal("PUBLIC", evnt.Class);
                Assert.Equal("This is a event entry", evnt.Summary);
                Assert.Equal("Description of the event entry", evnt.Description);
                Assert.Equal(dt, (DateTime)evnt.Created);
                Assert.Equal(dt.AddMinutes(3), (DateTime)evnt.LastModified);
                Assert.Equal("mailto:yan@deuxsucres.com", evnt.Organizer);
                Assert.Equal(1, (int)evnt.Sequence);
                Assert.Equal("TENTATIVE", evnt.Status);
                Assert.Equal("http://www.deuxsucres.com/", evnt.Url);
                Assert.Equal(dt, (DateTime)evnt.RecurrenceId);
                Assert.Null(evnt.Duration);
                Assert.Equal(dt.AddDays(10), (DateTime)evnt.DateEnd);
                Assert.NotNull(evnt.Geo);
                Assert.Equal(12.34, evnt.Geo.Latitude);
                Assert.Equal(34.56, evnt.Geo.Longitude);
                Assert.Equal("Here I am", evnt.Location);
                Assert.Equal(3, (int)evnt.Priority);
                Assert.False(evnt.IsAllDay);

                Assert.Equal(2, evnt.Attachments.Count);
                Assert.Equal("http://www.deuxsucres.com/", evnt.Attachments[0].UriValue.ToString());
                Assert.Equal("https://tools.ietf.org/html/rfc2425", evnt.Attachments[1].UriValue.ToString());

                Assert.Single(evnt.Attendees);
                Assert.Equal("mailto:contact@deuxsucres.com", evnt.Attendees[0].Value.ToString());
                Assert.Equal(Roles.Opt_Participant, (Roles)evnt.Attendees[0].Role);

                Assert.Single(evnt.Categories);
                Assert.Equal(new string[] { "test", "event" }, (string[])evnt.Categories[0]);

                Assert.Single(evnt.Comments);
                Assert.Equal("This is a test of event", evnt.Comments[0]);

                Assert.Equal(2, evnt.Contacts.Count);
                Assert.Equal("Yanos", evnt.Contacts[0]);
                Assert.Equal("deuxsucres", evnt.Contacts[1]);

                Assert.Single(evnt.Exdates);
                Assert.Equal(new DateTime[] { dt, dt.AddDays(1), dt.AddDays(1.5) }, (DateTime[])evnt.Exdates[0]);

                Assert.Single(evnt.Exrules);
                Assert.Equal(new int[] { 2, 5 }, evnt.Exrules[0].Value.ByMonth);

                Assert.Single(evnt.RequestStatuses);
                Assert.Equal("1.2", evnt.RequestStatuses[0].StatusCode);
                Assert.Equal("Success", evnt.RequestStatuses[0].StatusDescription);
                Assert.Equal("Good success", evnt.RequestStatuses[0].ExtraData);

                Assert.Single(evnt.RelatedTos);
                Assert.Equal("Test", evnt.RelatedTos[0].Value);
                Assert.Equal("PARENT", evnt.RelatedTos[0].RelationType);

                Assert.Single(evnt.Resources);
                Assert.Equal("Some resource", evnt.Resources[0]);

                Assert.Single(evnt.RecurDates);
                Assert.Equal(new DateTime[] { dt, dt.AddYears(1) }, (DateTime[])evnt.RecurDates[0]);

                Assert.Single(evnt.RecurRules);
                Assert.Equal(new List<int> { 12, 17, 32 }, evnt.RecurRules[0].Value.ByMinute);

                Assert.Single(evnt.Alarms);
                Assert.Equal(dt, (DateTime)evnt.Alarms[0].Trigger);
                Assert.Equal(TimeSpan.FromMinutes(5), (TimeSpan)evnt.Alarms[0].Duration);

                Assert.Equal("Other property value", (string)evnt.FindProperties<TextProperty>("OTHER").Single());

                reader.ReadNextLine();

                evnt = new Event();
                evnt.Deserialize(reader);

                Assert.Equal(18, evnt.PropertyCount);

                Assert.Equal("Event-ID-2", evnt.UID);
                Assert.Equal(dt, (DateTime)evnt.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45).Date, (DateTime)evnt.DateStart);
                Assert.Equal("PUBLIC", evnt.Class);
                Assert.Equal("This is a event entry", evnt.Summary);
                Assert.Equal("Description of the event entry", evnt.Description);
                Assert.Equal(dt, (DateTime)evnt.Created);
                Assert.Equal(dt.AddMinutes(3), (DateTime)evnt.LastModified);
                Assert.Equal("mailto:yan@deuxsucres.com", evnt.Organizer);
                Assert.Equal(1, (int)evnt.Sequence);
                Assert.Equal("CONFIRMED", evnt.Status);
                Assert.Equal("http://www.deuxsucres.com/", evnt.Url);
                Assert.Equal(dt, (DateTime)evnt.RecurrenceId);
                Assert.Equal(TimeSpan.FromDays(2.3), (TimeSpan)evnt.Duration);
                Assert.Null(evnt.DateEnd);
                Assert.NotNull(evnt.Geo);
                Assert.Equal(12.34, evnt.Geo.Latitude);
                Assert.Equal(34.56, evnt.Geo.Longitude);
                Assert.Equal("Here I am", evnt.Location);
                Assert.Equal(3, (int)evnt.Priority);
                Assert.True(evnt.IsAllDay);

                Assert.Null(reader.ReadNextLine());
            }
        }
    }
}
