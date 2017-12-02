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
    public class RelatedToPropertyTest
    {
        [Fact]
        public void Cast()
        {
            RelatedToProperty prop = new RelatedToProperty();
            Assert.Null(prop.Value);

            prop = "Test";
            Assert.Equal("Test", prop);

            prop = (string)null;
            Assert.Null(prop);
            Assert.Null((string)prop);
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new RelatedToProperty
                {
                    Value = "Test",
                    RelationType = RelationTypes.Parent
                };
                prop.Serialize(writer);
                prop = new RelatedToProperty
                {
                    Value = "Other test",
                };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("RELATED-TO;RELTYPE=PARENT:Test")
                .AppendLine("RELATED-TO:Other test")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("RELATED-TO;RELTYPE=PARENT:Test")
                .AppendLine("RELATED-TO:Other test")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new RelatedToProperty();
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("RELATED-TO", prop.Name);
                Assert.Equal("Test", prop.Value);
                Assert.Equal(RelationTypes.Parent, (RelationTypes)prop.RelationType);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("RELATED-TO", prop.Name);
                Assert.Equal("Other test", prop.Value);
                Assert.Null(prop.RelationType);
            }
        }
    }
}
