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
    public class TimeZoneTest
    {
        [Fact]
        public void Create()
        {
            var tz = new TimeZone();
            Assert.Equal("VTIMEZONE", tz.Name);
            Assert.Null(tz.Calendar);
            Assert.Null(tz.TzId);
            Assert.Null(tz.TzUrl);
            Assert.Null(tz.LastModified);
            Assert.Empty(tz.ZoneComponents);
        }

        [Fact]
        public void Properties()
        {
            var tz = new TimeZone();
            Assert.Equal("VTIMEZONE", tz.Name);
            Assert.Null(tz.TzId);
            Assert.Null(tz.TzUrl);
            Assert.Null(tz.LastModified);

            DateTime dt = DateTime.Now;

            tz.TzId = "Europe/Paris";
            tz.TzUrl = "http://www.deuxsucres.com";
            tz.LastModified = dt;

            Assert.Equal("Europe/Paris", tz.TzId);
            Assert.Equal("http://www.deuxsucres.com/", tz.TzUrl);
            Assert.Equal(dt, (DateTime)tz.LastModified);
        }

        [Fact]
        public void Serialization()
        {
            DateTime lastm = new DateTime(2017, 11, 27, 19, 37, 19);

            var tz = new TimeZone();
            tz.TzId = "Europe/Paris";
            tz.TzUrl = "http://www.deuxsucres.com";
            tz.LastModified = lastm;

            DateTime dts = new DateTime(2017, 11, 26, 19, 11, 19);
            TimeSpan off = TimeSpan.FromMinutes(123);
            TimeSpan oft = TimeSpan.FromMinutes(321);
            var tzc = new TimeZoneComponent()
            {
                IsDayLight = true,
                DateStart = dts,
                OffsetFrom = off,
                OffsetTo = oft
            };
            tzc.Comments.Add("First comment");
            tzc.RecurDates.Add(new RecurDateProperty { DatesValue = new List<DateTime> { dts } });
            tzc.RecurRules.Add(new RecurRuleProperty { Value = new Recurrence { Count = 5, ByHour = new List<int> { 2, 4, 6 } } });
            tzc.TimeZoneNames.Add("Europe/Paris");
            tz.ZoneComponents.Add(tzc);
            tz.AddProperty(new IntegerProperty { Name = "Other", Value = 123 });

            tzc = new TimeZoneComponent()
            {
                IsDayLight = false,
                DateStart = dts,
                OffsetFrom = off,
                OffsetTo = oft
            };
            tz.ZoneComponents.Add(tzc);

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);
                tz.Serialize(writer);
            }
            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:VTIMEZONE")
                .AppendLine("TZID:Europe/Paris")
                .AppendLine("TZURL:http://www.deuxsucres.com/")
                .AppendLine("LAST-MODIFIED:20171127T193719")
                .AppendLine("OTHER:123")
                .AppendLine("BEGIN:DAYLIGHT")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("COMMENT:First comment")
                .AppendLine("RDATE:20171126T191119")
                .AppendLine("RRULE:FREQ=SECONDLY;COUNT=5;BYHOUR=2,4,6")
                .AppendLine("TZNAME:Europe/Paris")
                .AppendLine("END:DAYLIGHT")
                .AppendLine("BEGIN:STANDARD")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("END:STANDARD")
                .AppendLine("END:VTIMEZONE")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("BEGIN:VTIMEZONE")
                .AppendLine("TZID:Europe/Paris")
                .AppendLine("TZURL:http://www.deuxsucres.com/")
                .AppendLine("BEGIN:DAYLIGHT")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("COMMENT:First comment")
                .AppendLine("RDATE:20171126T191119")
                .AppendLine("RRULE:FREQ=SECONDLY;COUNT=5;BYHOUR=2,4,6")
                .AppendLine("TZNAME:Europe/Paris")
                .AppendLine("END:DAYLIGHT")
                .AppendLine("LAST-MODIFIED:20171127T193719")
                .AppendLine("BEGIN:STANDARD")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("END:STANDARD")
                .AppendLine("OTHER:123")
                .AppendLine("BEGIN:VTIMEZONE")
                .AppendLine("TZID:GMT")
                .AppendLine("END:VTIMEZONE")
                .AppendLine("END:VTIMEZONE")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                tz = new TimeZone();
                tz.Deserialize(reader);
                Assert.Equal("Europe/Paris", tz.TzId);
                Assert.Equal("http://www.deuxsucres.com/", tz.TzUrl);
                Assert.Equal(lastm, (DateTime)tz.LastModified);
                Assert.Equal(4, tz.PropertyCount);
                Assert.Equal("123", tz.GetProperty<TextProperty>("OTHER"));
                Assert.Single(tz.ExtraComponents);
                Assert.Equal(2, tz.ZoneComponents.Count);

                Assert.Null(reader.ReadNextLine());
            }
        }

    }
}
