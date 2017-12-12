using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class CalPropertyParameterTest
    {

        [Fact]
        public void Create()
        {
            var param = new Mock<CalPropertyParameter>() { CallBase = true }.Object;
            Assert.Null(param.Name);
        }

        [Fact]
        public void Deserialize()
        {
            var mParam = new Mock<CalPropertyParameter>() { CallBase = true };
            mParam.Protected().Setup<bool>("InternalDeserialize", ItExpr.IsAny<ICalReader>(), ItExpr.IsAny<string>(), ItExpr.IsAny<string>())
                .Returns(false);
            var param = mParam.Object;

            var mReader = new Mock<ICalReader>();
            var reader = mReader.Object;

            param.Name = "test";
            Assert.False(param.Deserialize(reader, "PropName", "PropValue"));
            Assert.Null(param.Name);
            mParam.Protected().Verify("InternalDeserialize", Times.Once(), reader, "PropName", "PropValue");

            mParam = new Mock<CalPropertyParameter>() { CallBase = true };
            mParam.Protected().Setup<bool>("InternalDeserialize", ItExpr.IsAny<ICalReader>(), ItExpr.IsAny<string>(), ItExpr.IsAny<string>())
                .Returns(true);
            param = mParam.Object;

            param.Name = "test";
            Assert.True(param.Deserialize(reader, "PropName", "PropValue"));
            Assert.Equal("PropName", param.Name);
            mParam.Protected().Verify("InternalDeserialize", Times.Once(), reader, "PropName", "PropValue");
        }

        [Fact]
        public void Serialize()
        {
            var mParam = new Mock<CalPropertyParameter>() { CallBase = true };
            var param = mParam.Object;

            var mWriter = new Mock<ICalWriter>();
            mWriter.SetupGet(w => w.Parser).Returns(new CalendarParser());
            var writer = mWriter.Object;

            ContentLine line = new ContentLine();
            Assert.False(param.Serialize(writer, line));
            Assert.Equal(0, line.ParamCount);

            param.Name = "Test";
            Assert.Throws<NotImplementedException>(() => param.Serialize(writer, line));
            Assert.Equal(0, line.ParamCount);

            string value = null;
            mParam = new Mock<CalPropertyParameter>() { CallBase = true };
            mParam.Protected().Setup<string>("SerializeValue", ItExpr.IsAny<ICalWriter>(), ItExpr.IsAny<ContentLine>())
                .Returns(() => value);
            param = mParam.Object;
            param.Name = "Test";

            Assert.False(param.Serialize(writer, line));
            Assert.Equal(0, line.ParamCount);

            value = "Value";
            Assert.True(param.Serialize(writer, line));
            Assert.Equal(1, line.ParamCount);
            Assert.Equal(";TEST=Value:", writer.Parser.EncodeContentLine(line));
        }

    }
}
