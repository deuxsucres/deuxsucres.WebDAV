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
    public class TextListPropertyTest
    {
        [Fact]
        public void Cast()
        {
            var prop = new TextListProperty();
            Assert.Null(prop.Value);

            string[] values = new string[] { "one", "two", "three" };
            prop = values;
            Assert.NotNull(prop);
            Assert.NotNull(prop.Value);
            Assert.Equal(values, prop.Value);

            string[] str = prop;
            Assert.Equal(values, str);

            values = null;
            prop = values;
            Assert.Null(prop);

            str = prop;
            Assert.Null(str);
        }

        [Fact]
        public void TestToString()
        {
            TextListProperty prop = new string[] { "one", "two", "three" };
            Assert.Equal("one, two, three", prop.ToString());
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                TextListProperty prop = new TextListProperty
                {
                    Name = "test",
                    Value = new List<string> { "one", "two\nAnother '\"text;'", "three" }
                };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("TEST:one,two\\nAnother '\"text\\;',three")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("TEST:one,two\\nAnother '\"text\\;',three")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new TextListProperty();
                prop.Deserialize(reader, reader.ReadNextLine());

                Assert.Equal("TEST", prop.Name);
                Assert.Equal(new string[] { "one", "two\r\nAnother '\"text;'", "three" }, prop.Value);
            }

        }

    }
}
