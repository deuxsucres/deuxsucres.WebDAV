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
    public class TimeZoneComponentTest
    {
        [Fact]
        public void Create()
        {
            var tzc = new TimeZoneComponent();
            Assert.Equal("STANDARD", tzc.Name);
            Assert.False(tzc.IsDayLight);
            Assert.Null(tzc.Calendar);
            Assert.Null(tzc.DateStart);
            Assert.Null(tzc.OffsetFrom);
            Assert.Null(tzc.OffsetTo);
            Assert.Null(tzc.Calendar);
            Assert.Empty(tzc.Comments);
            Assert.Empty(tzc.RecurDates);
            Assert.Empty(tzc.RecurRules);
            Assert.Empty(tzc.TimeZoneNames);

            tzc = TimeZoneComponent.CreateDaylight();
            Assert.Equal("DAYLIGHT", tzc.Name);
            Assert.True(tzc.IsDayLight);
            Assert.Null(tzc.Calendar);
            Assert.Null(tzc.DateStart);
            Assert.Null(tzc.OffsetFrom);
            Assert.Null(tzc.OffsetTo);
            Assert.Null(tzc.Calendar);
            Assert.Empty(tzc.Comments);
            Assert.Empty(tzc.RecurDates);
            Assert.Empty(tzc.RecurRules);
            Assert.Empty(tzc.TimeZoneNames);

            tzc = TimeZoneComponent.CreateStandard();
            Assert.Equal("STANDARD", tzc.Name);
            Assert.False(tzc.IsDayLight);
            Assert.Null(tzc.Calendar);
            Assert.Null(tzc.DateStart);
            Assert.Null(tzc.OffsetFrom);
            Assert.Null(tzc.OffsetTo);
            Assert.Null(tzc.Calendar);
            Assert.Empty(tzc.Comments);
            Assert.Empty(tzc.RecurDates);
            Assert.Empty(tzc.RecurRules);
            Assert.Empty(tzc.TimeZoneNames);
        }

        [Fact]
        public void Properties()
        {
            var tzc = new TimeZoneComponent();
            Assert.Equal("STANDARD", tzc.Name);
            Assert.False(tzc.IsDayLight);
            Assert.Null(tzc.Calendar);
            Assert.Null(tzc.DateStart);
            Assert.Null(tzc.OffsetFrom);
            Assert.Null(tzc.OffsetTo);
            Assert.Empty(tzc.Comments);
            Assert.Empty(tzc.RecurDates);
            Assert.Empty(tzc.RecurRules);
            Assert.Empty(tzc.TimeZoneNames);

            DateTime dts = DateTime.Now;
            TimeSpan off = TimeSpan.FromMinutes(123);
            TimeSpan oft = TimeSpan.FromMinutes(321);
            tzc.IsDayLight = true;
            tzc.DateStart = dts;
            tzc.OffsetFrom = off;
            tzc.OffsetTo = oft;

            Assert.Equal("DAYLIGHT", tzc.Name);
            Assert.True(tzc.IsDayLight);
            Assert.Null(tzc.Calendar);
            Assert.Equal(dts, (DateTime)tzc.DateStart);
            Assert.Equal(off, (TimeSpan)tzc.OffsetFrom);
            Assert.Equal(oft, (TimeSpan)tzc.OffsetTo);
            Assert.Empty(tzc.Comments);
            Assert.Empty(tzc.RecurDates);
            Assert.Empty(tzc.RecurRules);
            Assert.Empty(tzc.TimeZoneNames);
        }

        [Fact]
        public void Serialization()
        {
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
            tzc.AddProperty(new IntegerProperty { Name = "Other", Value = 123 });

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);
                tzc.Serialize(writer);
            }
            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:DAYLIGHT")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("COMMENT:First comment")
                .AppendLine("RDATE:20171126T191119")
                .AppendLine("RRULE:FREQ=SECONDLY;COUNT=5;BYHOUR=2,4,6")
                .AppendLine("TZNAME:Europe/Paris")
                .AppendLine("OTHER:123")
                .AppendLine("END:DAYLIGHT")
                .ToString(), output.ToString());

            string input = output.ToString();
            using(var source=new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                tzc = new TimeZoneComponent();
                tzc.Deserialize(reader);
                Assert.True(tzc.IsDayLight);
                Assert.Equal(8, tzc.PropertyCount);
                Assert.Equal(dts, (DateTime)tzc.DateStart);
                Assert.Equal(off, (TimeSpan)tzc.OffsetFrom);
                Assert.Equal(oft, (TimeSpan)tzc.OffsetTo);
                Assert.Single(tzc.Comments);
                Assert.Single(tzc.RecurDates);
                Assert.Single(tzc.RecurRules);
                Assert.Single(tzc.TimeZoneNames);

                Assert.Null(reader.ReadNextLine());
            }

            input = new StringBuilder()
                .AppendLine("BEGIN:STANDARD")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("COMMENT:First comment")
                .AppendLine("RDATE:20171126T191119")
                .AppendLine("RRULE:FREQ=SECONDLY;COUNT=5;BYHOUR=2,4,6")
                .AppendLine("TZNAME:Europe/Paris")
                .AppendLine("OTHER:123")
                .AppendLine("END:STANDARD")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                tzc = new TimeZoneComponent();
                tzc.Deserialize(reader);
                Assert.False(tzc.IsDayLight);
                Assert.Equal(8, tzc.PropertyCount);
                Assert.Equal(dts, (DateTime)tzc.DateStart);
                Assert.Equal(off, (TimeSpan)tzc.OffsetFrom);
                Assert.Equal(oft, (TimeSpan)tzc.OffsetTo);
                Assert.Single(tzc.Comments);
                Assert.Single(tzc.RecurDates);
                Assert.Single(tzc.RecurRules);
                Assert.Single(tzc.TimeZoneNames);

                Assert.Null(reader.ReadNextLine());
            }

            input = new StringBuilder()
                .AppendLine("BEGIN:TEST")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("COMMENT:First comment")
                .AppendLine("RDATE:20171126T191119")
                .AppendLine("RRULE:FREQ=SECONDLY;COUNT=5;BYHOUR=2,4,6")
                .AppendLine("TZNAME:Europe/Paris")
                .AppendLine("OTHER:123")
                .AppendLine("END:TEST")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                tzc = new TimeZoneComponent();
                tzc.Deserialize(reader);
                Assert.False(tzc.IsDayLight);
                Assert.Equal(0, tzc.PropertyCount);

                Assert.NotNull(reader.ReadNextLine());
            }

            input = new StringBuilder()
                .AppendLine("TEST:TEST")
                .AppendLine("DTSTART:20171126T191119")
                .AppendLine("TZOFFSETFROM:+0203")
                .AppendLine("TZOFFSETTO:+0521")
                .AppendLine("COMMENT:First comment")
                .AppendLine("RDATE:20171126T191119")
                .AppendLine("RRULE:FREQ=SECONDLY;COUNT=5;BYHOUR=2,4,6")
                .AppendLine("TZNAME:Europe/Paris")
                .AppendLine("OTHER:123")
                .AppendLine("TEST:TEST")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                reader.ReadNextLine();

                tzc = new TimeZoneComponent();
                tzc.Deserialize(reader);
                Assert.False(tzc.IsDayLight);
                Assert.Equal(0, tzc.PropertyCount);

                Assert.NotNull(reader.ReadNextLine());
            }
        }
    }
}
