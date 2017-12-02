using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class AttendeePropertiesTest
    {
        [Fact]
        public void Parameters()
        {
            var prop = new JournalAttendeeProperty();
            Assert.Equal("ATTENDEE", prop.Name);
            Assert.Null(prop.CN);
            Assert.Null(prop.CuType);
            Assert.Null(prop.DelegatedFrom);
            Assert.Null(prop.DelegatedTo);
            Assert.Null(prop.Dir);
            Assert.Null(prop.Language);
            Assert.Null(prop.Member);
            Assert.Null(prop.PartStat);
            Assert.Null(prop.Role);
            Assert.Null(prop.Rsvp);
            Assert.Null(prop.SentBy);
            Assert.Null(prop.Value);

            prop.CN = "CN";
            prop.CuType = CalUserTypes.Individual;
            prop.DelegatedFrom = new string[] { "mailto:yan@deuxsucres.com" };
            prop.DelegatedTo = new string[] { "mailto:yan@deuxsucres.com" };
            prop.Dir = "http://www.deuxsucres.com";
            prop.Language = "fr";
            prop.Member = new List<Uri> { new Uri("mailto:yan@deuxsucres.com"), new Uri("mailto:contact@deuxsucres.com") };
            prop.PartStat = JournalPartStatuses.Accepted;
            prop.Role = Roles.Chair;
            prop.Rsvp = true;
            prop.SentBy = new Uri("mailto:yan@deuxsucres.com");
            prop.Value = new Uri("mailto:yan@deuxsucres.com");

            Assert.Equal("CN", prop.CN);
            Assert.Equal(CalUserTypes.Individual, (CalUserTypes)prop.CuType);
            Assert.Equal(new string[] { "mailto:yan@deuxsucres.com" }, (string[])prop.DelegatedFrom);
            Assert.Equal(new string[] { "mailto:yan@deuxsucres.com" }, (string[])prop.DelegatedTo);
            Assert.Equal("http://www.deuxsucres.com/", prop.Dir);
            Assert.Equal("fr", prop.Language);
            Assert.Equal(new List<Uri> { new Uri("mailto:yan@deuxsucres.com"), new Uri("mailto:contact@deuxsucres.com") }, (Uri[])prop.Member);
            Assert.Equal(JournalPartStatuses.Accepted, (JournalPartStatuses)prop.PartStat);
            Assert.Equal(Roles.Chair, (Roles)prop.Role);
            Assert.True((bool)prop.Rsvp);
            Assert.Equal("mailto:yan@deuxsucres.com", prop.SentBy);
            Assert.Equal(new Uri("mailto:yan@deuxsucres.com"), prop.Value);
        }

        [Fact]
        public void CastJournalAttendee()
        {
            var prop = new JournalAttendeeProperty();
            Assert.Null(prop.Value);
            prop = new Uri("mailto:yan@deuxsucres.com");
            Assert.NotNull(prop?.Value);
            Assert.Equal("mailto:yan@deuxsucres.com", prop.Value.ToString());

            Uri uri = prop;
            Assert.NotNull(uri);
            Assert.Equal("mailto:yan@deuxsucres.com", uri.ToString());

            prop = null;
            uri = prop;
            Assert.Null(uri);

            prop = new JournalAttendeeProperty();
            prop = uri;
            Assert.Null(prop);
        }

        [Fact]
        public void CastEventAttendee()
        {
            var prop = new EventAttendeeProperty();
            Assert.Null(prop.Value);
            prop = new Uri("mailto:yan@deuxsucres.com");
            Assert.NotNull(prop?.Value);
            Assert.Equal("mailto:yan@deuxsucres.com", prop.Value.ToString());

            Uri uri = prop;
            Assert.NotNull(uri);
            Assert.Equal("mailto:yan@deuxsucres.com", uri.ToString());

            prop = null;
            uri = prop;
            Assert.Null(uri);

            prop = new EventAttendeeProperty();
            prop = uri;
            Assert.Null(prop);
        }

        [Fact]
        public void CastTodoAttendee()
        {
            var prop = new TodoAttendeeProperty();
            Assert.Null(prop.Value);
            prop = new Uri("mailto:yan@deuxsucres.com");
            Assert.NotNull(prop?.Value);
            Assert.Equal("mailto:yan@deuxsucres.com", prop.Value.ToString());

            Uri uri = prop;
            Assert.NotNull(uri);
            Assert.Equal("mailto:yan@deuxsucres.com", uri.ToString());

            prop = null;
            uri = prop;
            Assert.Null(uri);

            prop = new TodoAttendeeProperty();
            prop = uri;
            Assert.Null(prop);
        }

        [Fact]
        public void CastFreeBusy()
        {
            var prop = new FreeBusyAttendeeProperty();
            Assert.Null(prop.Value);
            prop = new Uri("mailto:yan@deuxsucres.com");
            Assert.NotNull(prop?.Value);
            Assert.Equal("mailto:yan@deuxsucres.com", prop.Value.ToString());

            Uri uri = prop;
            Assert.NotNull(uri);
            Assert.Equal("mailto:yan@deuxsucres.com", uri.ToString());

            prop = null;
            uri = prop;
            Assert.Null(uri);

            prop = new FreeBusyAttendeeProperty();
            prop = uri;
            Assert.Null(prop);
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            var output = new StringBuilder();

            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new JournalAttendeeProperty()
                {
                    CN = "CN",
                    CuType = CalUserTypes.Individual,
                    DelegatedFrom = new string[] { "mailto:yan@deuxsucres.com" },
                    DelegatedTo = new string[] { "mailto:yan@deuxsucres.com" },
                    Dir = "http://www.deuxsucres.com",
                    Language = "fr",
                    Member = new List<Uri> { new Uri("mailto:yan@deuxsucres.com"), new Uri("mailto:contact@deuxsucres.com") },
                    PartStat = JournalPartStatuses.Accepted,
                    Role = Roles.Chair,
                    Rsvp = true,
                    SentBy = new Uri("mailto:yan@deuxsucres.com"),
                    Value = new Uri("mailto:yan@deuxsucres.com")
                };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("ATTENDEE;CN=CN;CUTYPE=INDIVIDUAL;DELEGATED-FROM=\"mailto:yan@deuxsucres.com\"")
                .AppendLine(" ;DELEGATED-TO=\"mailto:yan@deuxsucres.com\";DIR=\"http://www.deuxsucres.com/\"")
                .AppendLine(" ;LANGUAGE=fr;MEMBER=\"mailto:yan@deuxsucres.com\",\"mailto:contact@deuxsucres")
                .AppendLine(" .com\";PARTSTAT=ACCEPTED;ROLE=CHAIR;RSVP=TRUE;SENT-BY=\"mailto:yan@deuxsucre")
                .AppendLine(" s.com\":mailto:yan@deuxsucres.com")
                .ToString(), output.ToString());

            string input = output.ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = reader.MakeProperty<JournalAttendeeProperty>(reader.ReadNextLine());
                Assert.Equal("CN", prop.CN);
                Assert.Equal(CalUserTypes.Individual, (CalUserTypes)prop.CuType);
                Assert.Equal(new string[] { "mailto:yan@deuxsucres.com" }, (string[])prop.DelegatedFrom);
                Assert.Equal(new string[] { "mailto:yan@deuxsucres.com" }, (string[])prop.DelegatedTo);
                Assert.Equal("http://www.deuxsucres.com/", prop.Dir);
                Assert.Equal("fr", prop.Language);
                Assert.Equal(new List<Uri> { new Uri("mailto:yan@deuxsucres.com"), new Uri("mailto:contact@deuxsucres.com") }, (Uri[])prop.Member);
                Assert.Equal(JournalPartStatuses.Accepted, (JournalPartStatuses)prop.PartStat);
                Assert.Equal(Roles.Chair, (Roles)prop.Role);
                Assert.True((bool)prop.Rsvp);
                Assert.Equal("mailto:yan@deuxsucres.com", prop.SentBy);
                Assert.Equal(new Uri("mailto:yan@deuxsucres.com"), prop.Value);
            }
        }

    }
}
