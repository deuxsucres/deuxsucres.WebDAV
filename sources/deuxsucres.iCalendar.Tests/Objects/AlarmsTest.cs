using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects
{
    public class AlarmsTest
    {
        [Fact]
        public void Properties()
        {
            var alarm = new TodoAlarm();
            Assert.Equal("VALARM", alarm.Name);
            Assert.Null(alarm.Action);
            Assert.Null(alarm.Trigger);
            Assert.Null(alarm.Duration);
            Assert.Null(alarm.Repeat);
            Assert.Null(alarm.Description);
            Assert.Null(alarm.Summary);
            Assert.Empty(alarm.Attachments);
            Assert.Empty(alarm.Attendees);
            Assert.Empty(alarm.GetAttendees());

            DateTime dt = DateTime.Now;
            TimeSpan duration = TimeSpan.FromMinutes(5);

            alarm.Action = AlarmActions.Audio;
            alarm.Trigger = dt;
            alarm.Duration = duration;
            alarm.Repeat = 2;
            alarm.Description = "It's an alarm";
            alarm.Summary = "An alarm";
            alarm.Attachments.Add(new AttachProperty { UriValue = new Uri("http://www.deuxsucres.com") });
            alarm.Attendees.Add(new TodoAttendeeProperty { Value = new Uri("mailto:yan@deuxsucres.com") });

            Assert.Equal("AUDIO", alarm.Action);
            Assert.Equal(dt, (DateTime)alarm.Trigger);
            Assert.Equal(duration, (TimeSpan)alarm.Duration);
            Assert.Equal(2, (int)alarm.Repeat);
            Assert.Equal("It's an alarm", alarm.Description);
            Assert.Equal("An alarm", alarm.Summary);
            Assert.Single(alarm.Attachments);
            Assert.Single(alarm.Attendees);
            Assert.Single(alarm.GetAttendees());
        }

        [Fact]
        public void Serialization()
        {
            DateTime dt = new DateTime(2017, 12, 2, 8, 15, 12);
            TimeSpan duration = TimeSpan.FromMinutes(5);

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var alarm = new TodoAlarm()
                {
                    Action = AlarmActions.Audio,
                    Trigger = dt,
                    Duration = duration,
                    Repeat = 2,
                    Description = "It's an alarm",
                    Summary = "An alarm"
                };
                alarm.Attachments.Add(new AttachProperty { UriValue = new Uri("http://www.deuxsucres.com") });
                alarm.Attendees.Add(new TodoAttendeeProperty { Value = new Uri("mailto:yan@deuxsucres.com"), PartStat = TodoPartStatuses.Needs_Action });

                alarm.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:VALARM")
                .AppendLine("ACTION:AUDIO")
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171202T081512")
                .AppendLine("DURATION:PT5M")
                .AppendLine("REPEAT:2")
                .AppendLine("DESCRIPTION:It's an alarm")
                .AppendLine("SUMMARY:An alarm")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTENDEE;PARTSTAT=NEEDS-ACTION:mailto:yan@deuxsucres.com")
                .AppendLine("END:VALARM")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("BEGIN:VALARM")
                .AppendLine("ACTION:AUDIO")
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171202T081512")
                .AppendLine("DURATION:PT5M")
                .AppendLine("REPEAT:2")
                .AppendLine("DESCRIPTION:It's an alarm")
                .AppendLine("SUMMARY:An alarm")
                .AppendLine("OTHER:Other property")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTENDEE;PARTSTAT=NEEDS-ACTION:mailto:yan@deuxsucres.com")
                .AppendLine("END:VALARM")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                var alarm = new TodoAlarm();
                alarm.Deserialize(reader);

                Assert.Equal(9, alarm.PropertyCount);

                Assert.Equal(AlarmActions.Audio, (AlarmActions)alarm.Action);
                Assert.Equal(dt, (DateTime)alarm.Trigger);
                Assert.Equal(duration, (TimeSpan)alarm.Duration);
                Assert.Equal(2, (int)alarm.Repeat);
                Assert.Equal("It's an alarm", (string)alarm.Description);
                Assert.Equal("An alarm", (string)alarm.Summary);

                Assert.Equal(1, alarm.Attachments.Count);
                Assert.Equal("http://www.deuxsucres.com/", alarm.Attachments[0].UriValue.ToString());

                Assert.Single(alarm.Attendees);
                Assert.Equal("mailto:yan@deuxsucres.com", alarm.Attendees[0].Value.ToString());
                Assert.Equal(TodoPartStatuses.Needs_Action, (TodoPartStatuses)alarm.Attendees[0].PartStat);

                Assert.Null(reader.ReadNextLine());
            }
        }
    }
}
