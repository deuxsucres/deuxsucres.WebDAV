using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class TzNamePropertyTest
    {
        [Fact]
        public void Test()
        {
            var prop = new TzNameProperty();
            Assert.Equal("TZNAME", prop.Name);
            Assert.Null(prop.Language);

            prop = new TzNameProperty
            {
                Value = "The name",
                Language = "en"
            };
            Assert.Equal("TZNAME", prop.Name);
            Assert.NotNull(prop.Language);
            Assert.Equal("The name", prop.Value);
            Assert.Equal("en", prop.Language);
            Assert.Equal(1, prop.ParameterCount);
            Assert.Equal("The name", prop);

            prop = "Another name";
            Assert.Equal("TZNAME", prop.Name);
            Assert.Null(prop.Language);
            Assert.Equal("Another name", prop.Value);
            Assert.Equal(0, prop.ParameterCount);
            Assert.Equal("Another name", prop);

            prop = (string)null;
            Assert.Null(prop);
            Assert.Null((string)prop);
        }
    }
}
