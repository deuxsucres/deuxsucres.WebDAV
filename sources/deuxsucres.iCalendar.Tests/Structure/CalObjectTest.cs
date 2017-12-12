using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using deuxsucres.iCalendar.Parser;
using Moq.Protected;
using System.IO;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class CalObjectTest
    {
        class TestProperty : CalProperty<int>
        {
            protected override bool DeserializeValue(ICalReader reader, ContentLine line)
            {
                if (int.TryParse(line.Value, out int val))
                {
                    Value = val;
                    return true;
                }
                return false;
            }
            protected override string SerializeValue(ICalWriter writer, ContentLine line)
            {
                return Value.ToString();
            }
        }
        [Fact]
        public void Create()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            var obj = mObject.Object;

            Assert.Equal(0, obj.PropertyCount);
        }

        [Fact]
        public void Properties()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            var obj = mObject.Object;

            Assert.Equal(0, obj.PropertyCount);

            var p1 = new CalProperty<int> { Name = "p1", Value = 123 };
            var p2 = new CalProperty<int> { Name = "p2", Value = 456 };
            var p3 = new CalProperty<string> { Name = "P1", Value = "789" };
            obj.AddProperty(p1);
            obj.AddProperty(p2);
            obj.AddProperty(p3);
            obj.AddProperty(p1);
            obj.AddProperty(null);

            Assert.Equal(3, obj.PropertyCount);
            Assert.Same(p1, obj.FindProperty("P1"));
            Assert.Same(p2, obj.FindProperty("P2"));
            Assert.Null(obj.FindProperty("P3"));

            Assert.Same(p1, obj.FindProperty<CalProperty<int>>("P1"));
            Assert.Same(p3, obj.FindProperty<CalProperty<string>>("P1"));

            Assert.Equal(new ICalProperty[] { p1, p3 }, obj.FindProperties("p1"));
            Assert.Equal(new ICalProperty[] { p1 }, obj.FindProperties<CalProperty<int>>("p1"));
            Assert.Equal(new ICalProperty[] { p3 }, obj.FindProperties<CalProperty<string>>("p1"));

            Assert.Equal(new ICalProperty[] { p1, p2, p3 }, obj.GetProperties());

            obj.SetProperty(p1);
            obj.SetProperty(null);
            Assert.Equal(2, obj.PropertyCount);
            Assert.Same(p1, obj.FindProperty("P1"));
            Assert.Same(p2, obj.FindProperty("P2"));
            Assert.Null(obj.FindProperty("P3"));

            obj.RemoveProperty("p1");
            Assert.Equal(1, obj.PropertyCount);
            Assert.Null(obj.FindProperty("P1"));
            Assert.Same(p2, obj.FindProperty("P2"));
            Assert.Null(obj.FindProperty("P3"));

            obj.SetProperty(p1, "p4");
            Assert.Equal(2, obj.PropertyCount);
            Assert.Equal("p4", p1.Name);
            Assert.Null(obj.FindProperty("P1"));
            Assert.Same(p2, obj.FindProperty("P2"));
            Assert.Null(obj.FindProperty("P3"));
            Assert.Same(p1, obj.FindProperty("P4"));
            obj.SetProperty(null, "p4");
            Assert.Equal(1, obj.PropertyCount);
            Assert.Equal("p4", p1.Name);
            Assert.Null(obj.FindProperty("P1"));
            Assert.Same(p2, obj.FindProperty("P2"));
            Assert.Null(obj.FindProperty("P3"));
            Assert.Null(obj.FindProperty("P4"));

            var p5 = obj.GetProperty<CalProperty<int>>("p2");
            Assert.Same(p5, p2);
            Assert.Equal(1, obj.PropertyCount);
            p5 = obj.GetProperty<CalProperty<int>>("p3");
            Assert.NotSame(p5, p2);
            Assert.Equal(2, obj.PropertyCount);

            Assert.Same(p5, obj.GetProperty<CalProperty<int>>("p3", () => p1));
            Assert.Equal(2, obj.PropertyCount);
            Assert.Same(p1, obj.GetProperty<CalProperty<int>>("p5", () => p1));
            Assert.Equal(3, obj.PropertyCount);
            Assert.Equal("p5", p1.Name);

            Assert.Same(p1, obj.SetOrRemoveProperty<CalProperty<int>>("p5", true, p => p.Value = 999));
            Assert.Equal(999, p1.Value);
            p1.Value = 0;
            Assert.Null(obj.SetOrRemoveProperty<CalProperty<int>>("p5", false, p => p.Value = 999));
            Assert.Equal(0, p1.Value);
            var p6 = obj.SetOrRemoveProperty<CalProperty<int>>("p5", true, p => p.Value = 999);
            Assert.NotSame(p1, p6);
            Assert.Equal("p5", p6.Name);
            Assert.Equal(999, p6.Value);
            obj.RemoveProperty("p5");
            p6 = obj.SetOrRemoveProperty<CalProperty<int>>("p5", true);
            Assert.NotSame(p1, p6);
            Assert.Equal("p5", p6.Name);
            Assert.Equal(0, p6.Value);

            obj.ClearProperties();
            Assert.Equal(0, obj.PropertyCount);
        }

        [Fact]
        public void Reset()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            var obj = mObject.Object;

            obj.GetProperty<CalProperty<int>>("p1");
            obj.GetProperty<CalProperty<int>>("p2");
            obj.GetProperty<CalProperty<int>>("p3");
            Assert.Equal(3, obj.PropertyCount);

            obj.Reset();
            Assert.Equal(0, obj.PropertyCount);
        }

        [Fact]
        public void Serialize()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            mObject.SetupGet(o => o.Name).Returns("MyObject");
            var obj = mObject.Object;

            obj.GetProperty("p1", () => new TestProperty { Value = 123 });
            obj.GetProperty("p2", () => new TestProperty { Value = 456 });
            obj.GetProperty("p3", () => new TestProperty { Value = 789 });

            StringBuilder output = new StringBuilder();
            var mWriter = new Mock<ICalWriter>();
            ICalWriter writer = null;
            var parser = new CalendarParser();
            mWriter.SetupGet(w => w.Parser).Returns(parser);
            mWriter.Setup(w => w.WriteBegin(It.IsAny<string>())).Returns<string>(n => { output.AppendLine($"BEGIN:{n}"); return writer; });
            mWriter.Setup(w => w.WriteLine(It.IsAny<ContentLine>())).Returns<ContentLine>(l => { output.AppendLine(parser.EncodeContentLine(l)); return writer; });
            mWriter.Setup(w => w.WriteEnd(It.IsAny<string>())).Returns<string>(n => { output.AppendLine($"END:{n}"); return writer; });
            writer = mWriter.Object;

            obj.Serialize(writer);
            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:MyObject")
                .AppendLine("P1:123")
                .AppendLine("P2:456")
                .AppendLine("P3:789")
                .AppendLine("END:MyObject")
                .ToString()
                , output.ToString()
                );

            Assert.Throws<ArgumentNullException>(() => obj.Serialize(null));
        }

        [Fact]
        public void Deserialize()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            mObject.SetupGet(o => o.Name).Returns("MyObject");
            var obj = mObject.Object;

            obj.GetProperty("p1", () => new TestProperty { Value = 999 });
            obj.GetProperty("p6", () => new TestProperty { Value = 999 });
            obj.GetProperty("p2", () => new TestProperty { Value = 999 });

            var parser = new CalendarParser();
            using (var source = new StringReader(new StringBuilder()
                .AppendLine("BEGIN:MyObject")
                .AppendLine("END:MyObject")
                .ToString()))
            {
                var reader = new CalTextReader(parser, source, false);

                Assert.Equal(3, obj.PropertyCount);

                reader.ReadNextLine();
                obj.Deserialize(reader);
                Assert.Equal(0, obj.PropertyCount);
                mObject.Protected().Verify("CheckBeginLine", Times.Once(), ItExpr.IsAny<ContentLine>());
                mObject.Protected().Verify("InternalDeserialize", Times.Once(), reader);
                mObject.Protected().Verify("CheckEndLine", Times.Once(), ItExpr.IsAny<ContentLine>());

                Assert.Null(reader.ReadNextLine());
            }

            mObject.ResetCalls();
            using (var source = new StringReader(new StringBuilder()
                .AppendLine("BEGIN:Other")
                .AppendLine("END:Other")
                .ToString()))
            {
                var reader = new CalTextReader(parser, source, false);

                reader.ReadNextLine();
                obj.Deserialize(reader);
                Assert.Equal(0, obj.PropertyCount);
                mObject.Protected().Verify("CheckBeginLine", Times.Once(), ItExpr.IsAny<ContentLine>());
                mObject.Protected().Verify("InternalDeserialize", Times.Never(), reader);
                mObject.Protected().Verify("CheckEndLine", Times.Never(), ItExpr.IsAny<ContentLine>());

                Assert.NotNull(reader.ReadNextLine());
            }

            Assert.Throws<ArgumentNullException>(() => obj.Deserialize(null));
        }

    }
}
