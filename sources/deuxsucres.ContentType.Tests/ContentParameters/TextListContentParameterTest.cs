using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests.ContentParameters
{
    public class TextListContentParameterTest
    {
        [Fact]
        public void Casts()
        {
            TextListContentParameter param = new TextListContentParameter();
            var values = new string[] { "Val1", "Val2" };

            param = values;

            string[] s = param;
            Assert.Equal(values, s);
            List<string> l = param;
            Assert.Equal(values, l);

            param = null;
            s = param;
            Assert.Null(s);
            l = param;
            Assert.Null(l);

            s = new string[] { "Val1", "Val2" };
            param = s;
            Assert.Null(param.Name);
            Assert.Equal(values, param.Value);
            l = new List<string> { "Val1", "Val2" };
            param = l;
            Assert.Null(param.Name);
            Assert.Equal(values, param.Value);

            s = null;
            param = s;
            Assert.Null(param);

            l = null;
            param = l;
            Assert.Null(param);
        }

        [Fact]
        public void Serialization()
        {
            var syntax = new ContentSyntax();
            TextListContentParameter param = new TextListContentParameter
            {
                Name = "Param",
                Value = new List<string> { "Val1", "Val2" }
            };
            var cparam = param.Serialize(syntax);
            Assert.NotNull(cparam);
            Assert.Equal("Param", cparam.Name);
            Assert.Equal(new string[] { "Val1", "Val2" }, cparam.Values);
            Assert.Equal("Val1, Val2", cparam.Value);

            cparam = new ContentLineParameter("Content");
            cparam.Values.Add("Value 1");

            param = new TextListContentParameter();
            param.Deserialize(cparam, syntax);
            Assert.Equal("Content", param.Name);
            Assert.Equal(new string[] { "Value 1"}, param.Value);

            cparam.Values.Add("Value 2");

            param = new TextListContentParameter();
            param.Deserialize(cparam, syntax);
            Assert.Equal("Content", param.Name);
            Assert.Equal(new string[] { "Value 1", "Value 2" }, param.Value);
        }
    }
}
