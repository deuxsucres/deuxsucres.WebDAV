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
    public class FreeBusyTest
    {
        [Fact]
        public void Create()
        {
            var fbusy = new FreeBusy();
            Assert.Equal("VFREEBUSY", fbusy.Name);
            Assert.Empty(fbusy.Attendees);
            Assert.Empty(fbusy.Comments);
            Assert.Empty(fbusy.FreeBusies);
            Assert.Empty(fbusy.RequestStatuses);

            Assert.Null(fbusy.UID);
            Assert.Null(fbusy.Contact);
            Assert.Null(fbusy.DtStamp);
            Assert.Null(fbusy.DateStart);
            Assert.Null(fbusy.DateEnd);
            Assert.Null(fbusy.Organizer);
            Assert.Null(fbusy.Url);
            Assert.Null(fbusy.Duration);
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

                var fbusy = new FreeBusy()
                {
                    UID = "FreeBusy-ID",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Contact = "This the contact",
                    Organizer = "mailto:yan@deuxsucres.com",
                    Url = "http://www.deuxsucres.com",
                    DateEnd = dt.AddDays(10)
                };
                fbusy.Attendees.Add(new FreeBusyAttendeeProperty { Value = new Uri("mailto:contact@deuxsucres.com"), Role = Roles.Opt_Participant });
                fbusy.Comments.Add("This is a test of free/busy");
                fbusy.RequestStatuses.Add(new RequestStatusProperty { StatusCode = "1.2", StatusDescription = "Success", ExtraData = "Good success", Language = "EN" });
                fbusy.FreeBusies.Add(new FreeBusyProperty { FreeBusyType = FreeBusyTypes.Busy, Periods = new Period[] { new Period(dt, TimeSpan.FromMinutes(5)) } });
                fbusy.Serialize(writer);

                fbusy = new FreeBusy()
                {
                    UID = "FreeBusy-ID-2",
                    DtStamp = dt,
                    DateStart = dt.AddMinutes(123.45),
                    Contact = "This the contact",
                    Organizer = "mailto:yan@deuxsucres.com",
                    Url = "http://www.deuxsucres.com",
                    Duration = TimeSpan.FromDays(2.3),
                };
                fbusy.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:VFREEBUSY")
                .AppendLine("UID:FreeBusy-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CONTACT:This the contact")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("DTEND:20171212T081512")
                .AppendLine("ATTENDEE;ROLE=OPT-PARTICIPANT:mailto:contact@deuxsucres.com")
                .AppendLine("COMMENT:This is a test of free/busy")
                .AppendLine("REQUEST-STATUS;LANGUAGE=EN:1.2;Success;Good success")
                .AppendLine("FREEBUSY;FBTYPE=BUSY:20171202T081512/PT5M")
                .AppendLine("END:VFREEBUSY")
                .AppendLine("BEGIN:VFREEBUSY")
                .AppendLine("UID:FreeBusy-ID-2")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CONTACT:This the contact")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("DURATION:P2DT7H12M")
                .AppendLine("END:VFREEBUSY")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("BEGIN:VFREEBUSY")
                .AppendLine("UID:FreeBusy-ID")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CONTACT:This the contact")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("DTEND:20171212T081512")
                .AppendLine("ATTENDEE;ROLE=OPT-PARTICIPANT:mailto:contact@deuxsucres.com")
                .AppendLine("Other:Other property value")
                .AppendLine("COMMENT:This is a test of free/busy")
                .AppendLine("REQUEST-STATUS;LANGUAGE=EN:1.2;Success;Good success")
                .AppendLine("FREEBUSY;FBTYPE=BUSY:20171202T081512/PT5M")
                .AppendLine("END:VFREEBUSY")
                .AppendLine("BEGIN:VFREEBUSY")
                .AppendLine("UID:FreeBusy-ID-2")
                .AppendLine("DTSTAMP:20171202T081512")
                .AppendLine("DTSTART:20171202T101839")
                .AppendLine("CONTACT:This the contact")
                .AppendLine("ORGANIZER:mailto:yan@deuxsucres.com")
                .AppendLine("URL:http://www.deuxsucres.com/")
                .AppendLine("DURATION:P2DT7H12M")
                .AppendLine("END:VFREEBUSY")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var fbusy = new FreeBusy();
                fbusy.Deserialize(reader);

                Assert.Equal(12, fbusy.PropertyCount);

                Assert.Equal("FreeBusy-ID", fbusy.UID);
                Assert.Equal(dt, (DateTime)fbusy.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45), (DateTime)fbusy.DateStart);
                Assert.Equal("This the contact", fbusy.Contact);
                Assert.Equal("mailto:yan@deuxsucres.com", fbusy.Organizer);
                Assert.Equal("http://www.deuxsucres.com/", fbusy.Url);
                Assert.Null(fbusy.Duration);
                Assert.Equal(dt.AddDays(10), (DateTime)fbusy.DateEnd);

                Assert.Single(fbusy.Attendees);
                Assert.Equal("mailto:contact@deuxsucres.com", fbusy.Attendees[0].Value.ToString());
                Assert.Equal(Roles.Opt_Participant, (Roles)fbusy.Attendees[0].Role);

                Assert.Single(fbusy.Comments);
                Assert.Equal("This is a test of free/busy", fbusy.Comments[0]);

                Assert.Single(fbusy.RequestStatuses);
                Assert.Equal("1.2", fbusy.RequestStatuses[0].StatusCode);
                Assert.Equal("Success", fbusy.RequestStatuses[0].StatusDescription);
                Assert.Equal("Good success", fbusy.RequestStatuses[0].ExtraData);

                Assert.Single(fbusy.FreeBusies);
                Assert.Equal(FreeBusyTypes.Busy, (FreeBusyTypes)fbusy.FreeBusies[0].FreeBusyType);

                Assert.Equal("Other property value", (string)fbusy.FindProperties<TextProperty>("OTHER").Single());

                reader.ReadNextLine();

                fbusy = new FreeBusy();
                fbusy.Deserialize(reader);

                Assert.Equal(7, fbusy.PropertyCount);

                Assert.Equal("FreeBusy-ID-2", fbusy.UID);
                Assert.Equal(dt, (DateTime)fbusy.DtStamp);
                Assert.Equal(dt.AddMinutes(123.45), (DateTime)fbusy.DateStart);
                Assert.Equal("This the contact", fbusy.Contact);
                Assert.Equal("mailto:yan@deuxsucres.com", fbusy.Organizer);
                Assert.Equal("http://www.deuxsucres.com/", fbusy.Url);
                Assert.Equal(TimeSpan.FromDays(2.3), (TimeSpan)fbusy.Duration);
                Assert.Null(fbusy.DateEnd);

                Assert.Null(reader.ReadNextLine());
            }
        }
    }
}
