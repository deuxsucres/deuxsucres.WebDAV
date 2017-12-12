using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure.Properties
{
    public class CalAddressPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new CalAddressProperty();
            Assert.Null(prop.Value);
            Assert.Null(prop.Role);
        }

        [Fact]
        public void Cast()
        {
            var prop = new CalAddressProperty();
            Assert.Null(prop.Value);
            Assert.Null((Uri)prop);
            Assert.Null((string)prop);

            prop = "mailto:yan@deuxsucres.com";
            Assert.Equal(new Uri("mailto:yan@deuxsucres.com"), prop.Value);
            Assert.Equal(new Uri("mailto:yan@deuxsucres.com"), (Uri)prop);
            Assert.Equal("mailto:yan@deuxsucres.com", (string)prop);

            prop = new Uri("mailto:yan.grenier@deuxsucres.com");
            Assert.Equal(new Uri("mailto:yan.grenier@deuxsucres.com"), prop.Value);
            Assert.Equal(new Uri("mailto:yan.grenier@deuxsucres.com"), (Uri)prop);
            Assert.Equal("mailto:yan.grenier@deuxsucres.com", (string)prop);
        }

        [Fact]
        public void TestToString()
        {
            var prop = new CalAddressProperty();
            Assert.Null(prop.ToString());

            prop = "mailto:yan@deuxsucres.com";
            Assert.Equal("mailto:yan@deuxsucres.com", prop.ToString());

            prop = new Uri("mailto:yan.grenier@deuxsucres.com");
            Assert.Equal("mailto:yan.grenier@deuxsucres.com", prop.ToString());
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new CalAddressProperty
                {
                    Name = "Organizer",
                    Value = new Uri("mailto:yan@deuxsucres.com"),
                    Role = Roles.Opt_Participant
                };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("ORGANIZER;ROLE=OPT-PARTICIPANT:mailto:yan@deuxsucres.com")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("ORGANIZER;ROLE=Chair:mailto:contact@deuxsucres.com")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);
                
                var prop = new CalAddressProperty();
                prop.Deserialize(reader, reader.ReadNextLine());

                Assert.Equal("ORGANIZER", prop.Name);
                Assert.Equal(new Uri("mailto:contact@deuxsucres.com"), prop.Value);
                Assert.Equal(Roles.Chair, (Roles)prop.Role);
            }

        }

    }
}
