using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace deuxsucres.ContentType.Tests
{
    public class ContentSyntaxTest
    {
        [Fact]
        public void TestChars()
        {
            Assert.True(ContentSyntax.IsALPHA('f'));
            Assert.False(ContentSyntax.IsALPHA('6'));

            Assert.True(ContentSyntax.IsWSP(' '));
            Assert.False(ContentSyntax.IsWSP('r'));

            Assert.True(ContentSyntax.IsVCHAR(' '));
            Assert.False(ContentSyntax.IsVCHAR('\x09'));

            Assert.False(ContentSyntax.IsCTL(' '));
            Assert.True(ContentSyntax.IsCTL('\x09'));

            Assert.False(ContentSyntax.IsDIGIT('f'));
            Assert.True(ContentSyntax.IsDIGIT('6'));

            Assert.False(ContentSyntax.IsHEXDIGIT('Z'));
            Assert.True(ContentSyntax.IsHEXDIGIT('f'));
            Assert.True(ContentSyntax.IsHEXDIGIT('6'));

            Assert.False(ContentSyntax.IsBIT('f'));
            Assert.True(ContentSyntax.IsBIT('0'));

            Assert.False(ContentSyntax.IsCHAR('\x00'));
            Assert.True(ContentSyntax.IsCHAR('0'));
        }

        public static IEnumerable<object[]> DecodeContentLineData()
        {
            // Decode empty line
            yield return new object[] { null, 12, false, false, null, null };
            yield return new object[] { null, 12, true, false, null, null };
            yield return new object[] { null, 12, false, true, null, null };
            yield return new object[] { null, 12, true, true, null, null };

            yield return new object[] { "", 12, false, false, null, null };
            yield return new object[] { "", 12, true, false, null, null };
            yield return new object[] { "", 12, false, true, null, null };
            yield return new object[] { "", 12, true, true, null, null };

            yield return new object[] { " ", 12, false, false, null, null };
            yield return new object[] { " ", 12, true, false, null, null };
            yield return new object[] { " ", 12, false, true, null, null };
            yield return new object[] { " ", 12, true, true, null, null };

            // Name only
            var content = new ContentLine {
                Name = "Test"
            };
            yield return new object[] { "Test", 12, false, false, content, null };
            yield return new object[] { "Test", 12, true, false, content, null };
            yield return new object[] { "Test", 12, false, true, content, null };
            yield return new object[] { "Test", 12, true, true, content, null };

            // Invalid name
            Exception ex = new ContentSyntaxError("Invalid character ',' for the name.", 12, 0);
            yield return new object[] { ",", 12, false, false, null, null };
            yield return new object[] { ",", 12, true, false, null, null };
            yield return new object[] { ",", 12, false, true, null, ex };
            yield return new object[] { ",", 12, true, true, null, ex };

            // Group only
            content = new ContentLine
            {
                Group = "Group",
                Name = "Test"
            };
            yield return new object[] { "Group.Test", 12, false, false, content, null };
            yield return new object[] { "Group.Test", 12, true, false, content, null };
            yield return new object[] { "Group.Test", 12, false, true, content, null };
            yield return new object[] { "Group.Test", 12, true, true, content, null };

            // Invalid group / name
            content = new ContentLine
            {
                Group = "Group",
                Name = "Test"
            };
            ex = new ContentSyntaxError("Invalid character '.' for the name.", 12, 0);
            yield return new object[] { ".Test", 12, false, false, null, null };
            yield return new object[] { ".Test", 12, true, false, null, null };
            yield return new object[] { ".Test", 12, false, true, null, ex };
            yield return new object[] { ".Test", 12, true, true, null, ex };

            content = new ContentLine
            {
                Group = "Group",
                Name = null
            };
            ex = new ContentSyntaxError("Invalid character '\0' for the name.", 12, 6);
            yield return new object[] { "Group.", 12, false, false, content, null };
            yield return new object[] { "Group.", 12, true, false, null, null };
            yield return new object[] { "Group.", 12, false, true, null, ex };
            yield return new object[] { "Group.", 12, true, true, null, ex };

            // Value
            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            };
            yield return new object[] { "Test:Value", 12, false, false, content, null };
            yield return new object[] { "Test:Value", 12, true, false, content, null };
            yield return new object[] { "Test:Value", 12, false, true, content, null };
            yield return new object[] { "Test:Value", 12, true, true, content, null };

            // Empty value
            content = new ContentLine
            {
                Name = "Test",
                Value = string.Empty
            };
            yield return new object[] { "Test:", 12, false, false, content, null };
            yield return new object[] { "Test:", 12, true, false, content, null };
            yield return new object[] { "Test:", 12, false, true, content, null };
            yield return new object[] { "Test:", 12, true, true, content, null };

            // Invalid value
            content = new ContentLine
            {
                Name = "Test"
            };
            ex = new ContentSyntaxError("Expected ':' instead of ','.", 12, 4);
            yield return new object[] { "Test,", 12, false, false, content, null };
            yield return new object[] { "Test,", 12, true, false, null, null };
            yield return new object[] { "Test,", 12, false, true, null, ex };
            yield return new object[] { "Test,", 12, true, true, null, ex };

            // Parameters
            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            }.AddParam("p1", "v1");
            yield return new object[] { "Test;p1=v1:Value", 12, false, false, content, null };
            yield return new object[] { "Test;p1=v1:Value", 12, true, false, content, null };
            yield return new object[] { "Test;p1=v1:Value", 12, false, true, content, null };
            yield return new object[] { "Test;p1=v1:Value", 12, true, true, content, null };

            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            }.AddParam("p1", "");
            yield return new object[] { "Test;p1=:Value", 12, false, false, content, null };
            yield return new object[] { "Test;p1=:Value", 12, true, false, content, null };
            yield return new object[] { "Test;p1=:Value", 12, false, true, content, null };
            yield return new object[] { "Test;p1=:Value", 12, true, true, content, null };

            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            }
            .AddParam("p1", "v1")
            .AddParam("p2", "^v\"^2");
            yield return new object[] { "Test;p1=v1;p2=^^v^'^2:Value", 12, false, false, content, null };
            yield return new object[] { "Test;p1=v1;p2=^^v^'^2:Value", 12, true, false, content, null };
            yield return new object[] { "Test;p1=v1;p2=^^v^'^2:Value", 12, false, true, content, null };
            yield return new object[] { "Test;p1=v1;p2=^^v^'^2:Value", 12, true, true, content, null };


            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            }
            .AddParam("p1", "v1;p2=v2");
            yield return new object[] { "Test;p1=\"v1;p2=v2\":Value", 12, false, false, content, null };
            yield return new object[] { "Test;p1=\"v1;p2=v2\":Value", 12, true, false, content, null };
            yield return new object[] { "Test;p1=\"v1;p2=v2\":Value", 12, false, true, content, null };
            yield return new object[] { "Test;p1=\"v1;p2=v2\":Value", 12, true, true, content, null };

            // Invalid parameter name
            content = new ContentLine
            {
                Name = "Test",
                Value = null
            };
            ex = new ContentSyntaxError("Invalid character '=' for the name.", 12, 5);
            yield return new object[] { "Test;=v1:Value", 12, false, false, content, null };
            yield return new object[] { "Test;=v1:Value", 12, true, false, null, null };
            yield return new object[] { "Test;=v1:Value", 12, false, true, null, ex };
            yield return new object[] { "Test;=v1:Value", 12, true, true, null, ex };

            // Invalid parameter value
            content = new ContentLine
            {
                Name = "Test",
                Value = null
            };
            ex = new ContentSyntaxError("Expected '\"' instead of '\0'.", 12, 23);
            yield return new object[] { "Test;p1=\"v1;p2=v2:Value", 12, false, false, content, null };
            yield return new object[] { "Test;p1=\"v1;p2=v2:Value", 12, true, false, null, null };
            yield return new object[] { "Test;p1=\"v1;p2=v2:Value", 12, false, true, null, ex };
            yield return new object[] { "Test;p1=\"v1;p2=v2:Value", 12, true, true, null, ex };

            content = new ContentLine
            {
                Name = "Test",
                Value = null
            };
            ex = new ContentSyntaxError("Expected '=' instead of ':'.", 12, 7);
            yield return new object[] { "Test;p1:Value", 12, false, false, content, null };
            yield return new object[] { "Test;p1:Value", 12, true, false, null, null };
            yield return new object[] { "Test;p1:Value", 12, false, true, null, ex };
            yield return new object[] { "Test;p1:Value", 12, true, true, null, ex };
        }

        [Theory]
        [MemberData(nameof(DecodeContentLineData))]
        public void DecodeContentLine(string line, int lineNumber, bool strict, bool throwErrors, ContentLine result, Exception error)
        {
            var syntax = new ContentSyntax();

            if (error != null)
            {
                var ex = Assert.Throws(error.GetType(), () => syntax.DecodeContentLine(line, lineNumber, strict, throwErrors));
                Assert.Equal(error.Message, ex.Message);
            }
            else
            {
                var content = syntax.DecodeContentLine(line, lineNumber, strict, throwErrors);
                if (result == null)
                    Assert.Null(content);
                else
                {
                    Assert.NotNull(content);
                    Assert.Equal(result.Group, content.Group);
                    Assert.Equal(result.Name, content.Name);
                    Assert.Equal(result.ParamCount, content.ParamCount);
                    Assert.Equal(
                        result.GetParams().ToDictionary(p => p.Name, p => p.Value),
                        content.GetParams().ToDictionary(p => p.Name, p => p.Value)
                        );
                    Assert.Equal(result.Value, content.Value);
                }
            }
        }

        public static IEnumerable<object[]> EncodeContentLineData()
        {
            // Decode empty line
            yield return new object[] { null, null };

            // Name
            var content = new ContentLine
            {
                Name = "Test"
            };
            yield return new object[] { content, "Test:" };
            content = new ContentLine
            {
                Group = "Group",
                Name = "Test"
            };
            yield return new object[] { content, "Group.Test:" };
            content = new ContentLine
            {
                Group = "Group",
                Name = null
            };
            yield return new object[] { content, "Group:" };

            // Value
            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            };
            yield return new object[] { content, "Test:Value" };

            // Parameters
            content = new ContentLine
            {
                Name = "Test",
                Value = "Value"
            }
            .AddParam("p1", "v1")
            .AddParam("p2", "")
            .AddParam("p3", "v3").AddParam("p3", "v4")
            .AddParam("p4", "v,5").AddParam("p4", "v\"6").AddParam("p4", "v7")
            ;
            yield return new object[] { content, "Test;p1=v1;p2=;p3=v3,v4;p4=\"v,5\",v^'6,v7:Value" };
        }

        [Theory]
        [MemberData(nameof(EncodeContentLineData))]
        public void EncodeContentLine(ContentLine line, string result)
        {
            var syntax = new ContentSyntax();

            Assert.Equal(result, syntax.EncodeContentLine(line));
        }

    }
}
