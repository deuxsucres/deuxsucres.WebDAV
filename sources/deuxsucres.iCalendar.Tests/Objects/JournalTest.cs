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
    public class JournalTest
    {
        [Fact]
        public void Create()
        {
            var journal = new Journal();
            Assert.Equal("VJOURNAL", journal.Name);
            Assert.Empty(journal.Attachments);
            Assert.Empty(journal.Attendees);
            Assert.Empty(journal.Categories);
            Assert.Empty(journal.Comments);
            Assert.Empty(journal.Contacts);
            Assert.Empty(journal.Exdates);
            Assert.Empty(journal.Exrules);
            Assert.Empty(journal.RequestStatuses);
            Assert.Empty(journal.RelatedTos);
            Assert.Empty(journal.Resources);
            Assert.Empty(journal.RecurDates);
            Assert.Empty(journal.RecurRules);

            Assert.Null(journal.UID);
            Assert.Null(journal.DtStamp);
            Assert.Null(journal.DateStart);
            Assert.Null(journal.Class);
            Assert.Null(journal.Summary);
            Assert.Null(journal.Description);
            Assert.Null(journal.Created);
            Assert.Null(journal.LastModified);
            Assert.Null(journal.Organizer);
            Assert.Null(journal.Sequence);
            Assert.Null(journal.Status);
            Assert.Null(journal.Url);
            Assert.Null(journal.RecurrenceId);
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

                var journal = new Journal()
                {
                    UID = "Journal-ID",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Class = Classes.Public,
                    Summary = "This is a journal entry",
                    Description = "Description of the journal entry",
                    Created = dt,
                    LastModified = dt.AddMinutes(3),
                    Organizer = "mailto:yan@deuxsucres.com",
                    Sequence = 1,
                    Status = JournalStatuses.Final,
                    Url = "http://www.deuxsucres.com",
                    RecurrenceId = dt,
                };
                journal.Attachments.Add(new AttachProperty { UriValue = new Uri("http://www.deuxsucres.com") });
                journal.Attachments.Add(new AttachProperty { UriValue = new Uri("https://tools.ietf.org/html/rfc2425") });
                journal.Attendees.Add(new JournalAttendeeProperty { Value = new Uri("mailto:contact@deuxsucres.com"), Role = Roles.Opt_Participant });
                journal.Categories.Add(new string[] { "test", "journal" });
                journal.Comments.Add("This is a test of journal");
                journal.Contacts.Add("Yanos");
                journal.Contacts.Add("deuxsucres");
                journal.Exdates.Add(new DateTime[] { dt, dt.AddDays(1), dt.AddDays(1.5) });
                journal.Exrules.Add(new iCalendar.Structure.RecurrenceProperty { Value = new Recurrence { ByMonth = new int[] { 2, 5 } } });
                journal.RequestStatuses.Add(new RequestStatusProperty { StatusCode = "1.2", StatusDescription = "Success", ExtraData = "Good success", Language = "EN" });
                journal.RelatedTos.Add(new RelatedToProperty { Value = "Test", RelationType = RelationTypes.Parent });
                journal.Resources.Add("Some resource");
                journal.RecurDates.Add(new RecurDateProperty { DatesValue = new List<DateTime> { dt, dt.AddYears(1) } });
                journal.RecurRules.Add(new RecurRuleProperty { Value = new Recurrence { ByMinute = new List<int> { 12, 17, 32 } } });

                journal.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:VJOURNAL")
                .AppendLine("UID:Journal-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a journal entry")
                .AppendLine("DESCRIPTION:Description of the journal entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:FINAL")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
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
                .AppendLine("END:VJOURNAL")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("BEGIN:VJOURNAL")
                .AppendLine("UID:Journal-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CLASS:PUBLIC")
                .AppendLine("SUMMARY:This is a journal entry")
                .AppendLine("DESCRIPTION:Description of the journal entry")
                .AppendLine("CREATED:20171202T081512")
                .AppendLine("LAST-MODIFIED:20171202T081812")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("SEQUENCE:1")
                .AppendLine("STATUS:FINAL")
                .AppendLine("Other:Other property value")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("RECURRENCE-ID:20171202T081512")
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
                .AppendLine("END:VJOURNAL")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var journal = new Journal();
                journal.Deserialize(reader);

                Assert.Equal(28, journal.PropertyCount);

                Assert.Equal("Journal-ID", journal.UID);
                Assert.Equal(dt, (DateTime)journal.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45), (DateTime)journal.DateStart);
                Assert.Equal("PUBLIC", journal.Class);
                Assert.Equal("This is a journal entry", journal.Summary);
                Assert.Equal("Description of the journal entry", journal.Description);
                Assert.Equal(dt, (DateTime)journal.Created);
                Assert.Equal(dt.AddMinutes(3), (DateTime)journal.LastModified);
                Assert.Equal("mailto:yan@deuxsucres.com", journal.Organizer);
                Assert.Equal(1, (int)journal.Sequence);
                Assert.Equal("FINAL", journal.Status);
                Assert.Equal("http://www.deuxsucres.com/", journal.Url);
                Assert.Equal(dt, (DateTime)journal.RecurrenceId);

                Assert.Equal(2, journal.Attachments.Count);
                Assert.Equal("http://www.deuxsucres.com/", journal.Attachments[0].UriValue.ToString());
                Assert.Equal("https://tools.ietf.org/html/rfc2425", journal.Attachments[1].UriValue.ToString());

                Assert.Single(journal.Attendees);
                Assert.Equal("mailto:contact@deuxsucres.com", journal.Attendees[0].Value.ToString());
                Assert.Equal(Roles.Opt_Participant, (Roles)journal.Attendees[0].Role);

                Assert.Single(journal.Categories);
                Assert.Equal(new string[] { "test", "journal" }, (string[])journal.Categories[0]);

                Assert.Single(journal.Comments);
                Assert.Equal("This is a test of journal", journal.Comments[0]);

                Assert.Equal(2, journal.Contacts.Count);
                Assert.Equal("Yanos", journal.Contacts[0]);
                Assert.Equal("deuxsucres", journal.Contacts[1]);

                Assert.Single(journal.Exdates);
                Assert.Equal(new DateTime[] { dt, dt.AddDays(1), dt.AddDays(1.5) }, (DateTime[])journal.Exdates[0]);

                Assert.Single(journal.Exrules);
                Assert.Equal(new int[] { 2, 5 }, journal.Exrules[0].Value.ByMonth);

                Assert.Single(journal.RequestStatuses);
                Assert.Equal("1.2", journal.RequestStatuses[0].StatusCode);
                Assert.Equal("Success", journal.RequestStatuses[0].StatusDescription);
                Assert.Equal("Good success", journal.RequestStatuses[0].ExtraData);

                Assert.Single(journal.RelatedTos);
                Assert.Equal("Test", journal.RelatedTos[0].Value);
                Assert.Equal("PARENT", journal.RelatedTos[0].RelationType);

                Assert.Single(journal.Resources);
                Assert.Equal("Some resource", journal.Resources[0]);

                Assert.Single(journal.RecurDates);
                Assert.Equal(new DateTime[] { dt, dt.AddYears(1) }, (DateTime[])journal.RecurDates[0]);

                Assert.Single(journal.RecurRules);
                Assert.Equal(new List<int> { 12, 17, 32 }, journal.RecurRules[0].Value.ByMinute);

                Assert.Equal("Other property value", (string)journal.FindProperties<TextProperty>("OTHER").Single());

                Assert.Null(reader.ReadNextLine());
            }
        }

    }
}
