using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentSyntaxErrorTest
    {
        [Fact]
        public void Test()
        {
            var error = new ContentSyntaxError("message");
            Assert.Equal("message", error.ErrorMessage);
            Assert.Equal(-1, error.LineNumber);
            Assert.Equal(-1, error.LinePosition);
            Assert.Equal("Syntax error (-1, -1) : message", error.Message);

            error = new ContentSyntaxError("message", 1, 2);
            Assert.Equal("message", error.ErrorMessage);
            Assert.Equal(1, error.LineNumber);
            Assert.Equal(2, error.LinePosition);
            Assert.Equal("Syntax error (1, 2) : message", error.Message);
        }
    }
}
