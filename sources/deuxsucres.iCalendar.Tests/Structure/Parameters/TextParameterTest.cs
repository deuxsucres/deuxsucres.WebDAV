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
    public class TextParameterTest
    {
        [Fact]
        public void Create()
        {
            var param = new TextParameter();
            Assert.Null(param.Value);
        }

        [Fact]
        public void Cast()
        {
            var param = new TextParameter();
            Assert.Null(param.Value);
            Assert.Null((string)param);
            param = "Test";
            Assert.Equal("Test", param.Value);
            Assert.Equal("Test", (string)param);

            string sVal = null;
            param = sVal;
            Assert.Null(param);

            sVal = "Toto";
            param = sVal;
            Assert.Equal("Toto", param.Value);

            param = null;
            sVal = param;
            Assert.Null(sVal);
            Assert.Null((string)param);

            sVal = new TextParameter { Value = "Text" };
            Assert.Equal("Text", sVal);
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

            var param = new TextParameter { Name = "Test", Value = "Test" };
            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=Test:Content", writer.Parser.EncodeContentLine(line));
            Assert.True(param.Deserialize(reader, "param", "Other"));
            Assert.Equal("Other", param.Value);
            
        }

        [Fact]
        public void TestToString()
        {
            var param = new TextParameter { Name = "Test", Value = "Value" };
            Assert.Equal("Value", param.ToString());
        }
    }
}
