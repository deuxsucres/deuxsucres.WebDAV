using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure.Parameters
{
    public class CalAddressesParameterTest
    {
        [Fact]
        public void Create()
        {
            var param = new CalAddressesParameter();
            Assert.NotNull(param.Value);
            Assert.Empty(param.Value);
        }

        [Fact]
        public void Cast()
        {
            var addresses = new string[] { "mailto:yan@deuxsucres.com", "mailto:contact@deuxsucres" };
            var uris = addresses.Select(a => new Uri(a)).ToArray();

            CalAddressesParameter param = addresses;
            Assert.NotNull(param);
            Assert.Equal(uris, (Uri[])param);
            Assert.Equal(uris, (List<Uri>)param);
            Assert.Equal(addresses, (string[])param);
            Assert.Equal(addresses, (List<string>)param);

            param = addresses.ToList();
            Assert.NotNull(param);
            Assert.Equal(uris, (List<Uri>)param);

            param = uris;
            Assert.NotNull(param);
            Assert.Equal(addresses, (List<string>)param);

            param = uris.ToList();
            Assert.NotNull(param);
            Assert.Equal(addresses, (string[])param);

            addresses = null;
            uris = null;
            param = addresses;
            Assert.Null(param);
            param = uris;
            Assert.Null(param);
            param = (List<string>)null;
            Assert.Null(param);
            param = (List<Uri>)null;
            Assert.Null(param);
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();
            var mWriter = new Mock<ICalWriter>();
            mWriter.SetupGet(w => w.Parser).Returns(parser);
            var writer = mWriter.Object;
            var mReader = new Mock<ICalReader>();
            mReader.SetupGet(r => r.Parser).Returns(parser);
            var reader = mReader.Object;

            CalAddressesParameter param = new CalAddressesParameter();

            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };

            Assert.True(param.Deserialize(reader, "test1", "\"MAILTO:yan@deuxsucres.com\""));
            Assert.Equal("test1", param.Name);
            Assert.Equal(new Uri[] { new Uri("mailto:yan@deuxsucres.com") }, param.Value);

            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST1=\"mailto:yan@deuxsucres.com\":Content", writer.Parser.EncodeContentLine(line));

            Assert.True(param.Deserialize(reader, "test2", "\"MAILTO:yan@deuxsucres.com\",\"MAILTO:contact@deuxsucres.com\""));
            Assert.Equal("test2", param.Name);
            Assert.Equal(new Uri[] {
                new Uri("mailto:yan@deuxsucres.com"),
                new Uri("mailto:contact@deuxsucres.com")
            }, param.Value);

            line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST2=\"mailto:yan@deuxsucres.com\",\"mailto:contact@deuxsucres.com\":Content", writer.Parser.EncodeContentLine(line));

            Assert.True(param.Deserialize(reader, "test3", "a,\"MAILTO:yan@deuxsucres.com\", \"MAILTO:test@deuxsucres.com,\"MAILTO:contact@deuxsucres.com\""));
            Assert.Equal("test3", param.Name);
            Assert.Equal(new Uri[] {
                new Uri("mailto:yan@deuxsucres.com"),
                new Uri("mailto:contact@deuxsucres.com")
            }, param.Value);

            line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST3=\"mailto:yan@deuxsucres.com\",\"mailto:contact@deuxsucres.com\":Content", writer.Parser.EncodeContentLine(line));

            Assert.False(param.Deserialize(reader, "test3", "other"));
            Assert.Null(param.Name);
            Assert.Empty(param.Value);
        }

    }
}
