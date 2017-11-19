using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using Xunit;
using Moq;
using Moq.Protected;
using System.IO;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class CalPropertyTest
    {
        class TestPropertyParameter : CalPropertyParameter
        {
            protected override bool InternalDeserialize(ICalReader reader, string name, string value)
            {
                Name = name;
                Value = value;
                return !string.IsNullOrWhiteSpace(name);
            }
            protected override string SerializeValue(ICalWriter writer, ContentLine line)
            {
                return Value;
            }
            public string Value { get; set; }
        }

        [Fact]
        public void Create()
        {
            var prop = new Mock<CalProperty>() { CallBase = true }.Object;
            Assert.Equal(0, prop.ParameterCount);
            Assert.Null(prop.Name);
        }

        [Fact]
        public void Create_TypedProperty()
        {
            var tProp = new CalProperty<int>();
            Assert.Equal(0, tProp.ParameterCount);
            Assert.Null(tProp.Name);
            Assert.Equal(0, tProp.Value);
        }

        [Fact]
        public void Parameters()
        {
            var prop = new Mock<CalProperty>() { CallBase = true }.Object;

            Assert.Equal(0, prop.ParameterCount);
            TestPropertyParameter p1 = new TestPropertyParameter { Name = "p1", Value = "v1" };
            prop.SetParameter(p1);
            Assert.Equal(1, prop.ParameterCount);
            Assert.NotNull(prop.FindParameter("P1"));

            prop.SetParameter(null);
            Assert.Equal(1, prop.ParameterCount);

            ICalPropertyParameter p2 = new Mock<ICalPropertyParameter>().Object;
            prop.SetParameter(p2, "p2");
            Assert.Equal(2, prop.ParameterCount);
            Assert.NotNull(prop.FindParameter("P2"));

            TestPropertyParameter p3 = new TestPropertyParameter { Name = "p2", Value = "v2" };
            prop.SetParameter(p3, "p3");
            Assert.Equal(3, prop.ParameterCount);
            Assert.NotNull(prop.FindParameter("P3"));

            Assert.Same(p1, prop.FindParameter("P1"));
            Assert.Same(p2, prop.FindParameter("P2"));
            Assert.Same(p3, prop.FindParameter("P3"));
            Assert.Null(prop.FindParameter("P4"));

            Assert.NotNull(prop.FindParameter<TestPropertyParameter>("P1"));
            Assert.Null(prop.FindParameter<TestPropertyParameter>("P2"));
            Assert.NotNull(prop.FindParameter<TestPropertyParameter>("P3"));
            Assert.Null(prop.FindParameter<TestPropertyParameter>("P4"));

            Assert.Same(p3, prop.GetParameter<TestPropertyParameter>("p3"));
            var p4 = prop.GetParameter<TestPropertyParameter>("p4");
            Assert.NotNull(p4);
            Assert.Equal(4, prop.ParameterCount);

            var p5 = prop.GetParameter<TestPropertyParameter>("p2");
            Assert.Same(p5, prop.FindParameter("P2"));
            Assert.NotNull(prop.FindParameter<TestPropertyParameter>("P2"));
            Assert.Equal(4, prop.ParameterCount);

            prop.SetParameter(null, "p2");
            Assert.Equal(3, prop.ParameterCount);
            prop.SetParameter(null, "p3");
            Assert.Equal(2, prop.ParameterCount);

            Assert.Equal(new ICalPropertyParameter[] { p1, p4 }, prop.GetParameters());
            prop.RemoveParameter("P1");
            prop.RemoveParameter("P2");
            Assert.Equal(new ICalPropertyParameter[] { p4 }, prop.GetParameters());
        }

        [Fact]
        public void Reset()
        {
            var prop = new Mock<CalProperty>() { CallBase = true }.Object;

            prop.Name = "Test";
            prop.GetParameter<TestPropertyParameter>("p1");
            prop.GetParameter<TestPropertyParameter>("p2");
            prop.GetParameter<TestPropertyParameter>("p3");

            prop.Reset();
            Assert.Null(prop.Name);
            Assert.Equal(0, prop.ParameterCount);
        }

        [Fact]
        public void Serialize()
        {
            var mProp = new Mock<CalProperty>() { CallBase = true };
            mProp.Protected().Setup<string>("SerializeValue", ItExpr.IsAny<ICalWriter>(), ItExpr.IsAny<ContentLine>())
                .Returns<ICalWriter, ContentLine>((w, l) =>
                {
                    return "Value";
                });
            var prop = mProp.Object;

            prop.Name = "Property";
            prop.GetParameter<TestPropertyParameter>("p1").Value = "v1";
            prop.GetParameter<TestPropertyParameter>("p2").Value = "v2";
            prop.GetParameter<TestPropertyParameter>("p3").Value = "v3";

            var mWriter = new Mock<ICalWriter>();
            mWriter.SetupGet(w => w.Parser).Returns(new CalendarParser());
            var writer = mWriter.Object;

            var line = prop.Serialize(writer);
            Assert.Equal("PROPERTY;P1=v1;P2=v2;P3=v3:Value", writer.Parser.EncodeContentLine(line));
            mWriter.Verify(w => w.WriteLine(It.IsAny<ContentLine>()), Times.Once());
        }

        [Fact]
        public void Serialize_Typed()
        {
            var mProp = new Mock<CalProperty<int>>() { CallBase = true };
            CalProperty<int> prop = null;
            mProp.Protected().Setup<string>("SerializeValue", ItExpr.IsAny<ICalWriter>(), ItExpr.IsAny<ContentLine>())
                .Returns<ICalWriter, ContentLine>((w, l) =>
                {
                    return prop.Value.ToString();
                });
            prop = mProp.Object;

            prop.Name = "Property";
            prop.Value = 123;
            prop.GetParameter<TestPropertyParameter>("p1").Value = "v1";
            prop.GetParameter<TestPropertyParameter>("p2").Value = "v2";
            prop.GetParameter<TestPropertyParameter>("p3").Value = "v3";

            var mWriter = new Mock<ICalWriter>();
            mWriter.SetupGet(w => w.Parser).Returns(new CalendarParser());
            var writer = mWriter.Object;

            var line = prop.Serialize(writer);
            Assert.Equal("PROPERTY;P1=v1;P2=v2;P3=v3:123", writer.Parser.EncodeContentLine(line));
            mWriter.Verify(w => w.WriteLine(It.IsAny<ContentLine>()), Times.Once());

            prop = new CalProperty<int>();
            Assert.Throws<NotImplementedException>(() => prop.Serialize(writer));
        }

        [Fact]
        public void Deserialize()
        {
            var mProp = new Mock<CalProperty>() { CallBase = true };
            var prop = mProp.Object;

            var mReader = new Mock<ICalReader>();
            mReader.SetupGet(w => w.Parser).Returns(new CalendarParser());
            mReader.Setup(r => r.CreateDefaultParameter(It.IsAny<string>()))
                .Returns<string>((n) => n != "P2" ? new TestPropertyParameter { Name = n } : null);
            var reader = mReader.Object;

            ContentLine line;
            using (var str = new StringReader("PROPERTY;P1=v1;P2=v2;P3=v3:Value"))
                line = reader.Parser.ReadContentLine(str);
            prop.Deserialize(reader, line);

            Assert.Equal("PROPERTY", prop.Name);
            Assert.Equal(2, prop.ParameterCount);
        }

        [Fact]
        public void Deserialize_Type()
        {
            var mProp = new Mock<CalProperty<int>>() { CallBase = true };
            CalProperty<int> prop = null;
            mProp.Protected().Setup<bool>("DeserializeValue", ItExpr.IsAny<ICalReader>(), ItExpr.IsAny<ContentLine>())
                .Returns<ICalReader, ContentLine>((r, l) =>
                {
                    prop.Value = int.Parse(l.Value);
                    return true;
                });
            prop = mProp.Object;

            var mReader = new Mock<ICalReader>();
            mReader.SetupGet(w => w.Parser).Returns(new CalendarParser());
            mReader.Setup(r => r.CreateDefaultParameter(It.IsAny<string>()))
                .Returns<string>((n) => n != "P2" ? new TestPropertyParameter { Name = n } : null);
            var reader = mReader.Object;

            ContentLine line;
            using (var str = new StringReader("PROPERTY;P1=v1;P2=v2;P3=v3:321"))
                line = reader.Parser.ReadContentLine(str);
            prop.Deserialize(reader, line);

            Assert.Equal("PROPERTY", prop.Name);
            Assert.Equal(321, prop.Value);
            Assert.Equal(2, prop.ParameterCount);

            prop = new CalProperty<int>();
            Assert.Throws<NotImplementedException>(() => prop.Deserialize(reader, line));
        }

    }
}
