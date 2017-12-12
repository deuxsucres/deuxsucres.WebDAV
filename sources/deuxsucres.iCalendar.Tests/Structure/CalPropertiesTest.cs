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
    public class CalPropertiesTest
    {
        [Fact]
        public void Create()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            var obj= mObject.Object;

            var props = new CalProperties<CalProperty<int>>("test", obj);
            Assert.Equal(0, props.Count);

            Assert.Throws<ArgumentNullException>(() => new CalProperties<CalProperty<int>>("test", null));
        }

        [Fact]
        public void Properties()
        {
            var mObject = new Mock<CalObject>() { CallBase = true };
            var obj = mObject.Object;

            var props = new CalProperties<CalProperty<int>>("test", obj);
            Assert.Equal(0, props.Count);

            var p1 = new CalProperty<int> { Value = 123 };
            var p2 = new CalProperty<string> { Name = "test", Value = "456" };
            var p3 = new CalProperty<int> { Name = "test", Value = 789 };

            props.Add(p1);
            Assert.Equal("test", p1.Name);
            obj.AddProperty(p2);
            obj.AddProperty(p3);
            var p4 = obj.GetProperty("other", () => new CalProperty<int> { Value = 222 });

            Assert.Equal(4, obj.PropertyCount);
            Assert.Equal(2, props.Count);
            Assert.Same(p1, props[0]);
            Assert.Same(p3, props[1]);

            Assert.Equal(new ICalProperty[] { p1, p3 }, props.ToArray());
            Assert.Equal(new ICalProperty[] { p1, p2, p3, p4 }, obj.GetProperties());
            props.Clear();
            Assert.Equal(1, obj.PropertyCount);
            Assert.Equal(0, props.Count);
            Assert.Equal(new ICalProperty[] { }, props.ToArray());
            Assert.Equal(new ICalProperty[] { p4 }, obj.GetProperties());

            Assert.NotNull(((IEnumerable)props).GetEnumerator());
        }

    }
}
