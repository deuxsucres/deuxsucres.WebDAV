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
    public class FloatParameterTest
    {
        [Fact]
        public void Create()
        {
            var param = new FloatParameter();
            Assert.Equal(0, param.Value);
        }

        [Fact]
        public void Cast()
        {
            var param = new FloatParameter();
            Assert.Equal(0, param.Value);
            Assert.Equal(0, (double)param);
            param = 123.45;
            Assert.Equal(123.45, param.Value);
            Assert.Equal(123.45, (double)param);

            double? iVal = null;
            param = iVal;
            Assert.Null(param);

            iVal = 21.34;
            param = iVal;
            Assert.Equal(21.34, param.Value);

            iVal = 23.45;
            param = iVal;
            Assert.Equal(23.45, param.Value);

            param = null;
            iVal = param;
            Assert.Null(iVal);
            Assert.Equal(0, (double)param);

            iVal = new FloatParameter { Value = 98.76 };
            Assert.Equal(98.76, iVal);

            iVal = new FloatParameter { Value = 89.76 };
            Assert.Equal(89.76, iVal);
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

            var param = new FloatParameter { Name = "Test", Value = 123.45 };
            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=123.45:Content", writer.Parser.EncodeContentLine(line));
            Assert.True(param.Deserialize(reader, "param", "32.45"));
            Assert.Equal(32.45, param.Value);
            Assert.False(param.Deserialize(reader, "param", "Test"));
            Assert.Equal(0, param.Value);

            param = new FloatParameter { Name = "Test", Value = 321.45 };
            line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=321.45:Content", writer.Parser.EncodeContentLine(line));
            
        }
    }
}
