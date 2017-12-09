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
    }
}
