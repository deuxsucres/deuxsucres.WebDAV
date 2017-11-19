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
    public class IntParameterTest
    {
        [Fact]
        public void Create()
        {
            var param = new IntParameter();
            Assert.Equal(0, param.Value);
        }

        [Fact]
        public void Cast()
        {
            var param = new IntParameter();
            Assert.Equal(0, param.Value);
            Assert.Equal(0, (int)param);
            param = 123;
            Assert.Equal(123, param.Value);
            Assert.Equal(123, (int)param);

            int? iVal = null;
            param = iVal;
            Assert.Null(param);

            iVal = 21;
            param = iVal;
            Assert.Equal(21, param.Value);

            iVal = 23;
            param = iVal;
            Assert.Equal(23, param.Value);

            param = null;
            iVal = param;
            Assert.Null(iVal);
            Assert.Equal(0, (int)param);

            iVal = new IntParameter { Value = 98 };
            Assert.Equal(98, iVal);

            iVal = new IntParameter { Value = 89 };
            Assert.Equal(89, iVal);
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

            var param = new IntParameter { Name = "Test", Value = 123 };
            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=123:Content", writer.Parser.EncodeContentLine(line));
            Assert.True(param.Deserialize(reader, "param", "32"));
            Assert.Equal(32, param.Value);
            Assert.False(param.Deserialize(reader, "param", "Test"));
            Assert.Equal(0, param.Value);

            param = new IntParameter { Name = "Test", Value = 321 };
            line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=321:Content", writer.Parser.EncodeContentLine(line));
            
        }
    }
}
