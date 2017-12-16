using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests.ContentParameters
{
    public class TextContentParameterTest
    {
        [Fact]
        public void Casts()
        {
            TextContentParameter param = new TextContentParameter();
            param = "Value";

            string s = param;
            Assert.Equal("Value", s);

            param = null;
            s = param;
            Assert.Null(s);

            s = "Test";
            param = s;
            Assert.Null(param.Name);
            Assert.Equal("Test", param.Value);

            s = null;
            param = s;
            Assert.Null(param);
        }

        [Fact]
        public void Serialization()
        {
            var syntax = new ContentSyntax();
            TextContentParameter param = new TextContentParameter
            {
                Name = "Param",
                Value = "Value"
            };
            var cparam = param.Serialize(syntax);
            Assert.NotNull(cparam);
            Assert.Equal("Param", cparam.Name);
            Assert.Equal(new string[] { "Value" }, cparam.Values);
            Assert.Equal("Value", cparam.Value);

            cparam = new ContentLineParameter("Content");
            cparam.Values.Add("Value 1");

            param = new TextContentParameter();
            param.Deserialize(cparam, syntax);
            Assert.Equal("Content", param.Name);
            Assert.Equal("Value 1", param.Value);

            cparam.Values.Add("Value 2");

            param = new TextContentParameter();
            param.Deserialize(cparam, syntax);
            Assert.Equal("Content", param.Name);
            Assert.Equal("Value 1, Value 2", param.Value);
        }
    }
}
