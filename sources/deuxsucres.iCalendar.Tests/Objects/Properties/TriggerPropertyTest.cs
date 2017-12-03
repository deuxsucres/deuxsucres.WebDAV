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

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class TriggerPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new TriggerProperty();
            Assert.Equal("TRIGGER", prop.Name);
            Assert.Null(prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);
        }

        [Fact]
        public void Properties()
        {
            var prop = new TriggerProperty();
            Assert.Equal("TRIGGER", prop.Name);
            Assert.Null(prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);
            Assert.Null(prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

            DateTime dt = new DateTime(2017, 12, 3, 8, 18, 12);
            TimeSpan ts = TimeSpan.FromDays(12.3456789);

            prop.AbsoluteValue = dt;
            Assert.Equal(dt, prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);
            Assert.Equal(ValueTypes.Date_Time, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

            prop.AbsoluteValue = dt.Date;
            Assert.Equal(dt.Date, prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);
            Assert.Equal(ValueTypes.Date, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

            prop.RelativeValue = ts;
            Assert.Null(prop.AbsoluteValue);
            Assert.Equal(ts, prop.RelativeValue);
            Assert.Equal(ValueTypes.Duration, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));
        }

        [Fact]
        public void Cast()
        {
            var prop = new TriggerProperty();
            Assert.Null(prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);

            DateTime dt = new DateTime(2017, 12, 3, 8, 18, 12);
            TimeSpan ts = TimeSpan.FromDays(12.3456789);

            prop = dt;
            Assert.Equal(dt, prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);
            Assert.Equal(dt, (DateTime?)prop);
            Assert.Null((TimeSpan?)prop);

            prop = dt.Date;
            Assert.Equal(dt.Date, prop.AbsoluteValue);
            Assert.Null(prop.RelativeValue);
            Assert.Equal(dt.Date, (DateTime?)prop);
            Assert.Null((TimeSpan?)prop);

            prop = ts;
            Assert.Null(prop.AbsoluteValue);
            Assert.Equal(ts, prop.RelativeValue);
            Assert.Null((DateTime?)prop);
            Assert.Equal(ts, (TimeSpan?)prop);

            prop = (DateTime?)null;
            Assert.Null(prop);

            prop = (TimeSpan?)null;
            Assert.Null(prop);
        }

        [Fact]
        public void Serialization()
        {
            DateTime dt = new DateTime(2017, 12, 3, 8, 18, 12);
            TimeSpan ts = TimeSpan.FromDays(12.3456789);

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                TriggerProperty prop = dt;
                prop.Serialize(writer);

                prop = dt.Date;
                prop.Serialize(writer);

                prop = ts;
                prop.Serialize(writer);

                prop = dt;
                prop.RemoveParameter(Constants.VALUE);
                prop.Serialize(writer);

                prop = ts;
                prop.RemoveParameter(Constants.VALUE);
                prop.Serialize(writer);

                prop.AbsoluteValue = null;
                prop.RelativeValue = null;
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171203T081812")
                .AppendLine("TRIGGER;VALUE=DATE:20171203")
                .AppendLine("TRIGGER;VALUE=DURATION:P12DT8H17M46S")
                .AppendLine("TRIGGER:20171203T081812")
                .AppendLine("TRIGGER:P12DT8H17M46S")
                .AppendLine("TRIGGER:")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("TRIGGER;VALUE=DATE-TIME:20171203T081812")
                .AppendLine("TRIGGER;VALUE=DATE:20171203")
                .AppendLine("TRIGGER;VALUE=TEXT:20171203")
                .AppendLine("TRIGGER;VALUE=DURATION:P12DT8H17M46S")
                .AppendLine("TRIGGER:20171203T081812")
                .AppendLine("TRIGGER:P12DT8H17M46S")
                .AppendLine("TRIGGER:Test")
                .AppendLine("TRIGGER:")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new TriggerProperty();
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("TRIGGER", prop.Name);
                Assert.Equal(dt, prop.AbsoluteValue);
                Assert.Null(prop.RelativeValue);
                Assert.Equal(ValueTypes.Date_Time, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(dt.Date, prop.AbsoluteValue);
                Assert.Null(prop.RelativeValue);
                Assert.Equal(ValueTypes.Date, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(dt.Date, prop.AbsoluteValue);
                Assert.Null(prop.RelativeValue);
                Assert.Equal(ValueTypes.Text, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                TimeSpan tsd = new TimeSpan(12, 8, 17, 46);
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Null(prop.AbsoluteValue);
                Assert.Equal(tsd, prop.RelativeValue);
                Assert.Equal(ValueTypes.Duration, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(dt, prop.AbsoluteValue);
                Assert.Null(prop.RelativeValue);
                Assert.Equal(ValueTypes.Date_Time, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Null(prop.AbsoluteValue);
                Assert.Equal(tsd, prop.RelativeValue);
                Assert.Equal(ValueTypes.Duration, (ValueTypes)prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Null(prop.AbsoluteValue);
                Assert.Null(prop.RelativeValue);
                Assert.Null(prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Null(prop.AbsoluteValue);
                Assert.Null(prop.RelativeValue);
                Assert.Null(prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE));

            }

        }

    }
}
