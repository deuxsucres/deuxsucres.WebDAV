using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure.Properties
{
    public class FloatPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new FloatProperty();
            Assert.Equal(0.0, prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var prop = new FloatProperty();
            Assert.Equal(0, prop.Value);
            Assert.Equal(0, (double)prop);
            prop = 123.45;
            Assert.Equal(123.45, prop.Value);
            Assert.Equal(123.45, (double)prop);

            double? iVal = null;
            prop = iVal;
            Assert.Null(prop);

            iVal = 21.45;
            prop = iVal;
            Assert.Equal(21.45, prop.Value);

            iVal = 23.45;
            prop = iVal;
            Assert.Equal(23.45, prop.Value);

            prop = null;
            iVal = prop;
            Assert.Null(iVal);
            Assert.Equal(0, (double)prop);

            iVal = new FloatProperty { Value = 98.45 };
            Assert.Equal(98.45, iVal);

            iVal = new FloatProperty { Value = 89.45 };
            Assert.Equal(89.45, iVal);
        }

        [Fact]
        public void TestToString()
        {
            FloatProperty prop = 123.45;
            Assert.Equal((123.45).ToString(), prop.ToString());
            Assert.Equal("123.45", prop.ToString(CultureInfo.InvariantCulture));
            Assert.Equal((123.45).ToString("0 000.000"), prop.ToString("0 000.000"));
            Assert.Equal("0 123.450", prop.ToString("0 000.000", CultureInfo.InvariantCulture));
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

            var prop = new FloatProperty { Name = "Test", Value = 123.45 };
            ContentLine line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:123.45", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "param", Value = "32.45" };
            prop.Deserialize(reader, line);
            Assert.Equal(32.45, prop.Value);

            line = new ContentLine { Name = "param", Value = "test" };
            prop.Deserialize(reader, line);
            Assert.Equal(0, prop.Value);

            prop = new FloatProperty { Name = "Test", Value = 321.45 };
            Assert.NotNull(line = prop.Serialize(writer));
            Assert.Equal("TEST:321.45", writer.Parser.EncodeContentLine(line));            
        }

    }
}
