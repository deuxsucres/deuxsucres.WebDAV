using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Objects.Properties
{
    public class AttachPropertyTest
    {
        [Fact]
        public void Create()
        {
            var prop = new AttachProperty();
            Assert.Equal(Constants.ATTACH, prop.Name);
            Assert.False(prop.IsBinary);
            Assert.Null(prop.BinaryValue);
            Assert.Null(prop.UriValue);
            Assert.Null(prop.FormatType);
        }

        [Fact]
        public void Properties()
        {
            var prop = new AttachProperty();
            Assert.False(prop.IsBinary);
            Assert.Null(prop.BinaryValue);
            Assert.Null(prop.UriValue);
            Assert.Null(prop.FormatType);

            prop.FormatType = "Test";
            Assert.Equal("Test", prop.FormatType);
            prop.FormatType = null;
            Assert.Null(prop.FormatType);

            var data = new byte[] { 1, 2, 3, 4, 5 };
            prop.BinaryValue = data;
            Assert.True(prop.IsBinary);
            Assert.NotNull(prop.BinaryValue);
            Assert.Equal(data, prop.BinaryValue);
            Assert.Null(prop.UriValue);
            Assert.Equal(2, prop.ParameterCount);

            var uri = new Uri("http://www.deuxsucres.com");
            prop.UriValue = uri;
            Assert.False(prop.IsBinary);
            Assert.Null(prop.BinaryValue);
            Assert.NotNull(prop.UriValue);
            Assert.Equal("http://www.deuxsucres.com/", prop.UriValue.ToString());
            Assert.Equal(0, prop.ParameterCount);
        }

        [Fact]
        public void Cast()
        {
            var prop = new AttachProperty();

            var data = new byte[] { 1, 2, 3, 4, 5 };
            prop = data;
            Assert.True(prop.IsBinary);
            Assert.NotNull(prop.BinaryValue);
            Assert.Equal(data, prop.BinaryValue);
            Assert.Null(prop.UriValue);
            Assert.Equal(2, prop.ParameterCount);

            var uri = new Uri("http://www.deuxsucres.com");
            prop = uri;
            Assert.False(prop.IsBinary);
            Assert.Null(prop.BinaryValue);
            Assert.NotNull(prop.UriValue);
            Assert.Equal("http://www.deuxsucres.com/", prop.UriValue.ToString());
            Assert.Equal(0, prop.ParameterCount);
        }

        [Fact]
        public void Serialization()
        {
            var data = new byte[] { 1, 2, 3, 4, 5 };
            var uri = new Uri("http://www.deuxsucres.com");

            var parser = new CalendarParser();
            var output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new AttachProperty
                {
                    BinaryValue = data,
                    FormatType = "application/binary"
                };
                prop.Serialize(writer);

                prop = new AttachProperty
                {
                    UriValue = uri
                };
                prop.Serialize(writer);

                prop = new AttachProperty
                {
                    BinaryValue = data
                };
                prop.RemoveParameter(Constants.VALUE);
                prop.RemoveParameter(Constants.ENCODING);
                prop.Serialize(writer);

            }

            Assert.Equal(new StringBuilder()
                .AppendLine("ATTACH;ENCODING=Base64;VALUE=BINARY;FMTTYPE=application/binary:AQIDBAU=")
                .AppendLine("ATTACH:http://www.deuxsucres.com/")
                .AppendLine("ATTACH;ENCODING=Base64;VALUE=BINARY:AQIDBAU=")
                .ToString(), output.ToString());

            string input = output.ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var prop = reader.MakeProperty<AttachProperty>(reader.ReadNextLine());
                Assert.NotNull(prop);
                Assert.Equal(Constants.ATTACH, prop.Name);
                Assert.Equal("application/binary", prop.FormatType);
                Assert.True(prop.IsBinary);
                Assert.Null(prop.UriValue);
                Assert.Equal(data, prop.BinaryValue);
                Assert.Equal(3, prop.ParameterCount);
                Assert.Equal("application/binary", prop.FindParameter<TextParameter>(Constants.FMTTYPE).Value);
                Assert.Equal("Base64", prop.FindParameter<TextParameter>(Constants.ENCODING).Value);
                Assert.Equal(ValueTypes.Binary, prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE).Value);

                prop = reader.MakeProperty<AttachProperty>(reader.ReadNextLine());
                Assert.NotNull(prop);
                Assert.Equal(Constants.ATTACH, prop.Name);
                Assert.Null(prop.FormatType);
                Assert.False(prop.IsBinary);
                Assert.Equal("http://www.deuxsucres.com/", prop.UriValue.ToString());
                Assert.Null(prop.BinaryValue);
                Assert.Equal(0, prop.ParameterCount);

                prop = reader.MakeProperty<AttachProperty>(reader.ReadNextLine());
                Assert.NotNull(prop);
                Assert.Equal(Constants.ATTACH, prop.Name);
                Assert.Null(prop.FormatType);
                Assert.True(prop.IsBinary);
                Assert.Null(prop.UriValue);
                Assert.Equal(data, prop.BinaryValue);
                Assert.Equal(2, prop.ParameterCount);
                Assert.Equal("Base64", prop.FindParameter<TextParameter>(Constants.ENCODING).Value);
                Assert.Equal(ValueTypes.Binary, prop.FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE).Value);

                prop = reader.MakeProperty<AttachProperty>(reader.ReadNextLine());
                Assert.Null(prop);
            }

        }

    }
}
