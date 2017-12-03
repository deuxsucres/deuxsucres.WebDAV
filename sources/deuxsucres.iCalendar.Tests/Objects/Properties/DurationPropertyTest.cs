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
    public class DurationPropertyTest
    {
        [Fact]
        public void Cast()
        {
            TimeSpan ts = TimeSpan.FromDays(12.3456789);
            var prop = new DurationProperty();
            Assert.Equal(TimeSpan.Zero, prop.Value);
            Assert.Equal(TimeSpan.Zero, (TimeSpan)prop);
            Assert.Equal(TimeSpan.Zero, (TimeSpan?)prop);

            prop = ts;
            Assert.Equal(ts, prop.Value);
            Assert.Equal(ts, (TimeSpan)prop);
            Assert.Equal(ts, (TimeSpan?)prop);

            prop = (TimeSpan?)null;
            Assert.Null(prop);
            Assert.Equal(TimeSpan.Zero, (TimeSpan)prop);
            Assert.Null((TimeSpan?)prop);
        }

        [Fact]
        public void Serialization()
        {
            TimeSpan ts = TimeSpan.FromDays(12.3456789);

            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new DurationProperty() { Value = ts };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("DURATION:P12DT8H17M46S")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("DURATION:P12DT8H17M46S")
                .AppendLine("DURATION:Test")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new DurationProperty() { Value = ts };
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(new TimeSpan(12, 08, 17, 46), prop.Value);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(TimeSpan.Zero, prop.Value);
            }
        }
    }

}
