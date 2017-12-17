using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests.ContentProperties
{
    public class ContentPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new Mock<ContentProperty>() { CallBase = true }.Object;
            Assert.Null(prop.Name);
            Assert.Equal(0, prop.ParameterCount);
        }

        [Fact]
        public void AsComponent()
        {
            var prop = new Mock<ContentProperty>() { CallBase = true }.Object;
            IContentComponent comp = prop;
            Assert.Null(comp.Parent);

            var entity = new Mock<ContentEntity>() { CallBase = true }.Object;
            comp.Parent = entity;
            Assert.Same(entity, comp.Parent);
        }

        [Fact]
        public void Parameters()
        {
            var prop = new Mock<ContentProperty>() { CallBase = true }.Object;

            Assert.Equal(0, prop.ParameterCount);
            TextContentParameter p1 = new TextContentParameter { Name = "p1", Value = "v1" };
            prop.SetParameter(p1);
            Assert.Equal(1, prop.ParameterCount);
            Assert.NotNull(prop.FindParameter("P1"));

            prop.SetParameter(null);
            Assert.Equal(1, prop.ParameterCount);

            TextListContentParameter p2 = new TextListContentParameter { Value = new List<string> { "v2", "v3" } };
            prop.SetParameter(p2, "p2");
            Assert.Equal(2, prop.ParameterCount);
            Assert.NotNull(prop.FindParameter("P2"));

            TextContentParameter p3 = new TextContentParameter { Name = "p2", Value = "v2" };
            prop.SetParameter(p3, "p3");
            Assert.Equal(3, prop.ParameterCount);
            Assert.NotNull(prop.FindParameter("P3"));

            Assert.Same(p1, prop.FindParameter("P1"));
            Assert.Same(p2, prop.FindParameter("P2"));
            Assert.Same(p3, prop.FindParameter("P3"));
            Assert.Null(prop.FindParameter("P4"));

            Assert.NotNull(prop.FindParameter<TextContentParameter>("P1"));
            Assert.Null(prop.FindParameter<TextContentParameter>("P2"));
            Assert.NotNull(prop.FindParameter<TextContentParameter>("P3"));
            Assert.Null(prop.FindParameter<TextContentParameter>("P4"));

            Assert.Same(p3, prop.GetParameter<TextContentParameter>("p3"));
            var p4 = prop.GetParameter<TextContentParameter>("p4");
            Assert.NotNull(p4);
            Assert.Equal(4, prop.ParameterCount);

            var p5 = prop.GetParameter<TextContentParameter>("p2");
            Assert.Same(p5, prop.FindParameter("P2"));
            Assert.NotNull(prop.FindParameter<TextContentParameter>("P2"));
            Assert.Equal(4, prop.ParameterCount);

            prop.SetParameter(null, "p2");
            Assert.Equal(3, prop.ParameterCount);
            prop.SetParameter(null, "p3");
            Assert.Equal(2, prop.ParameterCount);

            Assert.Equal(new ContentParameter[] { p1, p4 }, prop.GetParameters());
            prop.RemoveParameter("P1");
            prop.RemoveParameter("P2");
            Assert.Equal(new ContentParameter[] { p4 }, prop.GetParameters());

            prop.RemoveParameter("p4");
            Assert.Equal(0, prop.ParameterCount);

            prop.SetParameter(p1);
            prop.SetParameter(p2);
            prop.SetParameter(p3);
            prop.SetParameter(p4);
            Assert.Equal(4, prop.ParameterCount);
            prop.ClearParameters();
            Assert.Equal(0, prop.ParameterCount);

        }
    }
}
