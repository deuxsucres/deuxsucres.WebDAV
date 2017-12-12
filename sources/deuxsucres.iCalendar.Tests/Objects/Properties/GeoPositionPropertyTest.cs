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
    public class GeoPositionPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new GeoPositionProperty();
            Assert.Equal("GEO", prop.Name);
            Assert.Equal(0, prop.Latitude);
            Assert.Equal(0, prop.Longitude);
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                GeoPositionProperty prop = new GeoPositionProperty
                {
                    Latitude = 12.34,
                    Longitude = -98.76
                };
                prop.Serialize(writer);

                prop = new GeoPositionProperty
                {
                    Latitude = -12.34,
                    Longitude = 98.76
                };
                prop.Serialize(writer);

                prop = new GeoPositionProperty();
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("GEO:12.34;-98.76")
                .AppendLine("GEO:-12.34;98.76")
                .AppendLine("GEO:0;0")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("GEO:12.34;-98.76")
                .AppendLine("GEO:-12.34;98.76")
                .AppendLine("GEO:0;0")
                .AppendLine("GEO:0;Test")
                .AppendLine("GEO:Test;0")
                .AppendLine("GEO:0")
                .AppendLine("GEO:")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new GeoPositionProperty();
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("GEO", prop.Name);
                Assert.Equal(12.34, prop.Latitude);
                Assert.Equal(-98.76, prop.Longitude);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(-12.34, prop.Latitude);
                Assert.Equal(98.76, prop.Longitude);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(0, prop.Latitude);
                Assert.Equal(0, prop.Longitude);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(0, prop.Latitude);
                Assert.Equal(0, prop.Longitude);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(0, prop.Latitude);
                Assert.Equal(0, prop.Longitude);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(0, prop.Latitude);
                Assert.Equal(0, prop.Longitude);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(0, prop.Latitude);
                Assert.Equal(0, prop.Longitude);

                Assert.Null(reader.ReadNextLine());
            }

        }
    }
}
