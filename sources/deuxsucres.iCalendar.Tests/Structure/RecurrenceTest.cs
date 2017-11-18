using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class RecurrenceTest
    {
        [Fact]
        public void Create()
        {
            var rec = new Recurrence();
            Assert.Equal(Recurrence.Weekdays.MO, rec.StartWeek);
        }

        [Fact]
        public void Other()
        {
            var rec = new Recurrence();
            Assert.Null(rec.Others);
            rec.AddOther("name", "value");
            Assert.NotNull(rec.Others);
            Assert.Equal(new Dictionary<string, string>()
            {
                { "name", "value" }
            }, rec.Others);
        }

    }
}
