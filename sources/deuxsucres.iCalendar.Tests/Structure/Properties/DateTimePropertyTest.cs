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
    public class DateTimePropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new DateTimeProperty();
            Assert.Equal(DateTime.MinValue, prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var prop = new DateTimeProperty();
            Assert.Equal(DateTime.MinValue, prop.Value);
            Assert.Equal(DateTime.MinValue, (DateTime)prop);
            Assert.Equal(DateTime.MinValue, (DateTime?)prop);

            DateTime dt = new DateTime(2017, 11, 26, 7, 30, 0);

            prop = dt;
            Assert.Equal(dt, prop.Value);
            Assert.Equal(dt, (DateTime)prop);
            Assert.Equal(dt, (DateTime?)prop);

            DateTime? dtn = dt;
            prop = dtn;
            Assert.Equal(dt, prop.Value);
            Assert.Equal(dt, (DateTime)prop);
            Assert.Equal(dt, (DateTime?)prop);

            dtn = null;
            prop = dtn;
            Assert.Null(prop);
            Assert.Equal(DateTime.MinValue, (DateTime)prop);
            Assert.Null((DateTime?)prop);
        }

        [Fact]
        public void TestToString()
        {
            DateTime dt = new DateTime(2017, 11, 26, 7, 30, 0);
            DateTimeProperty prop = dt;
            Assert.Equal(dt.ToString(), prop.ToString());
            Assert.Equal(dt.ToString(CultureInfo.InvariantCulture), prop.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(dt.ToString("d"), prop.ToString("d"));
            Assert.Equal(dt.ToString("d", CultureInfo.InvariantCulture), prop.ToString("d", CultureInfo.InvariantCulture));
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

            DateTime dt = new DateTime(2017, 11, 26, 7, 30, 0);
            DateTimeProperty prop = new DateTimeProperty { Name = "Test", Value = dt };
            ContentLine line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:20171126T073000", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "prop", Value = "20171126T073000" };
            prop.Deserialize(reader, line);
            Assert.Equal("prop", prop.Name);
            Assert.Equal(dt, prop.Value);

            line = new ContentLine { Name = "test", Value = "test" };
            prop.Deserialize(reader, line);
            Assert.Equal("test", prop.Name);
            Assert.Equal(DateTime.MinValue, prop.Value);
        }
    }
}
