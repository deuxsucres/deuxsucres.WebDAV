using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests
{
    public class CalSyntaxErrorTest
    {
        [Fact]
        public void Create()
        {
            var ex = new CalSyntaxError("test");
            Assert.Equal("test", ex.Message);
            Assert.Null(ex.InnerException);

            var iex = new InvalidOperationException("test");
            ex = new CalSyntaxError("test", iex);
            Assert.Equal("test", ex.Message);
            Assert.Same(iex, ex.InnerException);
        }
    }
}
