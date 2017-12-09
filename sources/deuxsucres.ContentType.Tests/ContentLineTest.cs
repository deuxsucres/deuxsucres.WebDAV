using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentLineTest
    {
        [Fact]
        public void Create()
        {
            var line = new ContentLine();
            Assert.Null(line.Group);
            Assert.Null(line.Name);
            Assert.Null(line.FullName);
            Assert.Null(line.Value);
            Assert.Equal(0, line.ParamCount);
        }

        [Fact]
        public void Group()
        {
            var content = new ContentLine();
            Assert.Null(content.Group);
            content.Group = "test";
            Assert.Equal("test", content.Group);
        }

        [Fact]
        public void Name()
        {
            var content = new ContentLine();
            Assert.Null(content.Name);
            content.Name = "test";
            Assert.Equal("test", content.Name);
        }

        [Fact]
        public void FullName()
        {
            var content = new ContentLine();
            Assert.Null(content.FullName);
            content.Name = "Test";
            Assert.Equal("Test", content.FullName);
            content.Group = "Group";
            Assert.Equal("Group.Test", content.FullName);
            content.Name = null;
            Assert.Equal("Group.", content.FullName);
        }

        [Fact]
        public void Value()
        {
            var content = new ContentLine();
            Assert.Null(content.Value);
            content.Value = "test";
            Assert.Equal("test", content.Value);
        }

        [Fact]
        public void Parameters()
        {
            var content = new ContentLine();
            Assert.Equal(0, content.ParamCount);

            Assert.False(content.HavingParam("p1"));
            Assert.False(content.HavingParam("p2"));
            Assert.False(content.HavingParam("p3"));

            content["p1"] = "v1";
            content["P2"] = "v2";
            content["P1"] = "v3";
            content[" "] = "v4";

            Assert.Equal(2, content.ParamCount);

            Assert.True(content.HavingParam("p1"));
            Assert.True(content.HavingParam("p2"));
            Assert.False(content.HavingParam("p3"));

            Assert.Equal("v3", content["p1"]);
            Assert.Equal("v2", content["P2"]);
            Assert.Equal("v3", content["P1"]);
            Assert.Null(content["P3"]);
            Assert.Null(content[" "]);

            Assert.Equal(new string[] { "p1", "P2" }, content.GetNames());
            Assert.Equal(new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("p1", "v3"),
                new KeyValuePair<string, string>("P2", "v2"),
            }, content.GetParams());

        }
    }
}
