using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Serialization
{
    public class CalTextWriterTest
    {
        [Fact]
        public void Create()
        {
            var parser = new CalendarParser();
            var twriter = new StringWriter();
            var writer = new CalTextWriter(parser, twriter);
            Assert.Same(parser, writer.Parser);
            Assert.Same(twriter, writer.Source);

            Assert.Throws<ArgumentNullException>(() => new CalTextWriter(null, twriter));
            Assert.Throws<ArgumentNullException>(() => new CalTextWriter(parser, null));
        }

        [Fact]
        public void Write()
        {
            var parser = new CalendarParser();
            var output = new StringBuilder();
            using (var twriter = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, twriter);
                writer.WriteBegin("Test");
                var content = new ContentLine { Name = "Prop", Value = "Value" };
                content.SetParam("P1", "1").SetParam("p2", "2");
                writer.WriteLine(content);
                writer.WriteEnd("Test");
            }
            Assert.Equal(new StringBuilder()
                .AppendLine("BEGIN:TEST")
                .AppendLine("Prop;P1=1;p2=2:Value")
                .AppendLine("END:TEST")
                .ToString(), output.ToString());
        }
    }
}
