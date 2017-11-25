using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Serialization
{
    public class CalTextReaderTest
    {
        class TestComponent : CalComponent
        {
            public override string Name => "Test";
        }

        [Fact]
        public void Create()
        {
            var parser = new CalendarParser();
            var treader = new StringReader("");
            var reader = new CalTextReader(parser, treader, false);
            Assert.Same(parser, reader.Parser);
            Assert.Same(treader, reader.Source);
            Assert.False(reader.StrictMode);

            reader = new CalTextReader(parser, treader, true);
            Assert.Same(parser, reader.Parser);
            Assert.Same(treader, reader.Source);
            Assert.True(reader.StrictMode);

            Assert.Throws<ArgumentNullException>(() => new CalTextReader(null, treader, false));
            Assert.Throws<ArgumentNullException>(() => new CalTextReader(parser, null, false));
        }

        [Fact]
        public void Read()
        {
            var parser = new CalendarParser();
            var treader = new StringReader(new StringBuilder()
                .AppendLine("DESC:This is the first line\n  containing a long data string content")
                .AppendLine("DESC:Another\n  line\n  for test")
                .AppendLine("BEGIN:Test")
                .AppendLine("Prop:Value")
                .AppendLine("END:Test")
                .AppendLine("BEGIN:VTODO")
                .AppendLine("P:V")
                .AppendLine("END:VTODO")
                .ToString());
            ICalReader reader = new CalTextReader(parser, treader, false);

            Assert.Null(reader.CurrentLine);
            Assert.Equal(1, reader.CurrentLineNumber);

            // Read the first line
            var line = reader.ReadNextLine();
            Assert.Same(line, reader.CurrentLine);
            Assert.Equal(1, reader.CurrentLineNumber);
            Assert.Equal("DESC", line.Name);
            Assert.Equal("This is the first line containing a long data string content", line.Value);

            // Read the second line
            line = reader.ReadNextLine();
            Assert.Same(line, reader.CurrentLine);
            Assert.Equal(3, reader.CurrentLineNumber);
            Assert.Equal("DESC", line.Name);
            Assert.Equal("Another line for test", line.Value);

            // Convert the line to default property
            var prop = reader.MakeProperty(line);
            Assert.IsType<TextProperty>(prop);
            Assert.Equal("DESC", prop.Name);
            Assert.Equal("Another line for test", ((TextProperty)prop).Value);
            Assert.Null(reader.MakeProperty(null));

            // Convert the line to typed property
            var tprop = reader.MakeProperty<TextProperty>(line);
            Assert.IsType<TextProperty>(prop);
            Assert.Equal("DESC", prop.Name);
            Assert.Equal("Another line for test", tprop.Value);
            Assert.Null(reader.MakeProperty<TextProperty>(null));

            // Test parameter creation
            var param = reader.CreateDefaultParameter("Test");
            Assert.IsType<TextParameter>(param);
            Assert.Equal("Test", param.Name);
            Assert.IsType<TextParameter>(reader.CreateDefaultParameter(null));

            // Read the first component
            line = reader.ReadNextLine();
            Assert.Equal(6, reader.CurrentLineNumber);
            Assert.Equal(Constants.BEGIN, line.Name);

            // By default this component not exist so failed to create it
            Assert.Throws<CalSyntaxError>(() => reader.ReadComponent(line));
            Assert.Null(reader.ReadComponent(null));

            Assert.Null(reader.ReadComponent<TestComponent>(null));
            var tcomp = reader.ReadComponent<TestComponent>(line);
            Assert.Equal(1, tcomp.PropertyCount);
            Assert.Equal("Prop", tcomp.GetProperties().First().Name);

            // Read the second component
            line = reader.ReadNextLine();
            Assert.Equal(9, reader.CurrentLineNumber);
            Assert.Equal(Constants.BEGIN, line.Name);
            tcomp = reader.ReadComponent<TestComponent>(line);
            Assert.Equal(1, tcomp.PropertyCount);
            Assert.Equal("P", tcomp.GetProperties().First().Name);

            // Check the end of read
            Assert.Null(reader.ReadNextLine());
            Assert.Null(reader.CurrentLine);
            Assert.Equal(12, reader.CurrentLineNumber);

            // Test a reader with default component creation
            parser = new CalendarParser();
            treader = new StringReader(new StringBuilder()
                .AppendLine("BEGIN:Test")
                .AppendLine("Prop:Value")
                .AppendLine("BEGIN:VTODO")
                .AppendLine("P:V")
                .AppendLine("END:VTODO")
                .AppendLine("END:Test")
                .ToString());
            var mReader = new Mock<CalTextReader>(parser, treader, false) { CallBase = true };
            mReader.Setup(r => r.CreateDefaultComponent(It.IsAny<string>()))
                .Returns(() => new TestComponent());
            reader = mReader.Object;

            line = reader.ReadNextLine();
            Assert.Equal(1, reader.CurrentLineNumber);
            Assert.Equal(Constants.BEGIN, line.Name);
            var comp = reader.ReadComponent(line);
            Assert.Equal(1, comp.PropertyCount);
            Assert.Single(comp.ExtraComponents);

            Assert.Null(reader.ReadNextLine());
            Assert.Null(reader.CurrentLine);
            Assert.Equal(7, reader.CurrentLineNumber);

        }

    }
}
