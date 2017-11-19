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
    public class BooleanParameterTest
    {
        [Fact]
        public void Create()
        {
            var param = new BooleanParameter();
            Assert.False(param.Value);
        }

        [Fact]
        public void Cast()
        {
            var param = new BooleanParameter();
            Assert.False(param.Value);
            Assert.False(param);
            param = true;
            Assert.True(param.Value);
            Assert.True(param);

            bool? bVal = null;
            param = bVal;
            Assert.Null(param);

            bVal = false;
            param = bVal;
            Assert.False(param.Value);

            bVal = true;
            param = bVal;
            Assert.True(param.Value);

            param = null;
            bVal = param;
            Assert.Null(bVal);
            Assert.False(param);

            bVal = new BooleanParameter { Value = false };
            Assert.False(bVal);

            bVal = new BooleanParameter { Value = true };
            Assert.True(bVal);
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

            var param = new BooleanParameter { Name = "Test", Value = false };
            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=FALSE:Content", writer.Parser.EncodeContentLine(line));
            Assert.True(param.Deserialize(reader, "param", "False"));
            Assert.False(param.Value);
            Assert.True(param.Deserialize(reader, "param", "True"));
            Assert.True(param.Value);
            Assert.False(param.Deserialize(reader, "param", "Test"));
            Assert.False(param.Value);

            param = new BooleanParameter { Name = "Test", Value = true };
            line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=TRUE:Content", writer.Parser.EncodeContentLine(line));
            
        }
    }
}
