using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure.Properties
{
    public class ExtendedTextPropertyTest
    {
        [Fact]
        public void Test()
        {
            var prop = new ExtendedTextProperty();
            Assert.Null(prop.AltRep);
            Assert.Null(prop.Language);

            prop.Value = "My text";
            prop.AltRep = "My current text";
            prop.Language = "fr";
            Assert.NotNull(prop.AltRep);
            Assert.NotNull(prop.Language);
            Assert.Equal("My text", prop.Value);
            Assert.Equal("My current text", prop.AltRep);
            Assert.Equal("fr", prop.Language);

            Assert.Equal(2, prop.ParameterCount);

            prop.AltRep = null;
            prop.Language = null;
            Assert.Null(prop.AltRep);
            Assert.Null(prop.Language);
            Assert.Equal(0, prop.ParameterCount);

            prop = new ExtendedTextProperty
            {
                Value = "Another text",
                Language = "en"
            };
            Assert.Null(prop.AltRep);
            Assert.NotNull(prop.Language);
            Assert.Equal("Another text", prop.Value);
            Assert.Equal("en", prop.Language);
            Assert.Equal(1, prop.ParameterCount);
            Assert.Equal("Another text", prop);

            prop = "Last test";
            Assert.Null(prop.Name);
            Assert.Null(prop.AltRep);
            Assert.Null(prop.Language);
            Assert.Equal("Last test", prop.Value);
            Assert.Equal(0, prop.ParameterCount);
            Assert.Equal("Last test", prop);

            prop = (string)null;
            Assert.Null(prop);
        }
    }
}
