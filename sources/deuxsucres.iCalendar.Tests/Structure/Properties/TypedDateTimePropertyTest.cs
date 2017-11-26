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

namespace deuxsucres.iCalendar.Tests.Structure.Properties
{
    public class TypedDateTimePropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new TypedDateTimeProperty();
            Assert.Equal(DateTime.MinValue, prop.Value);
            Assert.False(prop.IsDate);
            Assert.Null(prop.TimeZoneID);
        }

        [Fact]
        public void Cast()
        {
            var prop = new TypedDateTimeProperty();
            DateTime dt = prop;
            DateTime? dtn = prop;
            Assert.Equal(DateTime.MinValue, dt);
            Assert.Equal(DateTime.MinValue, dtn);

            prop = null;
            dt = prop;
            dtn = prop;
            Assert.Equal(DateTime.MinValue, dt);
            Assert.Null(dtn);

            DateTime now = DateTime.Now;
            prop = new TypedDateTimeProperty { Value = now };
            dt = prop;
            dtn = prop;
            Assert.Equal(now, dt);
            Assert.Equal(now, dtn);

            dt = now;
            prop = dt;
            Assert.Equal(dt, prop.Value);

            dtn = now;
            prop = dtn;
            Assert.Equal(dt, prop.Value);

            dtn = null;
            prop = dtn;
            Assert.Null(prop);
        }

        [Fact]
        public void Properties()
        {
            DateTime now = DateTime.Now;
            var prop = new TypedDateTimeProperty()
            {
                Value = now,
                TimeZoneID = "TimeZone"
            };
            Assert.Equal(now, prop.Value);
            Assert.Equal("TimeZone", prop.TimeZoneID);
            Assert.False(prop.IsDate);

            prop.Value = now.Date;
            prop.TimeZoneID = null;
            Assert.Equal(now.Date, prop.Value);
            Assert.Null(prop.TimeZoneID);
            Assert.True(prop.IsDate);
        }

        [Fact]
        public void SetAs()
        {
            DateTime now = DateTime.Now;
            var prop = new TypedDateTimeProperty();
            prop.SetAsDateTime(now);
            Assert.Equal(now, prop.Value);
            Assert.False(prop.IsDate);

            prop.SetAsDate();
            Assert.Equal(now.Date, prop.Value);
            Assert.True(prop.IsDate);

            prop.SetAsDateTime();
            Assert.Equal(now.Date, prop.Value);
            Assert.False(prop.IsDate);

            prop.SetAsDate(now.AddDays(-1));
            Assert.Equal(now.Date.AddDays(-1), prop.Value);
            Assert.True(prop.IsDate);
        }

        [Fact]
        public void Serialization()
        {
            DateTime dt = new DateTime(2017, 11, 26, 18, 25, 12);
            var prop = new TypedDateTimeProperty()
            {
                Name = "MyDate",
                Value = dt,
                TimeZoneID = "TimeZone"
            };

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);
                prop.Serialize(writer);
                prop.SetAsDate();
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("MYDATE;TZID=TimeZone:20171126T182512")
                .AppendLine("MYDATE;TZID=TimeZone;VALUE=DATE:20171126")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("MYDATE;TZID=TimeZone:20171126T182512")
                .AppendLine("MYDATE;VALUE=DATE:20171126")
                .AppendLine("Other:Test")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<TypedDateTimeProperty>(line);
                Assert.Equal("MYDATE", prop.Name);
                Assert.Equal(dt, prop.Value);
                Assert.False(prop.IsDate);
                Assert.Equal("TimeZone", prop.TimeZoneID);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<TypedDateTimeProperty>(line);
                Assert.Equal("MYDATE", prop.Name);
                Assert.Equal(dt.Date, prop.Value);
                Assert.True(prop.IsDate);
                Assert.Null(prop.TimeZoneID);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<TypedDateTimeProperty>(line);
                Assert.Equal(DateTime.MinValue, prop.Value);

                line = reader.ReadNextLine();
                Assert.Null(line);
            }

        }
    }
}
