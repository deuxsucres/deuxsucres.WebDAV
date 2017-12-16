using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests.ContentParameters
{
    public class ContentParameterTest
    {
        [Fact]
        public void Create()
        {
            var mParam = new Mock<ContentParameter>() { CallBase = true };
            var param = mParam.Object;
            Assert.Null(param.Name);
        }

        [Fact]
        public void Serialization()
        {
            var mParam = new Mock<ContentParameter>() { CallBase = true };
            var param = mParam.Object;

            var syntax = new ContentSyntax();
            var cparam = new ContentLineParameter("test");

            Assert.Throws<ArgumentNullException>(() => param.Serialize(null));
            Assert.Null(param.Serialize(syntax));
            param.Name = "MyTest";
            Assert.NotNull(param.Serialize(syntax));

            Assert.False(param.Deserialize(cparam, syntax));
            Assert.Equal("MyTest", param.Name);

            mParam.Protected().Setup<bool>("InternalDeserialize", ItExpr.IsAny<ContentLineParameter>(), ItExpr.IsAny<ContentSyntax>())
                .Returns(() => true);
            param = mParam.Object;
            Assert.True(param.Deserialize(cparam, syntax));
            Assert.Equal("test", param.Name);

            Assert.Throws<ArgumentNullException>(() => param.Deserialize(cparam, null));
            Assert.Throws<ArgumentNullException>(() => param.Deserialize(null, syntax));

            mParam.Protected().Verify("InternalSerialize", Times.Once(), ItExpr.IsAny<ContentLineParameter>(), ItExpr.IsAny<ContentSyntax>());
            mParam.Protected().Verify("InternalDeserialize", Times.Exactly(2), ItExpr.IsAny<ContentLineParameter>(), ItExpr.IsAny<ContentSyntax>());
        }

    }
}
