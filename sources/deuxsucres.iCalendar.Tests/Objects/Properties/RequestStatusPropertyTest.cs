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
    public class RequestStatusPropertyTest
    {
        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                RequestStatusProperty prop = new RequestStatusProperty
                {
                    StatusCode = "1.2",
                    StatusDescription = "Status",
                    ExtraData = "Extra data",
                    Language = "fr"
                };
                prop.Serialize(writer);
                prop = new RequestStatusProperty
                {
                    StatusCode = "3.4",
                    StatusDescription = "Status"
                };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("REQUEST-STATUS;LANGUAGE=fr:1.2;Status;Extra data")
                .AppendLine("REQUEST-STATUS:3.4;Status")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("REQUEST-STATUS;LANGUAGE=fr:1.2;Status;Extra data")
                .AppendLine("REQUEST-STATUS:3.4;Status")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = new RequestStatusProperty();
                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("REQUEST-STATUS", prop.Name);
                Assert.Equal("1.2", prop.StatusCode);
                Assert.Equal("Status", prop.StatusDescription);
                Assert.Equal("Extra data", prop.ExtraData);
                Assert.Equal("fr", prop.Language);

                prop.Deserialize(reader, reader.ReadNextLine());
                Assert.Equal("REQUEST-STATUS", prop.Name);
                Assert.Equal("3.4", prop.StatusCode);
                Assert.Equal("Status", prop.StatusDescription);
                Assert.Null(prop.ExtraData);
                Assert.Null(prop.Language);

                prop.Deserialize(reader, new ContentLine { Name = "REQUEST-STATUS" });
                Assert.Equal("REQUEST-STATUS", prop.Name);
                Assert.Null(prop.StatusCode);
                Assert.Null(prop.StatusDescription);
                Assert.Null(prop.ExtraData);
                Assert.Null(prop.Language);
            }
        }
    }
}
