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
    public class RecurrencePropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new RecurrenceProperty();
            Assert.Null(prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var rec = new Recurrence();
            var prop = new RecurrenceProperty();
            Assert.Null(prop.Value);
            Assert.Null((Recurrence)prop);
            prop = rec;
            Assert.Same(rec, prop.Value);
            Assert.Same(rec, (Recurrence)prop);

            rec = null;
            prop = rec;
            Assert.Null(prop);
            Assert.Null((Recurrence)prop);
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

            var prop = new RecurrenceProperty { Name = "Test"};
            ContentLine line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:", writer.Parser.EncodeContentLine(line));

            var rec = new Recurrence() { ByHour = new int[] { 5, 12 }, Count = 17, Frequency = Recurrence.Frequencies.Minutely };
            prop.Value = rec;
            line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:FREQ=MINUTELY;COUNT=17;BYHOUR=5,12", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "Prop", Value = "FREQ=HOURLY;COUNT=2;BYHOUR=2,4,6" };
            prop.Deserialize(reader, line);
            Assert.Equal("Prop", prop.Name);
            Assert.NotNull(prop.Value);
            Assert.Equal(Recurrence.Frequencies.Hourly, prop.Value.Frequency);
            Assert.Equal(2, prop.Value.Count);
            Assert.Equal(new int[] { 2, 4, 6 }, prop.Value.ByHour);

            line = new ContentLine { Name = "Recur", Value = "test" };
            prop.Deserialize(reader, line);
            Assert.Equal("Recur", prop.Name);
            Assert.Null(prop.Value);
        }
    }
}
