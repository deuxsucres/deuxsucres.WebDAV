using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentWriterTest
    {
        [Fact]
        public void Create()
        {
            var source = new StringWriter();
            ContentWriter writer;
            using (writer = new ContentWriter(source))
                Assert.Same(source, writer.Source);

            using (writer = new ContentWriter(source, true))
                Assert.Same(source, writer.Source);

            using (writer = new ContentWriter(source, false))
                Assert.Same(source, writer.Source);

            Assert.Throws<ArgumentNullException>(() => new ContentWriter(null));
            Assert.Throws<ArgumentNullException>(() => new ContentWriter(null, true));
        }

        [Fact]
        public void TestDispose()
        {
            var source = new StringWriter();
            ContentWriter writer;
            using (writer = new ContentWriter(source))
            {
                Assert.Same(source, writer.Source);
            }
            Assert.Throws<ObjectDisposedException>(() => writer.WriteLine("Test"));
            Assert.Null(writer.Source);

            using (writer = new ContentWriter(source, false))
            {
                Assert.Same(source, writer.Source);
            }
            Assert.Null(writer.Source);

            using (writer = new ContentWriter(source, true))
            {
                Assert.Same(source, writer.Source);
            }
            Assert.Same(source, writer.Source);
        }

        [Fact]
        public void WriteLine()
        {
            var source = new StringWriter();
            using (var writer = new ContentWriter(source))
            {
                Assert.Equal(75, writer.LineSize);

                writer.LineSize = 0;
                Assert.Equal(1, writer.WriteLine("DESCRIPTION:This is a long description text in a only one line of text."));

                writer.LineSize = 40;
                Assert.Equal(1, writer.WriteLine("DESCRIPTION:This is the second line."));

                Assert.Equal(0, writer.WriteLine(""));
                Assert.Equal(0, writer.WriteLine(null));

                writer.LineSize = 26;
                Assert.Equal(3, writer.WriteLine("DESCRIPTION:This is a long description text splitted in three lines of text."));

                Assert.Equal(1, writer.WriteLine("DESCRIPTION:The last line."));
            }

            StringBuilder content = new StringBuilder()
                .AppendLine("DESCRIPTION:This is a long description text in a only one line of text.")
                .AppendLine("DESCRIPTION:This is the second line.")
                .AppendLine("DESCRIPTION:This is a long")
                .AppendLine("  description text splitte")
                .AppendLine(" d in three lines of text.")
                .AppendLine("DESCRIPTION:The last line.")
                ;
            Assert.Equal(content.ToString(), source.GetStringBuilder().ToString());
        }

    }
}
