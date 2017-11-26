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
    public class RecurDatePropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new RecurDateProperty();
            Assert.Equal("RDATE", prop.Name);
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.Null(prop.PeriodsValue);
            Assert.Null(prop.TimeZoneID);
        }

        [Fact]
        public void Properties()
        {
            var prop = new RecurDateProperty();
            Assert.Equal("RDATE", prop.Name);
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.Null(prop.PeriodsValue);
            Assert.Null(prop.TimeZoneID);

            prop.PeriodsValue = new List<Period>();
            prop.TimeZoneID = "test";
            Assert.Equal(ValueTypes.Period, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.NotNull(prop.PeriodsValue);
            Assert.Equal("test", prop.TimeZoneID);

            prop.DatesValue = new List<DateTime>();
            prop.TimeZoneID = null;
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.NotNull(prop.DatesValue);
            Assert.Null(prop.PeriodsValue);
            Assert.Null(prop.TimeZoneID);
        }

        [Fact]
        public void Cast()
        {
            RecurDateProperty prop = new RecurDateProperty();
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.Null((DateTime[])prop);
            Assert.Null((Period[])prop);

            prop = new Period[0];
            Assert.Equal(ValueTypes.Period, prop.ValueType);
            Assert.Null((DateTime[])prop);
            Assert.NotNull((Period[])prop);

            prop = new DateTime[0];
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.NotNull((DateTime[])prop);
            Assert.Null((Period[])prop);
        }

        [Fact]
        public void SetValues()
        {
            RecurDateProperty prop = new RecurDateProperty();
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.Null(prop.PeriodsValue);

            var dates = new DateTime[] { DateTime.MinValue, DateTime.Now, DateTime.MaxValue };
            prop.SetAsDate();
            Assert.Equal(ValueTypes.Date, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.Null(prop.PeriodsValue);

            prop.SetAsDateTime(dates);
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.NotNull(prop.DatesValue);
            Assert.Equal(dates, prop.DatesValue);
            Assert.Null(prop.PeriodsValue);

            prop.SetAsPeriod();
            Assert.Equal(ValueTypes.Period, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.Null(prop.PeriodsValue);

            prop.SetAsDate(dates);
            Assert.Equal(ValueTypes.Date, prop.ValueType);
            Assert.NotNull(prop.DatesValue);
            Assert.Equal(dates, prop.DatesValue);
            Assert.Null(prop.PeriodsValue);

            prop.SetAsDateTime();
            Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
            Assert.NotNull(prop.DatesValue);
            Assert.Equal(dates, prop.DatesValue);
            Assert.Null(prop.PeriodsValue);

            var periods = new Period[] { new Period(DateTime.Now, DateTime.Now.AddDays(2)) };
            prop.SetAsPeriod(periods);
            Assert.Equal(ValueTypes.Period, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.NotNull(prop.PeriodsValue);
            Assert.Equal(periods, prop.PeriodsValue);
            prop.SetAsPeriod();
            Assert.Equal(ValueTypes.Period, prop.ValueType);
            Assert.Null(prop.DatesValue);
            Assert.NotNull(prop.PeriodsValue);
            Assert.Equal(periods, prop.PeriodsValue);

        }

        [Fact]
        public void Serialization()
        {
            List<RecurDateProperty> props = new List<RecurDateProperty>();
            var parser = new CalendarParser();
            string input = new StringBuilder()
                .AppendLine("RDATE:19970714T123000Z")
                .AppendLine("RDATE;TZID=America/New_York:19970714T083000")
                .AppendLine("RDATE;VALUE=PERIOD:19960403T020000Z/19960403T040000Z,")
                .AppendLine(" 19960404T010000Z/PT3H")
                .AppendLine("RDATE;VALUE=DATE:19970101,19970120,19970217,19970421,")
                .AppendLine(" 19970526,19970704,19970901,19971014,19971128,19971129,19971225")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var line = reader.ReadNextLine();
                Assert.NotNull(line);
                var prop = reader.MakeProperty<RecurDateProperty>(line);
                props.Add(prop);
                Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
                Assert.Null(prop.TimeZoneID);
                Assert.Equal(new DateTime[] { new DateTime(1997, 7, 14, 12, 30, 0) }, prop.DatesValue);
                Assert.Null(prop.PeriodsValue);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<RecurDateProperty>(line);
                props.Add(prop);
                Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
                Assert.Equal("America/New_York", prop.TimeZoneID);
                Assert.Equal(new DateTime[] { new DateTime(1997, 7, 14, 8, 30, 0) }, prop.DatesValue);
                Assert.Null(prop.PeriodsValue);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<RecurDateProperty>(line);
                props.Add(prop);
                Assert.Equal(ValueTypes.Period, prop.ValueType);
                Assert.Null(prop.TimeZoneID);
                Assert.Null(prop.DatesValue);
                Assert.Equal(new Period[] {
                    new Period(new DateTime(1996, 4, 3, 2, 0, 0, DateTimeKind.Utc), new DateTime(1996, 4, 3, 4, 0, 0, DateTimeKind.Utc)),
                    new Period(new DateTime(1996, 4, 4, 1, 0, 0, DateTimeKind.Utc), TimeSpan.FromHours(3))
                }, prop.PeriodsValue);

                line = reader.ReadNextLine();
                Assert.NotNull(line);
                prop = reader.MakeProperty<RecurDateProperty>(line);
                props.Add(prop);
                Assert.Equal(ValueTypes.Date, prop.ValueType);
                Assert.Null(prop.TimeZoneID);
                Assert.Equal(new DateTime[] {
                    new DateTime(1997, 1, 1, 0,0,0,DateTimeKind.Local), new DateTime(1997, 1, 20, 0,0,0,DateTimeKind.Local),
                    new DateTime(1997, 2, 17, 0,0,0,DateTimeKind.Local), new DateTime(1997, 4, 21, 0,0,0,DateTimeKind.Local),
                    new DateTime(1997, 5, 26, 0,0,0,DateTimeKind.Local), new DateTime(1997, 7, 4, 0,0,0,DateTimeKind.Local),
                    new DateTime(1997, 9, 1, 0,0,0,DateTimeKind.Local), new DateTime(1997, 10, 14, 0,0,0,DateTimeKind.Local),
                    new DateTime(1997, 11, 28, 0,0,0,DateTimeKind.Local), new DateTime(1997, 11, 29, 0,0,0,DateTimeKind.Local),
                    new DateTime(1997, 12, 25, 0,0,0,DateTimeKind.Local)
                }, prop.DatesValue);
                Assert.Null(prop.PeriodsValue);

                line = reader.ReadNextLine();
                Assert.Null(line);

                line = new ContentLine { Name = "RDATE", Value = null };
                line.SetParam("VALUE", "PERIOD");
                prop = reader.MakeProperty<RecurDateProperty>(line);
                Assert.Equal(ValueTypes.Period, prop.ValueType);
                Assert.Null(prop.TimeZoneID);
                Assert.Null(prop.DatesValue);
                Assert.Equal(new Period[0], prop.PeriodsValue);

                line = new ContentLine { Name = "RDATE", Value = null };
                line.SetParam("VALUE", "DATE");
                prop = reader.MakeProperty<RecurDateProperty>(line);
                Assert.Equal(ValueTypes.Date, prop.ValueType);
                Assert.Null(prop.TimeZoneID);
                Assert.Equal(new DateTime[0], prop.DatesValue);
                Assert.Null(prop.PeriodsValue);

                line = new ContentLine { Name = "RDATE", Value = null };
                prop = reader.MakeProperty<RecurDateProperty>(line);
                Assert.Equal(ValueTypes.Date_Time, prop.ValueType);
                Assert.Null(prop.TimeZoneID);
                Assert.Equal(new DateTime[0], prop.DatesValue);
                Assert.Null(prop.PeriodsValue);
            }

            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);
                foreach (var prop in props)
                    prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("RDATE:19970714T123000Z")
                .AppendLine("RDATE;TZID=America/New_York:19970714T083000")
                .AppendLine("RDATE;VALUE=PERIOD:19960403T020000Z/19960403T040000Z,19960404T010000Z/PT3H")
                .AppendLine("RDATE;VALUE=DATE:19970101,19970120,19970217,19970421,19970526,19970704,1997")
                .AppendLine(" 0901,19971014,19971128,19971129,19971225")
                .ToString(), output.ToString());
        }

    }
}
