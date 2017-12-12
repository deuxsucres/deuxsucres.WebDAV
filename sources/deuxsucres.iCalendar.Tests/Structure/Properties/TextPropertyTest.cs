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

namespace deuxsucres.iCalendar.Tests.Structure.Properties
{
    public class TextPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new TextProperty();
            Assert.Null(prop.Value);
        }

        [Fact]
        public void Cast()
        {
            var prop = new TextProperty();
            Assert.Null(prop.Value);
            Assert.Null((string)prop);
            prop = "Test";
            Assert.Equal("Test", prop.Value);
            Assert.Equal("Test", (string)prop);

            string sVal = null;
            prop = sVal;
            Assert.Null(prop);

            sVal = "Toto";
            prop = sVal;
            Assert.Equal("Toto", prop.Value);

            prop = null;
            sVal = prop;
            Assert.Null(sVal);
            Assert.Null((string)prop);

            sVal = new TextProperty { Value = "Text" };
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

            var prop = new TextProperty { Name = "Test", Value = "Text" };
            ContentLine line = prop.Serialize(writer);
            Assert.NotNull(line);
            Assert.Equal("TEST:Text", writer.Parser.EncodeContentLine(line));

            line = new ContentLine { Name = "Prop", Value = "Value" };
            prop = new TextProperty { Name = "Test", Value = "Test" };
            prop.Deserialize(reader, line);
            Assert.Equal("Prop", prop.Name);
            Assert.Equal("Value", prop.Value);
        }

        [Fact]
        public void TestToString()
        {
            var param = new TextProperty { Name = "Test", Value = "Value" };
            Assert.Equal("Value", param.ToString());
        }
    }
}
