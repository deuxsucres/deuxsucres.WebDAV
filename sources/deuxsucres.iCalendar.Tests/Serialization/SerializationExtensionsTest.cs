using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Serialization
{
    public class SerializationExtensionsTest
    {
        [Fact]
        public void CheckStrict()
        {
            bool strict = false;
            var mReader = new Mock<ICalReader>();
            mReader.SetupGet(r => r.StrictMode).Returns(() => strict);
            var reader = mReader.Object;

            bool invoked = false;
            Action action = () => invoked = true;

            Assert.True(reader.CheckStrict(() => true, action));
            Assert.False(invoked);

            invoked = false;
            Assert.False(reader.CheckStrict(() => false, action));
            Assert.False(invoked);

            invoked = false;
            Assert.True(reader.CheckStrict(null, action));
            Assert.False(invoked);

            strict = true;
            invoked = false;
            Assert.True(reader.CheckStrict(() => true, action));
            Assert.False(invoked);

            invoked = false;
            Assert.False(reader.CheckStrict(() => false, action));
            Assert.True(invoked);

            invoked = false;
            Assert.True(reader.CheckStrict(null, action));
            Assert.False(invoked);
        }

        [Fact]
        public void CheckSyntaxError()
        {
            bool strict = false;
            var mReader = new Mock<ICalReader>();
            mReader.SetupGet(r => r.StrictMode).Returns(() => strict);
            var reader = mReader.Object;

            reader.CheckSyntaxError(() => true, "error");
            reader.CheckSyntaxError(() => true, () => "error");

            reader.CheckSyntaxError(() => false, "error");
            reader.CheckSyntaxError(() => false, () => "error");

            reader.CheckSyntaxError(null, "error");
            reader.CheckSyntaxError(null, () => "error");

            strict = true;

            reader.CheckSyntaxError(() => true, "error");
            reader.CheckSyntaxError(() => true, () => "error");

            Assert.Throws<CalSyntaxError>(() => reader.CheckSyntaxError(() => false, "error"));
            Assert.Throws<CalSyntaxError>(() => reader.CheckSyntaxError(() => false, () => "error"));

            reader.CheckSyntaxError(null, "error");
            reader.CheckSyntaxError(null, () => "error");

        }

        [Fact]
        public void MakePropertyParameter_Default()
        {
            Mock<ICalPropertyParameter> mProp = null;
            ICalPropertyParameter prop = null;

            var mReader = new Mock<ICalReader>();
            mReader.Setup(r => r.CreateDefaultParameter(It.IsAny<string>()))
                .Returns<string>(n =>
                {
                    if (mProp != null)
                    {
                        mProp.SetupGet(p => p.Name).Returns(n);
                        prop = mProp.Object;
                    }
                    return prop;
                });
            var reader = mReader.Object;

            var result = reader.MakePropertyParameter("PropName", "PropValue");
            Assert.Null(result);

            mProp = new Mock<ICalPropertyParameter>();
            result = reader.MakePropertyParameter("PropName", "PropValue");
            Assert.Null(result);
            mProp.Verify(p => p.Deserialize(reader, "PropName", "PropValue"), Times.Once());

            mProp = new Mock<ICalPropertyParameter>();
            mProp.Setup(p => p.Deserialize(It.IsAny<ICalReader>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns<ICalReader, string, string>((r, n, v) =>
                {
                    return true;
                });
            result = reader.MakePropertyParameter("PropName", "PropValue");
            Assert.NotNull(result);
            Assert.Same(prop, result);
            mProp.Verify(p => p.Deserialize(reader, "PropName", "PropValue"), Times.Once());

            reader = null;
            result = reader.MakePropertyParameter("PropName", "PropValue");
            Assert.Null(result);
        }

        class TestPropertyParameter : ICalPropertyParameter
        {
            public bool Deserialize(ICalReader reader, string name, string value)
            {
                Name = name;
                Value = value;
                return !string.IsNullOrWhiteSpace(name);
            }
            public bool Serialize(ICalWriter writer, ContentLine line)
            {
                line.SetParam(Name, Value);
                return true;
            }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        [Fact]
        public void MakePropertyParameter_Generic()
        {
            var mReader = new Mock<ICalReader>();
            var reader = mReader.Object;

            var result = reader.MakePropertyParameter<TestPropertyParameter>("PropName", "PropValue");
            Assert.NotNull(result);
            Assert.Equal("PropName", result.Name);
            Assert.Equal("PropValue", result.Value);

            // If the property name is empty, the TestPropertyParameter.Deserialize() returns false
            result = reader.MakePropertyParameter<TestPropertyParameter>("", "PropValue");
            Assert.Null(result);
        }

    }
}
