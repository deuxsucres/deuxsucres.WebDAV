using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentSyntaxTest
    {
        [Fact]
        public void TestChars()
        {
            Assert.True(ContentSyntax.IsALPHA('f'));
            Assert.False(ContentSyntax.IsALPHA('6'));

            Assert.True(ContentSyntax.IsWSP(' '));
            Assert.False(ContentSyntax.IsWSP('r'));

            Assert.True(ContentSyntax.IsVCHAR(' '));
            Assert.False(ContentSyntax.IsVCHAR('\x09'));

            Assert.False(ContentSyntax.IsCTL(' '));
            Assert.True(ContentSyntax.IsCTL('\x09'));

            Assert.False(ContentSyntax.IsDIGIT('f'));
            Assert.True(ContentSyntax.IsDIGIT('6'));

            Assert.False(ContentSyntax.IsHEXDIGIT('Z'));
            Assert.True(ContentSyntax.IsHEXDIGIT('f'));
            Assert.True(ContentSyntax.IsHEXDIGIT('6'));

            Assert.False(ContentSyntax.IsBIT('f'));
            Assert.True(ContentSyntax.IsBIT('0'));

            Assert.False(ContentSyntax.IsCHAR('\x00'));
            Assert.True(ContentSyntax.IsCHAR('0'));
        }
    }
}
