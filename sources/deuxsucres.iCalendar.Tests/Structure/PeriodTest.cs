using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class PeriodTest
    {
        [Fact]
        public void TestPeriod()
        {
            DateTime dt1 = new DateTime(2017, 11, 16, 4, 32, 48);
            DateTime dt2 = new DateTime(2017, 11, 18, 17, 17, 23);
            TimeSpan delay = dt2 - dt1;

            var period = new Period(dt1, dt2);
            Assert.Equal(dt1, period.DateStart);
            Assert.Equal(dt2, period.DateEnd);
            Assert.Equal(dt2, period.GetDateEnd());
            Assert.Equal(delay, period.GetDuration());
            Assert.Equal(746391939, period.GetHashCode());
            Assert.True(period.Equals(new Period(dt1, dt2)));
            Assert.False(period.Equals(new Period(dt2, delay)));
            Assert.False(period.Equals(new Period(dt1, delay)));

            period = new Period(dt2, dt1);
            Assert.Equal(dt1, period.DateStart);
            Assert.Equal(dt2, period.DateEnd);
            Assert.Equal(dt2, period.GetDateEnd());
            Assert.Equal(delay, period.GetDuration());
            Assert.Equal(746391939, period.GetHashCode());
            Assert.True(period.Equals(new Period(dt1, dt2)));
            Assert.False(period.Equals(new Period(dt2, delay)));
            Assert.False(period.Equals(new Period(dt1, delay)));

            period = new Period(dt1, delay);
            Assert.Equal(dt1, period.DateStart);
            Assert.Null(period.DateEnd);
            Assert.Equal(dt2, period.GetDateEnd());
            Assert.Equal(delay, period.GetDuration());
            Assert.Equal(611646077, period.GetHashCode());
            Assert.False(period.Equals(new Period(dt1, dt2)));
            Assert.False(period.Equals(new Period(dt2, delay)));
            Assert.True(period.Equals(new Period(dt1, delay)));

            Assert.False(period.Equals(123));
        }
    }
}
