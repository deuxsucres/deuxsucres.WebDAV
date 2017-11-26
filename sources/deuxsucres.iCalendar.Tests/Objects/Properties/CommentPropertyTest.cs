using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class CommentPropertyTest
    {
        [Fact]
        public void Test()
        {
            var prop = new CommentProperty();
            Assert.Equal("COMMENT", prop.Name);
            Assert.Null(prop.AltRep);
            Assert.Null(prop.Language);

            prop = new CommentProperty
            {
                Value = "My comment",
                Language = "en"
            };
            Assert.Equal("COMMENT", prop.Name);
            Assert.Null(prop.AltRep);
            Assert.NotNull(prop.Language);
            Assert.Equal("My comment", prop.Value);
            Assert.Equal("en", prop.Language);
            Assert.Equal(1, prop.ParameterCount);
            Assert.Equal("My comment", prop);

            prop = "Another comment";
            Assert.Equal("COMMENT", prop.Name);
            Assert.Null(prop.AltRep);
            Assert.Null(prop.Language);
            Assert.Equal("Another comment", prop.Value);
            Assert.Equal(0, prop.ParameterCount);
            Assert.Equal("Another comment", prop);

            prop = (string)null;
            Assert.Null(prop);
            Assert.Null((string)prop);
        }
    }
}
