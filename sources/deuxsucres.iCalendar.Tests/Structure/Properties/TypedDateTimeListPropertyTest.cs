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
    public class TypedDateTimeListPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new TypedDateTimeListProperty();
            Assert.Null(prop.Value);
            Assert.False(prop.IsDate);
            Assert.Null(prop.TimeZoneID);
        }

        [Fact]
        public void Cast()
        {
            DateTime now = DateTime.Now;
            var dts = new DateTime[] { now, now.AddDays(1), now.AddHours(12.34) };
            var prop = new TypedDateTimeListProperty()
            {
                Value = dts.ToList()
            };
            DateTime[] dta = prop;
            Assert.Equal(dts, dta);

            prop = null;
            dta = prop;
            Assert.Null(dta);

            dta = dts;
            prop = dta;
            Assert.Equal(dts, prop.Value);
        }

        [Fact]
        public void Properties()
        {
            DateTime now = DateTime.Now;
            var dts = new DateTime[] { now, now.AddDays(1), now.AddHours(12.34) };
            var prop = new TypedDateTimeListProperty()
            {
                Value = dts.ToList(),
                TimeZoneID = "TimeZone"
            };
            Assert.Equal(dts, prop.Value);
            Assert.Equal("TimeZone", prop.TimeZoneID);
            Assert.False(prop.IsDate);
        }

        [Fact]
        public void TestToString()
        {
            DateTime now = DateTime.Now;
            var dts = new DateTime[] { now, now.AddDays(1), now.AddHours(12.34) };
            var prop = new TypedDateTimeListProperty()
            {
                Value = dts.ToList(),
                TimeZoneID = "TimeZone"
            };
            Assert.Equal(string.Join(", ", dts.Select(d => d.ToString())), prop.ToString());
            prop.Value = null;
            Assert.Null(prop.ToString());
        }

        [Fact]
        public void SetAs()
        {
            DateTime now = DateTime.Now;
            var dts = new DateTime[] { now, now.AddDays(1), now.AddHours(12.34) };
            var prop = new TypedDateTimeListProperty();
            prop.SetAsDateTime(dts);
            Assert.Equal(dts, prop.Value);
            Assert.False(prop.IsDate);

            prop.SetAsDate();
            Assert.Equal(dts.Select(d => d.Date), prop.Value);
            Assert.True(prop.IsDate);

            prop.SetAsDateTime();
            Assert.Equal(dts.Select(d => d.Date), prop.Value);
            Assert.False(prop.IsDate);

            prop.SetAsDate(dts);
            Assert.Equal(dts.Select(d => d.Date), prop.Value);
            Assert.True(prop.IsDate);
        }

        [Fact]
        public void Serialization()
        {
            DateTime dt = new DateTime(2017, 11, 26, 18, 25, 12);
            var dts = new DateTime[] { dt, dt.AddDays(1), dt.AddHours(12.34) };
            var prop = new TypedDateTimeListProperty()
            {
                Name = "MyDate",
                Value = dts.ToList(),
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
                .AppendLine("MYDATE;TZID=TimeZone:20171126T182512,20171127T182512,20171127T064536")
                .AppendLine("MYDATE;TZID=TimeZone;VALUE=DATE:20171126,20171127,20171127")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("MYDATE;TZID=TimeZone:20171126T182512,20171127T182512,20171127T064536")
                .AppendLine("MYDATE;VALUE=DATE:20171126,20171127,20171127")
                .AppendLine("Other:Test")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<TypedDateTimeListProperty>(line);
                Assert.Equal("MYDATE", prop.Name);
                Assert.Equal(dts, prop.Value);
                Assert.False(prop.IsDate);
                Assert.Equal("TimeZone", prop.TimeZoneID);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<TypedDateTimeListProperty>(line);
                Assert.Equal("MYDATE", prop.Name);
                Assert.Equal(dts.Select(d => d.Date), prop.Value);
                Assert.True(prop.IsDate);
                Assert.Null(prop.TimeZoneID);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<TypedDateTimeListProperty>(line);
                Assert.NotNull(prop.Value);
                Assert.Empty(prop.Value);

                line = reader.ReadNextLine();
                Assert.Null(line);
            }
        }

    }
}
