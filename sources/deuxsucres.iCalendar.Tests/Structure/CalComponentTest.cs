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
    public class CalComponentTest
    {
        [Fact]
        public void Create()
        {
            var comp = new Mock<CalComponent>().Object;

            Assert.Null(comp.Calendar);
            Assert.NotNull(comp.ExtraComponents);
            Assert.Empty(comp.ExtraComponents);
        }

        [Fact]
        public void Reset()
        {
            var comp1 = new Mock<CalComponent>() { CallBase = true }.Object;
            var comp2 = new Mock<CalComponent>() { CallBase = true }.Object;
            var comp3 = new Mock<CalComponent>() { CallBase = true }.Object;

            comp1.ExtraComponents.Add(comp2);
            comp1.ExtraComponents.Add(comp3);
            Assert.Equal(2, comp1.ExtraComponents.Count);
            comp1.Reset();
            Assert.Empty(comp1.ExtraComponents);
        }

        [Fact]
        public void Serialize()
        {
            var mComp2 = new Mock<CalComponent>() { CallBase = true };
            mComp2.SetupGet(o => o.Name).Returns("MyComponent1");
            var comp2 = mComp2.Object;
            comp2.GetProperty("p4", () => new IntegerProperty { Value = 123 });
            comp2.GetProperty("p5", () => new IntegerProperty { Value = 456 });
            comp2.GetProperty("p6", () => new IntegerProperty { Value = 789 });

            var mComp3 = new Mock<CalComponent>() { CallBase = true };
            mComp3.SetupGet(o => o.Name).Returns("MyComponent2");
            var comp3 = mComp3.Object;
            comp3.GetProperty("p7", () => new IntegerProperty { Value = 123 });
            comp3.GetProperty("p8", () => new IntegerProperty { Value = 456 });
            comp3.GetProperty("p9", () => new IntegerProperty { Value = 789 });

            var mComp = new Mock<CalComponent>() { CallBase = true };
            mComp.SetupGet(o => o.Name).Returns("MyObject");
            var comp = mComp.Object;
            comp.GetProperty("p1", () => new IntegerProperty { Value = 123 });
            comp.GetProperty("p2", () => new IntegerProperty { Value = 456 });
            comp.GetProperty("p3", () => new IntegerProperty { Value = 789 });
            comp.ExtraComponents.Add(comp2);
            comp.ExtraComponents.Add(comp3);

            StringBuilder output = new StringBuilder();
            var mWriter = new Mock<ICalWriter>();
            ICalWriter writer = null;
            var parser = new CalendarParser();
            mWriter.SetupGet(w => w.Parser).Returns(parser);
            mWriter.Setup(w => w.WriteBegin(It.IsAny<string>())).Returns<string>(n => { output.AppendLine($"BEGIN:{n}"); return writer; });
            mWriter.Setup(w => w.WriteLine(It.IsAny<ContentLine>())).Returns<ContentLine>(l => { output.AppendLine(parser.EncodeContentLine(l)); return writer; });
            mWriter.Setup(w => w.WriteEnd(It.IsAny<string>())).Returns<string>(n => { output.AppendLine($"END:{n}"); return writer; });
            writer = mWriter.Object;

            comp.Serialize(writer);
            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:MyObject")
                .AppendLine("P1:123")
                .AppendLine("P2:456")
                .AppendLine("P3:789")
                .AppendLine("BEGIN:MyComponent1")
                .AppendLine("P4:123")
                .AppendLine("P5:456")
                .AppendLine("P6:789")
                .AppendLine("END:MyComponent1")
                .AppendLine("BEGIN:MyComponent2")
                .AppendLine("P7:123")
                .AppendLine("P8:456")
                .AppendLine("P9:789")
                .AppendLine("END:MyComponent2")
                .AppendLine("END:MyObject")
                .ToString()
                , output.ToString()
                );

            Assert.Throws<ArgumentNullException>(() => comp.Serialize(null));
        }

        [Fact]
        public void Deserialize()
        {
            var mComp = new Mock<CalComponent>() { CallBase = true };
            mComp.SetupGet(o => o.Name).Returns("MyObject");
            // This component 'Process' the property 'P0' so this property never added in the propery list
            mComp.Protected().Setup<bool>("ProcessProperty", ItExpr.IsAny<ICalReader>(), ItExpr.IsAny<ContentLine>())
                .Returns<ICalReader, ContentLine>((r, l) => l.Name == "P0");
            // This component 'Process' the component 'MyComponent3' so this component never added in the extra component list
            mComp.Protected().Setup<bool>("ProcessComponent", ItExpr.IsAny<CalComponent>())
                .Returns<CalComponent>((c) => c.Name == "MyComponent3");
            var comp = mComp.Object;

            var mReader = new Mock<ICalReader>();
            ICalReader reader = null;
            List<ContentLine> lines = new List<ContentLine> {
                new ContentLine { Name = "BEGIN", Value = "MyObject" },
                new ContentLine { Name = "P1", Value = "123" },
                new ContentLine { Name = "P0", Value = "000" },
                new ContentLine { Name = "P2", Value = "456" },
                new ContentLine { Name = "P3", Value = "789" },
                new ContentLine { Name = "BEGIN", Value = "MyComponent1" },
                new ContentLine { Name = "P4", Value = "123" },
                new ContentLine { Name = "BEGIN", Value = "MyComponent3" },
                new ContentLine { Name = "P0", Value = "123" },
                new ContentLine { Name = "P1", Value = "456" },
                new ContentLine { Name = "P9", Value = "789" },
                new ContentLine { Name = "END", Value = "MyComponent3" },
                new ContentLine { Name = "P5", Value = "456" },
                new ContentLine { Name = "P6", Value = "789" },
                new ContentLine { Name = "END", Value = "MyComponent1" },
                new ContentLine { Name = "BEGIN", Value = "MyComponent2" },
                new ContentLine { Name = "P7", Value = "123" },
                new ContentLine { Name = "P8", Value = "456" },
                new ContentLine { Name = "P9", Value = "789" },
                new ContentLine { Name = "END", Value = "MyComponent2" },
                new ContentLine { Name = "BEGIN", Value = "MyComponent3" },
                new ContentLine { Name = "P0", Value = "123" },
                new ContentLine { Name = "P1", Value = "456" },
                new ContentLine { Name = "P9", Value = "789" },
                new ContentLine { Name = "END", Value = "MyComponent3" },
                new ContentLine { Name = "END", Value = "MyObject" },
            };
            int currLineIndex = 0;
            ContentLine currLine = null;
            Func<ContentLine, ContentLine> setCurrLine = c => currLine = c;
            var parser = new CalendarParser();
            mReader.SetupGet(w => w.Parser).Returns(parser);
            mReader.SetupGet(w => w.CurrentLine).Returns(() => currLine);
            mReader.Setup(w => w.MakeProperty(It.IsAny<ContentLine>())).Returns<ContentLine>(l =>
            {
                return new IntegerProperty { Name = l.Name, Value = int.Parse(l.Value) };
            });
            mReader.Setup(w => w.ReadComponent(It.IsAny<ContentLine>())).Returns<ContentLine>(l =>
            {
                var mC = new Mock<CalComponent>() { CallBase = true };
                mC.SetupGet(o => o.Name).Returns(l.Value);
                var c = mC.Object;
                c.Deserialize(reader);
                return c;
            });
            mReader.Setup(r => r.ReadNextLine()).Returns(() =>
            {
                if (currLineIndex >= lines.Count)
                {
                    currLine = null;
                }
                else
                {
                    currLine = lines[currLineIndex++];
                }
                return currLine;
            });
            reader = mReader.Object;

            reader.ReadNextLine();
            comp.Deserialize(reader);
            Assert.Equal(3, comp.PropertyCount);
            Assert.Equal(2, comp.ExtraComponents.Count);

            // Check the current component
            Assert.Equal("MyObject", comp.Name);
            var props = comp.GetProperties().ToArray();
            Assert.Equal("P1", props[0].Name);
            Assert.IsType<IntegerProperty>(props[0]);
            Assert.Equal(123, ((IntegerProperty)props[0]).Value);
            Assert.Equal("P2", props[1].Name);
            Assert.IsType<IntegerProperty>(props[1]);
            Assert.Equal(456, ((IntegerProperty)props[1]).Value);
            Assert.Equal("P3", props[2].Name);
            Assert.IsType<IntegerProperty>(props[2]);
            Assert.Equal(789, ((IntegerProperty)props[2]).Value);

            // Check the first extra component
            var ecomp = comp.ExtraComponents[0];
            Assert.Equal("MyComponent1", ecomp.Name);
            Assert.Equal(3, ecomp.PropertyCount);
            Assert.Single(ecomp.ExtraComponents);
            props = ecomp.GetProperties().ToArray();
            Assert.Equal("P4", props[0].Name);
            Assert.IsType<IntegerProperty>(props[0]);
            Assert.Equal(123, ((IntegerProperty)props[0]).Value);
            Assert.Equal("P5", props[1].Name);
            Assert.IsType<IntegerProperty>(props[1]);
            Assert.Equal(456, ((IntegerProperty)props[1]).Value);
            Assert.Equal("P6", props[2].Name);
            Assert.IsType<IntegerProperty>(props[2]);
            Assert.Equal(789, ((IntegerProperty)props[2]).Value);

            // Check the sub-component of the first extra component
            ecomp = ecomp.ExtraComponents[0];
            Assert.Equal("MyComponent3", ecomp.Name);
            Assert.Equal(3, ecomp.PropertyCount);
            Assert.Empty(ecomp.ExtraComponents);
            props = ecomp.GetProperties().ToArray();
            Assert.Equal("P0", props[0].Name);
            Assert.IsType<IntegerProperty>(props[0]);
            Assert.Equal(123, ((IntegerProperty)props[0]).Value);
            Assert.Equal("P1", props[1].Name);
            Assert.IsType<IntegerProperty>(props[1]);
            Assert.Equal(456, ((IntegerProperty)props[1]).Value);
            Assert.Equal("P9", props[2].Name);
            Assert.IsType<IntegerProperty>(props[2]);
            Assert.Equal(789, ((IntegerProperty)props[2]).Value);

            // Check the second extra component
            ecomp = comp.ExtraComponents[1];
            Assert.Equal("MyComponent2", ecomp.Name);
            Assert.Equal(3, ecomp.PropertyCount);
            Assert.Empty(ecomp.ExtraComponents);
            props = ecomp.GetProperties().ToArray();
            Assert.Equal("P7", props[0].Name);
            Assert.IsType<IntegerProperty>(props[0]);
            Assert.Equal(123, ((IntegerProperty)props[0]).Value);
            Assert.Equal("P8", props[1].Name);
            Assert.IsType<IntegerProperty>(props[1]);
            Assert.Equal(456, ((IntegerProperty)props[1]).Value);
            Assert.Equal("P9", props[2].Name);
            Assert.IsType<IntegerProperty>(props[2]);
            Assert.Equal(789, ((IntegerProperty)props[2]).Value);

            Assert.Throws<ArgumentNullException>(() => comp.Deserialize(null));
        }

    }
}
