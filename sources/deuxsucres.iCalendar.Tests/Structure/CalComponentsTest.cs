using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure
{
    public class CalComponentsTest
    {
        class TestComponent : CalComponent
        {
            public override string Name => "Test";
        }

        [Fact]
        public void Create()
        {
            var source = new List<CalComponent>();
            var comps = new CalComponents<TestComponent>(source);
            Assert.Equal(0, comps.Count);

            Assert.Throws<ArgumentNullException>(() => new CalComponents<TestComponent>(null));
        }

        [Fact]
        public void Components()
        {
            var c1 = new TestComponent();
            var c2 = new Mock<CalComponent>() { CallBase = true }.Object;
            var c3 = new TestComponent();
            var c4 = new TestComponent();

            var source = new List<CalComponent> { c1, c2, c3 };

            var comps = new CalComponents<TestComponent>(source);
            Assert.Equal(2, comps.Count);

            comps.Add(c4);
            Assert.Equal(3, comps.Count);
            comps.Add(c4);
            Assert.Equal(3, comps.Count);

            var c5 = comps.CreateNew();
            Assert.Equal(4, comps.Count);

            Assert.Equal(new CalComponent[] { c1, c2, c3, c4, c5 }, source);
            Assert.Equal(new CalComponent[] { c1, c3, c4, c5 }, comps);

            Assert.Same(c1, comps[0]);
            Assert.Same(c3, comps[1]);
            Assert.Same(c4, comps[2]);

            Assert.True(comps.Remove(c4));
            Assert.False(comps.Remove(c4));

            Assert.Equal(new CalComponent[] { c1, c2, c3, c5 }, source);
            Assert.Equal(new CalComponent[] { c1, c3, c5 }, comps);

            comps.Clear();
            Assert.Equal(new CalComponent[] { c2}, source);
            Assert.Equal(new CalComponent[] { }, comps);

            Assert.NotNull(((IEnumerable)comps).GetEnumerator());
        }

    }
}
