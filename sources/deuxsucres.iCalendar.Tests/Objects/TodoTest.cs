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
    public class TodoTest
    {
        [Fact]
        public void Create()
        {
            var todo = new Todo();
            Assert.Equal("VTODO", todo.Name);
            Assert.Empty(todo.Alarms);
            Assert.Empty(todo.Attachments);
            Assert.Empty(todo.Attendees);
            Assert.Empty(todo.Categories);
            Assert.Empty(todo.Comments);
            Assert.Empty(todo.Contacts);
            Assert.Empty(todo.Exdates);
            Assert.Empty(todo.Exrules);
            Assert.Empty(todo.RequestStatuses);
            Assert.Empty(todo.RelatedTos);
            Assert.Empty(todo.Resources);
            Assert.Empty(todo.RecurDates);
            Assert.Empty(todo.RecurRules);

            Assert.Null(todo.UID);
            Assert.Null(todo.DtStamp);
            Assert.Null(todo.DateStart);
            Assert.Null(todo.Class);
            Assert.Null(todo.Summary);
            Assert.Null(todo.Description);
            Assert.Null(todo.Created);
            Assert.Null(todo.LastModified);
            Assert.Null(todo.Organizer);
            Assert.Null(todo.Sequence);
            Assert.Null(todo.Status);
            Assert.Null(todo.Url);
            Assert.Null(todo.RecurrenceId);
            Assert.Null(todo.Completed);
            Assert.Null(todo.Duration);
            Assert.Null(todo.Due);
            Assert.Null(todo.Geo);
            Assert.Null(todo.Location);
            Assert.Null(todo.PercentCompleted);
            Assert.Null(todo.Priority);
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

                var todo = new Todo()
                {
                    UID = "Todo-ID",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Class = Classes.Public,
                    Summary = "This is a todo entry",
                    Description = "Description of the todo entry",
                    Created = dt,
                    LastModified = dt.AddMinutes(3),
                    Organizer = "mailto:yan@deuxsucres.com",
                    Sequence = 1,
                    Status = TodoStatuses.In_Process,
                    Url = "http://www.deuxsucres.com",
                    RecurrenceId = dt,
                    Completed = dt.AddDays(12.34),
                    //Duration = TimeSpan.FromDays(2.3),
                    Due = dt.AddDays(10),
                    Geo = new GeoPositionProperty { Latitude = 12.34, Longitude = 34.56 },
                    Location = "Here I am",
                    PercentCompleted = 78,
                    Priority = 3
                };
                todo.Alarms.Add(new TodoAlarm
                {
                    Trigger = dt,
                    Duration = TimeSpan.FromMinutes(5)
                });
                todo.Attachments.Add(new AttachProperty { UriValue = new Uri("http://www.deuxsucres.com") });
                todo.Attachments.Add(new AttachProperty { UriValue = new Uri("https://tools.ietf.org/html/rfc2425") });
                todo.Attendees.Add(new TodoAttendeeProperty { Value = new Uri("mailto:contact@deuxsucres.com"), Role = Roles.Opt_Participant });
                todo.Categories.Add(new string[] { "test", "journal" });
                todo.Comments.Add("This is a test of journal");
                todo.Contacts.Add("Yanos");
                todo.Contacts.Add("deuxsucres");
                todo.Exdates.Add(new DateTime[] { dt, dt.AddDays(1), dt.AddDays(1.5) });
                todo.Exrules.Add(new iCalendar.Structure.RecurrenceProperty { Value = new Recurrence { ByMonth = new int[] { 2, 5 } } });
                todo.RequestStatuses.Add(new RequestStatusProperty { StatusCode = "1.2", StatusDescription = "Success", ExtraData = "Good success", Language = "EN" });
                todo.RelatedTos.Add(new RelatedToProperty { Value = "Test", RelationType = RelationTypes.Parent });
                todo.Resources.Add("Some resource");
                todo.RecurDates.Add(new RecurDateProperty { DatesValue = new List<DateTime> { dt, dt.AddYears(1) } });
                todo.RecurRules.Add(new RecurRuleProperty { Value = new Recurrence { ByMinute = new List<int> { 12, 17, 32 } } });

                todo.Serialize(writer);

                todo = new Todo()
                {
                    UID = "Todo-ID-2",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Class = Classes.Public,
                    Summary = "This is a todo entry",
                    Description = "Description of the todo entry",
                    Created = dt,
                    LastModified = dt.AddMinutes(3),
                    Organizer = "mailto:yan@deuxsucres.com",
                    Sequence = 1,
                    Status = TodoStatuses.In_Process,
                    Url = "http://www.deuxsucres.com",
                    RecurrenceId = dt,
                    Completed = dt.AddDays(12.34),
                    Duration = TimeSpan.FromDays(2.3),
                    Geo = new GeoPositionProperty { Latitude = 12.34, Longitude = 34.56 },
                    Location = "Here I am",
                    PercentCompleted = 78,
                    Priority = 3
                };
                todo.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:VTODO")
                .AppendLine("UID:Todo-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a todo entry")
                .AppendLine("DESCRIPTION:Description of the todo entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:IN-PROCESS")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("COMPLETED:20171214T162448")
                .AppendLine("DUE:20171212T081512")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PERCENT-COMPLETE:78")
                .AppendLine("PRIORITY:3")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTACH:https://tools.ietf.org/html/rfc2425")
                .AppendLine("ATTENDEE;ROLE=OPT-PARTICIPANT:mailto:contact@deuxsucres.com")
                .AppendLine("CATEGORIES:test,journal")
                .AppendLine("COMMENT:This is a test of journal")
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
                .AppendLine("END:VTODO")
                .AppendLine("BEGIN:VTODO")
                .AppendLine("UID:Todo-ID-2")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a todo entry")
                .AppendLine("DESCRIPTION:Description of the todo entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:IN-PROCESS")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("COMPLETED:20171214T162448")
                .AppendLine("DURATION:P2DT7H12M")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PERCENT-COMPLETE:78")
                .AppendLine("PRIORITY:3")
                .AppendLine("END:VTODO")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("BEGIN:VTODO")
                .AppendLine("UID:Todo-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a todo entry")
                .AppendLine("DESCRIPTION:Description of the todo entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:IN-PROCESS")
                .AppendLine("Other:Other property value")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("COMPLETED:20171214T162448")
                .AppendLine("DUE:20171212T081512")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PERCENT-COMPLETE:78")
                .AppendLine("PRIORITY:3")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTACH:https://tools.ietf.org/html/rfc2425")
                .AppendLine("ATTENDEE;ROLE=OPT-PARTICIPANT:mailto:contact@deuxsucres.com")
                .AppendLine("CATEGORIES:test,journal")
                .AppendLine("COMMENT:This is a test of journal")
                .AppendLine("CONTACT:Yanos")
                .AppendLine("CONTACT:deuxsucres")
                .AppendLine("BEGIN:VALARM")
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171202T081512")
                .AppendLine("DURATION:PT5M")
                .AppendLine("END:VALARM")
                .AppendLine("EXDATE:20171202T081512,20171203T081512,20171203T201512")
                .AppendLine("EXRULE:FREQ=SECONDLY;BYMONTH=2,5")
                .AppendLine("REQUEST-STATUS;LANGUAGE=EN:1.2;Success;Good success")
                .AppendLine("RELATED-TO;RELTYPE=PARENT:Test")
                .AppendLine("RESOURCES:Some resource")
                .AppendLine("RDATE:20171202T081512,20181202T081512")
                .AppendLine("RRULE:FREQ=SECONDLY;BYMINUTE=12,17,32")
                .AppendLine("END:VTODO")
                .AppendLine("BEGIN:VTODO")
                .AppendLine("UID:Todo-ID-2")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a todo entry")
                .AppendLine("DESCRIPTION:Description of the todo entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:IN-PROCESS")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
                .AppendLine("COMPLETED:20171214T162448")
                .AppendLine("DURATION:P2DT7H12M")
                .AppendLine("GEO:12.34;34.56")
                .AppendLine("LOCATION:Here I am")
                .AppendLine("PERCENT-COMPLETE:78")
                .AppendLine("PRIORITY:3")
                .AppendLine("BEGIN:VJOURNAL")
                .AppendLine("END:VJOURNAL")
                .AppendLine("END:VTODO")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var todo = new Todo();
                todo.Deserialize(reader);

                Assert.Equal(34, todo.PropertyCount);

                Assert.Equal("Todo-ID", todo.UID);
                Assert.Equal(dt, (DateTime)todo.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45), (DateTime)todo.DateStart);
                Assert.Equal("PUBLIC", todo.Class);
                Assert.Equal("This is a todo entry", todo.Summary);
                Assert.Equal("Description of the todo entry", todo.Description);
                Assert.Equal(dt, (DateTime)todo.Created);
                Assert.Equal(dt.AddMinutes(3), (DateTime)todo.LastModified);
                Assert.Equal("mailto:yan@deuxsucres.com", todo.Organizer);
                Assert.Equal(1, (int)todo.Sequence);
                Assert.Equal("IN-PROCESS", todo.Status);
                Assert.Equal("http://www.deuxsucres.com/", todo.Url);
                Assert.Equal(dt, (DateTime)todo.RecurrenceId);
                Assert.Equal(dt.AddDays(12.34), (DateTime)todo.Completed);
                Assert.Null(todo.Duration);
                Assert.Equal(dt.AddDays(10), (DateTime)todo.Due);
                Assert.NotNull(todo.Geo);
                Assert.Equal(12.34, todo.Geo.Latitude);
                Assert.Equal(34.56, todo.Geo.Longitude);
                Assert.Equal("Here I am", todo.Location);
                Assert.Equal(78, (int)todo.PercentCompleted);
                Assert.Equal(3, (int)todo.Priority);

                Assert.Equal(2, todo.Attachments.Count);
                Assert.Equal("http://www.deuxsucres.com/", todo.Attachments[0].UriValue.ToString());
                Assert.Equal("https://tools.ietf.org/html/rfc2425", todo.Attachments[1].UriValue.ToString());

                Assert.Single(todo.Attendees);
                Assert.Equal("mailto:contact@deuxsucres.com", todo.Attendees[0].Value.ToString());
                Assert.Equal(Roles.Opt_Participant, (Roles)todo.Attendees[0].Role);

                Assert.Single(todo.Categories);
                Assert.Equal(new string[] { "test", "journal" }, (string[])todo.Categories[0]);

                Assert.Single(todo.Comments);
                Assert.Equal("This is a test of journal", todo.Comments[0]);

                Assert.Equal(2, todo.Contacts.Count);
                Assert.Equal("Yanos", todo.Contacts[0]);
                Assert.Equal("deuxsucres", todo.Contacts[1]);

                Assert.Single(todo.Exdates);
                Assert.Equal(new DateTime[] { dt, dt.AddDays(1), dt.AddDays(1.5) }, (DateTime[])todo.Exdates[0]);

                Assert.Single(todo.Exrules);
                Assert.Equal(new int[] { 2, 5 }, todo.Exrules[0].Value.ByMonth);

                Assert.Single(todo.RequestStatuses);
                Assert.Equal("1.2", todo.RequestStatuses[0].StatusCode);
                Assert.Equal("Success", todo.RequestStatuses[0].StatusDescription);
                Assert.Equal("Good success", todo.RequestStatuses[0].ExtraData);

                Assert.Single(todo.RelatedTos);
                Assert.Equal("Test", todo.RelatedTos[0].Value);
                Assert.Equal("PARENT", todo.RelatedTos[0].RelationType);

                Assert.Single(todo.Resources);
                Assert.Equal("Some resource", todo.Resources[0]);

                Assert.Single(todo.RecurDates);
                Assert.Equal(new DateTime[] { dt, dt.AddYears(1) }, (DateTime[])todo.RecurDates[0]);

                Assert.Single(todo.RecurRules);
                Assert.Equal(new List<int> { 12, 17, 32 }, todo.RecurRules[0].Value.ByMinute);

                Assert.Single(todo.Alarms);
                Assert.Equal(dt, (DateTime)todo.Alarms[0].Trigger);
                Assert.Equal(TimeSpan.FromMinutes(5), (TimeSpan)todo.Alarms[0].Duration);

                Assert.Equal("Other property value", (string)todo.FindProperties<TextProperty>("OTHER").Single());

                reader.ReadNextLine();

                todo = new Todo();
                todo.Deserialize(reader);

                Assert.Equal(19, todo.PropertyCount);

                Assert.Equal("Todo-ID-2", todo.UID);
                Assert.Equal(dt, (DateTime)todo.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45), (DateTime)todo.DateStart);
                Assert.Equal("PUBLIC", todo.Class);
                Assert.Equal("This is a todo entry", todo.Summary);
                Assert.Equal("Description of the todo entry", todo.Description);
                Assert.Equal(dt, (DateTime)todo.Created);
                Assert.Equal(dt.AddMinutes(3), (DateTime)todo.LastModified);
                Assert.Equal("mailto:yan@deuxsucres.com", todo.Organizer);
                Assert.Equal(1, (int)todo.Sequence);
                Assert.Equal("IN-PROCESS", todo.Status);
                Assert.Equal("http://www.deuxsucres.com/", todo.Url);
                Assert.Equal(dt, (DateTime)todo.RecurrenceId);
                Assert.Equal(dt.AddDays(12.34), (DateTime)todo.Completed);
                Assert.Equal(TimeSpan.FromDays(2.3), (TimeSpan)todo.Duration);
                Assert.Null(todo.Due);
                Assert.NotNull(todo.Geo);
                Assert.Equal(12.34, todo.Geo.Latitude);
                Assert.Equal(34.56, todo.Geo.Longitude);
                Assert.Equal("Here I am", todo.Location);
                Assert.Equal(78, (int)todo.PercentCompleted);
                Assert.Equal(3, (int)todo.Priority);

                Assert.Null(reader.ReadNextLine());
            }
        }
    }
}
