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
    public class IntegerPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new IntegerProperty();
            Assert.Equal(0, prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var prop = new IntegerProperty();
            Assert.Equal(0, prop.Value);
            Assert.Equal(0, (int)prop);
            prop = 123;
            Assert.Equal(123, prop.Value);
            Assert.Equal(123, (int)prop);

            int? iVal = null;
            prop = iVal;
            Assert.Null(prop);

            iVal = 21;
            prop = iVal;
            Assert.Equal(21, prop.Value);

            iVal = 23;
            prop = iVal;
            Assert.Equal(23, prop.Value);

            prop = null;
            iVal = prop;
            Assert.Null(iVal);
            Assert.Equal(0, (int)prop);

            iVal = new IntegerProperty { Value = 98 };
            Assert.Equal(98, iVal);

            iVal = new IntegerProperty { Value = 89 };
            Assert.Equal(89, iVal);
        }

        [Fact]
        public void TestToString()
        {
            IntegerProperty prop = 123;
            Assert.Equal("123", prop.ToString());
            Assert.Equal("123", prop.ToString(CultureInfo.InvariantCulture));
            Assert.Equal("00123", prop.ToString("D5"));
            Assert.Equal("00123", prop.ToString("D5", CultureInfo.InvariantCulture));
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

            var prop = new IntegerProperty { Name = "Test", Value = 123 };
            ContentLine line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:123", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "param", Value = "32" };
            prop.Deserialize(reader, line);
            Assert.Equal(32, prop.Value);

            line = new ContentLine { Name = "param", Value = "test" };
            prop.Deserialize(reader, line);
            Assert.Equal(0, prop.Value);

            prop = new IntegerProperty { Name = "Test", Value = 321 };
            Assert.NotNull(line = prop.Serialize(writer));
            Assert.Equal("TEST:321", writer.Parser.EncodeContentLine(line));            
        }

    }
}
