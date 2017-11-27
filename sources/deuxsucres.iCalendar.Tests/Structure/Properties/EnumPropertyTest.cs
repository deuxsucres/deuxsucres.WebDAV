using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.iCalendar.Tests.Structure.Properties
{
    public class EnumPropertyTest
    {
        public enum EnumTest
        {
            Value,
            SecondValue,
            Third_Value
        }

        [Fact]
        public void Create()
        {
            var param = new EnumProperty<EnumTest>();
            Assert.Null(param.Value);
            Assert.Null(param.StringValue);
        }

        [Fact]
        public void Values()
        {
            var param = new EnumProperty<EnumTest>();
            Assert.Null(param.Value);
            Assert.Null(param.StringValue);

            param.Value = EnumTest.Third_Value;
            Assert.Equal(EnumTest.Third_Value, param.Value);
            Assert.Equal("THIRD-VALUE", param.StringValue);

            param.Value = null;
            Assert.Null(param.Value);
            Assert.Equal("THIRD-VALUE", param.StringValue);

            param.StringValue = "SecondValue";
            Assert.Equal(EnumTest.SecondValue, param.Value);
            Assert.Equal("SecondValue", param.StringValue);

            param.StringValue = "Test";
            Assert.Null(param.Value);
            Assert.Equal("Test", param.StringValue);

            param.StringValue = null;
            Assert.Null(param.Value);
            Assert.Null(param.StringValue);
        }

        [Fact]
        public void Cast()
        {
            var param = new EnumProperty<EnumTest>();
            Assert.Null(param.Value);
            Assert.Null((EnumTest?)param);
            Assert.Null((string)param);

            param = EnumTest.Third_Value;
            Assert.Equal(EnumTest.Third_Value, param.Value);
            Assert.Equal(EnumTest.Third_Value, (EnumTest?)param);
            Assert.Equal("THIRD-VALUE", param.StringValue);
            Assert.Equal("THIRD-VALUE", param);

            param = "SecondValue";
            Assert.Equal(EnumTest.SecondValue, param.Value);
            Assert.Equal(EnumTest.SecondValue, (EnumTest?)param);
            Assert.Equal("SecondValue", param.StringValue);
            Assert.Equal("SecondValue", param);

            param = "Test";
            Assert.Null(param.Value);
            Assert.Null((EnumTest?)param);
            Assert.Equal("Test", param.StringValue);
            Assert.Equal("Test", param);
        }

        [Fact]
        public void TestToString()
        {
            var param = new EnumProperty<EnumTest>();
            Assert.Null(param.ToString());
            param = EnumTest.Third_Value;
            Assert.Equal("THIRD-VALUE", param.ToString());
            param = "Test";
            Assert.Equal("Test", param.ToString());
        }

        [Fact]
        public void Serialization()
        {
            var parser = new CalendarParser();

            StringBuilder output = new StringBuilder();
            using (var source = new StringWriter(output))
            {
                var writer = new CalTextWriter(parser, source);

                var prop = new EnumProperty<EnumTest> { Name = "Test", Value = EnumTest.Third_Value };
                prop.Serialize(writer);
                prop = new EnumProperty<EnumTest> { Name = "Test2", StringValue = "Test" };
                prop.Serialize(writer);
            }

            Assert.Equal(new StringBuilder()
                .AppendLine("TEST:THIRD-VALUE")
                .AppendLine("TEST2:Test")
                .ToString(), output.ToString());

            string input = new StringBuilder()
                .AppendLine("TEST:THIRD-VALUE")
                .AppendLine("TEST2:Test")
                .AppendLine("TEST3:")
                .ToString();
            using (var source = new StringReader(input))
            {
                var reader = new CalTextReader(parser, source, false);

                var line = reader.ReadNextLine();
                var prop = reader.MakeProperty<EnumProperty<EnumTest>>(line);
                Assert.Equal("TEST", prop.Name);
                Assert.Equal(EnumTest.Third_Value, prop.Value);
                Assert.Equal("THIRD-VALUE", prop.StringValue);

                line = reader.ReadNextLine();
                prop = reader.MakeProperty<EnumProperty<EnumTest>>(line);
                Assert.Equal("TEST2", prop.Name);
                Assert.Null(prop.Value);
                Assert.Equal("Test", prop.StringValue);

                line = reader.ReadNextLine();
                prop = reader.MakeProperty<EnumProperty<EnumTest>>(line);
                Assert.Equal("TEST3", prop.Name);
                Assert.Null(prop.Value);
                Assert.Null(prop.StringValue);

                line = reader.ReadNextLine();
                Assert.Null(line);
            }
        }
    }
}
