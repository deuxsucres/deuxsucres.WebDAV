using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentEntityTest
    {
        class TestEntity : ContentEntity
        {
            public override string Name => "Test";
        }
        class TestProperty : ContentProperty
        {
        }

        [Fact]
        public void Create()
        {
            var entity = new Mock<ContentEntity>() { CallBase = true }.Object;
            Assert.Equal(0, entity.ComponentCount);
        }

        [Fact]
        public void Components()
        {
            var entity = new Mock<ContentEntity>() { CallBase = true }.Object;
            Assert.Equal(0, entity.ComponentCount);

            var c1 = new TestEntity();
            var c2 = new TestProperty();
            var c3 = new TestEntity();
            var c4 = new TestProperty();

            entity.AddComponent(c1);
            entity.AddComponent(null);
            Assert.Equal(1, entity.ComponentCount);

            entity.AddComponent(c2);
            Assert.Equal(2, entity.ComponentCount);
            c2.Name = "Test";

            entity.AddComponent(c3);
            entity.AddComponent(c4);
            c4.Name = "Property";

            Assert.Equal(new IContentComponent[] { c1, c2, c3, c4 }, entity.GetComponents());
            Assert.Same(c1, entity[0]);
            Assert.Same(c4, entity[3]);
            Assert.Equal(new IContentComponent[] { c1, c2, c3 }, entity.FindComponents("test"));
            Assert.Equal(new IContentComponent[] { c1, c3 }, entity.FindComponents<ContentEntity>("test"));
            Assert.Equal(new IContentComponent[] { c2 }, entity.FindComponents<ContentProperty>("test"));
            Assert.Equal(new IContentComponent[] { c1, c3 }, entity.FindComponents<TestEntity>("TEST"));
            Assert.Equal(new IContentComponent[] { c2 }, entity.FindComponents<TestProperty>("test"));

            entity.RemoveComponents("TEST");
            Assert.Equal(new IContentComponent[] { c4 }, entity.GetComponents());

            entity.RemoveComponent(c1);
            Assert.Equal(new IContentComponent[] { c4 }, entity.GetComponents());
            entity.RemoveComponent(null);
            Assert.Equal(new IContentComponent[] { c4 }, entity.GetComponents());
            entity.RemoveComponent(c4);
            Assert.Equal(new IContentComponent[] { }, entity.GetComponents());

            entity.AddComponent(c1);
            entity.AddComponent(c2);
            entity.AddComponent(c3);
            entity.AddComponent(c4);

            entity.SetComponent(null);
            entity.SetComponent(c3);
            Assert.Equal(new IContentComponent[] { c4, c3 }, entity.GetComponents());

            entity.SetProperty(c4, "Test");
            Assert.Equal("Test", c4.Name);
            Assert.Equal(new IContentComponent[] { c4 }, entity.GetComponents());

            Assert.Same(c4, entity.GetProperty<TestProperty>("test"));
            var c5 = entity.GetProperty<TestProperty>("Prop");
            Assert.NotNull(c5);
            Assert.Equal("Prop", c5.Name);
            Assert.Equal(new IContentComponent[] { c4, c5 }, entity.GetComponents());

            entity.SetProperty(null, null);
            Assert.Equal(new IContentComponent[] { c4, c5 }, entity.GetComponents());
            entity.SetProperty(null, "prop");
            Assert.Equal(new IContentComponent[] { c4 }, entity.GetComponents());

            entity.ClearComponents();
            Assert.Equal(new IContentComponent[] { }, entity.GetComponents());

        }

    }
}
