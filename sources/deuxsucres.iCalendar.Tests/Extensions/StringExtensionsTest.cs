using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Extensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void IsEqual()
        {
            Assert.True("Test".IsEqual("tesT"));
            Assert.False("Test".IsEqual(null));
            string s = null;
            Assert.True(s.IsEqual(null));
        }
    }
}
