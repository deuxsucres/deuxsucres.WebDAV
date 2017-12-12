using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class FreeBusyPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new FreeBusyProperty();
            Assert.Equal("FREEBUSY", prop.Name);
            Assert.Null(prop.FreeBusyType);
            Assert.Null(prop.Periods);
        }

        [Fact]
        public void Properties()
        {
            var prop = new FreeBusyProperty();
            Assert.Null(prop.FreeBusyType);
            Assert.Null(prop.Periods);

            prop.FreeBusyType = FreeBusyTypes.Busy;
            Assert.Equal(FreeBusyTypes.Busy, (FreeBusyTypes)prop.FreeBusyType);
            prop.FreeBusyType = null;
            Assert.Null(prop.FreeBusyType);

            DateTime dt = DateTime.Now;
            var data = new Period[] { new Period(dt, TimeSpan.FromMinutes(5)) };
            prop.Periods = data;
            Assert.NotNull(prop.Periods);
            Assert.Equal(data, prop.Periods);
            prop.Periods = null;
            Assert.Null(prop.Periods);
        }

        [Fact]
        public void Serialization()
        {
            DateTime dt = new DateTime(2017, 12, 9, 12, 36, 12);
            var periods = new Period[] { new Period(dt, TimeSpan.FromMinutes(5)) };

            var parser = new CalendarParser();
            var output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new FreeBusyProperty
                {
                    FreeBusyType = FreeBusyTypes.Busy,
                    Periods = periods
                };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("FREEBUSY;FBTYPE=BUSY:20171209T123612/PT5M")
                .ToString(), output.ToString());

            string input = output.ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = reader.MakeProperty<FreeBusyProperty>(reader.ReadNextLine());
                Assert.Equal(FreeBusyTypes.Busy, (FreeBusyTypes)prop.FreeBusyType);
                Assert.Single(prop.Periods);

                prop = reader.MakeProperty<FreeBusyProperty>(reader.ReadNextLine());
                Assert.Null(prop);
            }

        }

    }
}
