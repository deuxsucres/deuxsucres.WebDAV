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
    public class VersionPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new VersionProperty();
            Assert.Null(prop.Value);
            Assert.Null(prop.MinVersion);
        }

        [Fact]
        public void Cast()
        {
            var prop = new VersionProperty();
            Assert.Null((string)prop);
            prop = "Test";
            Assert.Equal("Test", (string)prop);
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                VersionProperty prop = new VersionProperty();
                prop.Serialize(writer);

                prop.Value="2.0";
                prop.MinVersion = "1.1";
                prop.Serialize(writer);

                prop.Value = "2.0";
                prop.MinVersion = null;
                prop.Serialize(writer);

                prop.Value = null;
                prop.MinVersion = "1.1";
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("VERSION:")
                .AppendLine("VERSION:1.1;2.0")
                .AppendLine("VERSION:2.0")
                .AppendLine("VERSION:1.1")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("VERSION:")
                .AppendLine("VERSION:1.1;2.0")
                .AppendLine("VERSION:2.0")
                .AppendLine("VERSION:1.1")
                .AppendLine("VERSION:;2.0")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new VersionProperty();
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal(string.Empty, prop.Value);
                Assert.Null(prop.MinVersion);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("2.0", prop.Value);
                Assert.Equal("1.1", prop.MinVersion);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("2.0", prop.Value);
                Assert.Null(prop.MinVersion);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("1.1", prop.Value);
                Assert.Null(prop.MinVersion);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("2.0", prop.Value);
                Assert.Equal(string.Empty, prop.MinVersion);

                Assert.Null(reader.ReadNextLine());

                prop.Deserialize(reader, new ContentLine { });
                Assert.Null(prop.Value);
                Assert.Null(prop.MinVersion);

            }
        }

    }
}
