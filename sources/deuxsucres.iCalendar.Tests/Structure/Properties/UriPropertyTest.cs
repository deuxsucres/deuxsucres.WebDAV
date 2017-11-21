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
    public class UriPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new UriProperty();
            Assert.Null(prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var prop = new UriProperty();
            Assert.Null(prop.Value);
            Assert.Null((Uri)prop);
            Assert.Null((string)prop);

            prop = "http://www.deuxsucres.com";
            Assert.Equal(new Uri("http://www.deuxsucres.com"), prop.Value);
            Assert.Equal(new Uri("http://www.deuxsucres.com"), (Uri)prop);
            Assert.Equal("http://www.deuxsucres.com/", (string)prop);

            prop = new Uri("http://www.neige-informatique.fr");
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), prop.Value);
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), (Uri)prop);
            Assert.Equal("http://www.neige-informatique.fr/", (string)prop);
        }

        [Fact]
        public void TestToString()
        {
            UriProperty prop = "http://www.deuxsucres.com";
            Assert.Equal("http://www.deuxsucres.com/", prop.ToString());
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

            var prop = new UriProperty { Name = "Test", Value = new Uri("http://www.deuxsucres.com") };
            ContentLine line;
            Assert.NotNull(line = prop.Serialize(writer));
            Assert.Equal("TEST:http://www.deuxsucres.com/", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "param", Value = "http://www.neige-informatique.fr" };
            prop.Deserialize(reader, line);
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), prop.Value);

            line = new ContentLine { Name = "param", Value = "\"http://www.neige-informatique.fr\"" };
            prop.Deserialize(reader, line);
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), prop.Value);

            line = new ContentLine { Name = "param", Value = "\"Test\"" };
            prop.Deserialize(reader, line);
            Assert.Null(prop.Value);

            line = new ContentLine { Name = "param", Value = "Test" };
            prop.Deserialize(reader, line);
            Assert.Null(prop.Value);
        }

    }
}
