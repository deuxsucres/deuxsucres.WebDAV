using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class OrganizerPropertyTest
    {

        [Fact]
        public void Cast()
        {
            var prop = new OrganizerProperty();
            Assert.Null(prop.Value);
            Assert.Null((Uri)prop);
            Assert.Null((string)prop);

            prop = "mailto:yan@deuxsucres.com";
            Assert.Equal(new Uri("mailto:yan@deuxsucres.com"), prop.Value);
            Assert.Equal(new Uri("mailto:yan@deuxsucres.com"), (Uri)prop);
            Assert.Equal("mailto:yan@deuxsucres.com", (string)prop);

            prop = new Uri("mailto:yan.grenier@deuxsucres.com");
            Assert.Equal(new Uri("mailto:yan.grenier@deuxsucres.com"), prop.Value);
            Assert.Equal(new Uri("mailto:yan.grenier@deuxsucres.com"), (Uri)prop);
            Assert.Equal("mailto:yan.grenier@deuxsucres.com", (string)prop);
        }

        [Fact]
        public void Parameters()
        {
            var prop = new OrganizerProperty();
            Assert.Equal("ORGANIZER", prop.Name);
            Assert.Null(prop.Value);
            Assert.Null(prop.Language);
            Assert.Null(prop.CN);
            Assert.Null(prop.Dir);
            Assert.Null(prop.Role);
            Assert.Null(prop.SentBy);

            prop.Value = new Uri("mailto:yan@deuxsucres");
            prop.Language="FR";
            prop.CN = "CN";
            prop.Dir = "http://www.deuxsucres.com";
            prop.Role = Roles.Chair;
            prop.SentBy = "mailto:yan.grenier@deuxsucres.com";

            Assert.Equal(new Uri("mailto:yan@deuxsucres"), prop.Value);
            Assert.Equal("FR", prop.Language);
            Assert.Equal("CN", prop.CN);
            Assert.Equal("http://www.deuxsucres.com/", prop.Dir);
            Assert.Equal(Roles.Chair, (Roles)prop.Role);
            Assert.Equal("mailto:yan.grenier@deuxsucres.com", prop.SentBy);
        }

    }
}
