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
    public class UriParameterTest
    {
        [Fact]
        public void Create()
        {
            var param = new UriParameter();
            Assert.Null(param.Value);
        }

        [Fact]
        public void Cast()
        {
            var param = new UriParameter();
            Assert.Null(param.Value);
            Assert.Null((Uri)param);
            Assert.Null((string)param);

            param = "http://www.deuxsucres.com";
            Assert.Equal(new Uri("http://www.deuxsucres.com"), param.Value);
            Assert.Equal(new Uri("http://www.deuxsucres.com"), (Uri)param);
            Assert.Equal("http://www.deuxsucres.com/", (string)param);

            param = new Uri("http://www.neige-informatique.fr");
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), param.Value);
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), (Uri)param);
            Assert.Equal("http://www.neige-informatique.fr/", (string)param);
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

            var param = new UriParameter { Name = "Test", Value = new Uri("http://www.deuxsucres.com") };
            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=\"http://www.deuxsucres.com/\":Content", writer.Parser.EncodeContentLine(line));
            Assert.True(param.Deserialize(reader, "param", "\"http://www.neige-informatique.fr\""));
            Assert.Equal(new Uri("http://www.neige-informatique.fr"), param.Value);
            Assert.False(param.Deserialize(reader, "param", "\"Test\""));
            Assert.Null(param.Value);
            Assert.False(param.Deserialize(reader, "param", "Test"));
            Assert.Null(param.Value);
        }

    }
}
