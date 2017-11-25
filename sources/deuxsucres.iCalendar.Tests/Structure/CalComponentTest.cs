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

    }
}
