using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentReaderTest
    {
        [Fact]
        public void Create()
        {
            var source = new StringReader("");
            ContentReader reader;
            using (reader = new ContentReader(source))
                Assert.Same(source, reader.Source);

            using (reader = new ContentReader(source, true))
                Assert.Same(source, reader.Source);

            using (reader = new ContentReader(source, false))
                Assert.Same(source, reader.Source);

            Assert.Throws<ArgumentNullException>(() => new ContentReader(null));
            Assert.Throws<ArgumentNullException>(() => new ContentReader(null, true));
        }

        [Fact]
        public void TestDispose()
        {
            var source = new StringReader("");
            ContentReader reader;
            using (reader = new ContentReader(source))
            {
                Assert.Same(source, reader.Source);
            }
            Assert.Throws<ObjectDisposedException>(() => reader.ReadLine());
            Assert.Null(reader.Source);

            using (reader = new ContentReader(source, false))
            {
                Assert.Same(source, reader.Source);
            }
            Assert.Null(reader.Source);

            using (reader = new ContentReader(source, true))
            {
                Assert.Same(source, reader.Source);
            }
            Assert.Same(source, reader.Source);
        }

        [Fact]
        public void ReadLine()
        {
            StringBuilder content = new StringBuilder()
                .AppendLine("DESCRIPTION:This is a long description text in a only one line of text.")
                .AppendLine("DESCRIPTION:This is the second line.")
                .AppendLine("")
                .AppendLine("DESCRIPTION:This is a lo")
                .AppendLine(" ng description text")
                .AppendLine("  splitted in three lines of text.")
                .AppendLine("")
                .AppendLine("DESCRIPTION:The last line.")
                ;
            var source = new StringReader(content.ToString());
            ContentReader reader;
            using (reader = new ContentReader(source))
            {
                Assert.Equal("DESCRIPTION:This is a long description text in a only one line of text.", reader.ReadLine());
                Assert.Equal("DESCRIPTION:This is the second line.", reader.ReadLine());
                Assert.Equal("DESCRIPTION:This is a long description text splitted in three lines of text.", reader.ReadLine());
                Assert.Equal("DESCRIPTION:The last line.", reader.ReadLine());
                Assert.Null(reader.ReadLine());
            }
        }

    }
}
