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
    }
}
