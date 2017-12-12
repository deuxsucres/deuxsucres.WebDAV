using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class RecurRulePropertyTest
    {
        [Fact]
        public void Test()
        {
            var prop = new RecurRuleProperty();
            Assert.Equal("RRULE", prop.Name);

            Recurrence rec = prop;
            Assert.Null(rec);

            prop.Value = new Recurrence { Count = 2 };
            rec = prop;
            Assert.NotNull(rec);
            Assert.Equal(2, rec.Count);

            rec = new Recurrence() { };
            prop = rec;
            Assert.Same(rec, prop.Value);

            rec = null;
            prop = rec;
            Assert.Null(prop);
        }
    }
}
