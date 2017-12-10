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
            }, content.GetParams().ToDictionary(p => p.Name, p => p.Value));

            content["P2"] = null;
            Assert.Equal(1, content.ParamCount);

            Assert.True(content.HavingParam("p1"));
            Assert.False(content.HavingParam("p2"));
            Assert.False(content.HavingParam("p3"));

            content.AddParam("p2", "a");
            Assert.Equal("a", content["P2"]);
            content.AddParam("p2", "b");
            Assert.Equal("a, b", content["P2"]);

            Assert.Equal(2, content.ParamCount);
            Assert.True(content.HavingParam("p1"));
            Assert.True(content.HavingParam("p2"));
            Assert.False(content.HavingParam("p3"));

            content.AddParam(" ", "a");
            Assert.Equal(2, content.ParamCount);
        }

        [Fact]
        public void ContentLineParameter()
        {
            var param = new ContentLineParameter("name");
            Assert.Equal("name", param.Name);
            Assert.Empty(param.Values);
            Assert.Equal(string.Empty, param.Value);

            param.Values.Add("v1");
            Assert.Equal(new string[] { "v1" }, param.Values);
            Assert.Equal(new string[] { "v1" }, param);
            Assert.Equal("v1", param.Value);

            param.Values.Add("v2");
            Assert.Equal(new string[] { "v1", "v2" }, param.Values);
            Assert.Equal(new string[] { "v1", "v2" }, param);
            Assert.Equal(new string[] { "v1", "v2" }, (IEnumerable<string>)param);
            Assert.Equal(new string[] { "v1", "v2" }, param.ToList());
            Assert.Equal(new string[] { "v1", "v2" }, (System.Collections.IEnumerable)param);
            Assert.Equal(new string[] { "v1", "v2" }, (string[])param);
            Assert.Equal(new string[] { "v1", "v2" }, (List<string>)param);
            Assert.Equal("v1, v2", param.Value);
            Assert.Equal("v1, v2", param);

            param.Value = "v3";
            Assert.Equal(new string[] { "v3" }, param.Values);
            Assert.Equal(new string[] { "v3" }, param);
            Assert.Equal("v3", param.Value);
        }
    }
}
