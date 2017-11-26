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
    public class UtcOffsetPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new UtcOffsetProperty();
            Assert.Equal(TimeSpan.Zero, prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var prop = new UtcOffsetProperty();
            Assert.Equal(TimeSpan.Zero, prop.Value);
            Assert.Equal(TimeSpan.Zero, (TimeSpan)prop);
            Assert.Equal(TimeSpan.Zero, (TimeSpan?)prop);

            TimeSpan ts = TimeSpan.FromMinutes(123);

            prop = ts;
            Assert.Equal(ts, prop.Value);
            Assert.Equal(ts, (TimeSpan)prop);
            Assert.Equal(ts, (TimeSpan?)prop);

            TimeSpan? tsn = ts;
            prop = tsn;
            Assert.Equal(ts, prop.Value);
            Assert.Equal(ts, (TimeSpan)prop);
            Assert.Equal(ts, (TimeSpan?)prop);

            tsn = null;
            prop = tsn;
            Assert.Null(prop);
            Assert.Equal(TimeSpan.Zero, (TimeSpan)prop);
            Assert.Null((TimeSpan?)prop);
        }

        [Fact]
        public void TestToString()
        {
            TimeSpan ts = TimeSpan.FromMinutes(123);
            UtcOffsetProperty prop = ts;
            Assert.Equal(ts.ToString(), prop.ToString());
            Assert.Equal(ts.ToString("c"), prop.ToString("c"));
            Assert.Equal(ts.ToString("c", CultureInfo.InvariantCulture), prop.ToString("c", CultureInfo.InvariantCulture));
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

            TimeSpan ts = TimeSpan.FromMinutes(123);
            UtcOffsetProperty prop = new UtcOffsetProperty { Name = "Test", Value = ts };
            ContentLine line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:+0203", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "prop", Value = "+0203" };
            prop.Deserialize(reader, line);
            Assert.Equal("prop", prop.Name);
            Assert.Equal(ts, prop.Value);

            line = new ContentLine { Name = "test", Value = "test" };
            prop.Deserialize(reader, line);
            Assert.Equal("test", prop.Name);
            Assert.Equal(TimeSpan.Zero, prop.Value);
        }
    }
}
