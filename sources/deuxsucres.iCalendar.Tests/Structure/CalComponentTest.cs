using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class CalComponentTest
    {
        [Fact]
        public void Create()
        {
            var comp = new Mock<CalComponent>().Object;

            Assert.Null(comp.Calendar);
            Assert.NotNull(comp.ExtraComponents);
            Assert.Empty(comp.ExtraComponents);
        }

        [Fact]
        public void Reset()
        {
            var comp1 = new Mock<CalComponent>() { CallBase = true }.Object;
            var comp2 = new Mock<CalComponent>() { CallBase = true }.Object;
            var comp3 = new Mock<CalComponent>() { CallBase = true }.Object;

            comp1.ExtraComponents.Add(comp2);
            comp1.ExtraComponents.Add(comp3);
            Assert.Equal(2, comp1.ExtraComponents.Count);
            comp1.Reset();
            Assert.Empty(comp1.ExtraComponents);
        }
    }
}
