using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure.Parameters;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure.Parameters
{
    public class EnumParameterTest
    {
        public enum EnumTest
        {
            Value,
            SecondValue,
            Third_Value
        }

        [Fact]
        public void Create()
        {
            var param = new EnumParameter<EnumTest>();
            Assert.Null(param.Value);
            Assert.Null(param.StringValue);
        }

        [Fact]
        public void Values()
        {
            var param = new EnumParameter<EnumTest>();
            Assert.Null(param.Value);
            Assert.Null(param.StringValue);

            param.Value = EnumTest.Third_Value;
            Assert.Equal(EnumTest.Third_Value, param.Value);
            Assert.Equal("THIRD-VALUE", param.StringValue);

            param.Value = null;
            Assert.Null(param.Value);
            Assert.Equal("THIRD-VALUE", param.StringValue);

            param.StringValue = "SecondValue";
            Assert.Equal(EnumTest.SecondValue, param.Value);
            Assert.Equal("SecondValue", param.StringValue);

            param.StringValue = "Test";
            Assert.Null(param.Value);
            Assert.Equal("Test", param.StringValue);

            param.StringValue = null;
            Assert.Null(param.Value);
            Assert.Null(param.StringValue);
        }

        [Fact]
        public void Cast()
        {
            var param = new EnumParameter<EnumTest>();
            Assert.Null(param.Value);
            Assert.Null((EnumTest?)param);
            Assert.Null((string)param);

            param = EnumTest.Third_Value;
            Assert.Equal(EnumTest.Third_Value, param.Value);
            Assert.Equal(EnumTest.Third_Value, (EnumTest?)param);
            Assert.Equal("THIRD-VALUE", param.StringValue);
            Assert.Equal("THIRD-VALUE", param);

            param = "SecondValue";
            Assert.Equal(EnumTest.SecondValue, param.Value);
            Assert.Equal(EnumTest.SecondValue, (EnumTest?)param);
            Assert.Equal("SecondValue", param.StringValue);
            Assert.Equal("SecondValue", param);

            param = "Test";
            Assert.Null(param.Value);
            Assert.Null((EnumTest?)param);
            Assert.Equal("Test", param.StringValue);
            Assert.Equal("Test", param);
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

            var param = new EnumParameter<EnumTest> { Name = "Test", Value = EnumTest.Third_Value };
            ContentLine line = new ContentLine
            {
                Name = "Line",
                Value = "Content"
            };
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=THIRD-VALUE:Content", writer.Parser.EncodeContentLine(line));

            param.Value = EnumTest.SecondValue;
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=SECONDVALUE:Content", writer.Parser.EncodeContentLine(line));

            param.StringValue = "Test";
            Assert.True(param.Serialize(writer, line));
            Assert.Equal("Line;TEST=Test:Content", writer.Parser.EncodeContentLine(line));

            Assert.True(param.Deserialize(reader, "param", "Third-Value"));
            Assert.Equal(EnumTest.Third_Value, (EnumTest?)param);
            Assert.Equal("THIRD-VALUE", param);

            Assert.True(param.Deserialize(reader, "param", "Secondvalue"));
            Assert.Equal(EnumTest.SecondValue, (EnumTest?)param);
            Assert.Equal("SECONDVALUE", param);

            Assert.True(param.Deserialize(reader, "param", "Test"));
            Assert.Null((EnumTest?)param);
            Assert.Equal("Test", param);

            Assert.False(param.Deserialize(reader, "param", ""));
            Assert.Null((EnumTest?)param);
            Assert.Null((string)param);
        }

    }
}
