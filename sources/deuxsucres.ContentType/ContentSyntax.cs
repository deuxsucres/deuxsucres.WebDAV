using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Syntax definition of the content
    /// </summary>
    /// <remarks>
    /// Provides all methods to encode/decode content
    /// </remarks>
    public class ContentSyntax
    {

        #region Chars utilities
        /// <summary>
        /// LF: line feed
        /// </summary>
        public const char LF = '\x0A';
        protected static readonly string sLF = LF.ToString();

        /// <summary>
        /// CR: carriage return
        /// </summary>
        public const char CR = '\x0D';
        protected static readonly string sCR = CR.ToString();

        /// <summary>
        /// CRLF: end of line
        /// </summary>
        public const string CRLF = "\x0D\x0A";

        /// <summary>
        /// SP: space char
        /// </summary>
        public const char SP = ' ';
        //protected static readonly string sSP = SP.ToString();

        /// <summary>
        /// HTAB: horizontal tab
        /// </summary>
        public const char HTAB = '\x09';
        //protected static readonly string sHTAB = HTAB.ToString();

        /// <summary>
        /// DQUOTE: double quote
        /// </summary>
        public const char DQUOTE = '\x22';
        //protected static readonly string sDQUOTE = DQUOTE.ToString();

        /// <summary>
        /// Check if <paramref name="c"/> is a whitspace
        /// </summary>
        public static bool IsWSP(char c) => c == SP || c == HTAB;

        /// <summary>
        /// Check if <paramref name="c"/> is a visible char
        /// </summary>
        public static bool IsVCHAR(char c) => !char.IsControl(c);
        //public static bool IsVCHAR(char c) => c >= '\x21' && c <= '\x7E';

        /// <summary>
        /// Check if <paramref name="c"/> is a control char
        /// </summary>
        public static bool IsCTL(char c) => char.IsControl(c);
        //public static bool IsCTL(char c) => (c >= '\x00' && c <= '\x1F') || c=='\x7F';

        /// <summary>
        /// Check if <paramref name="c"/> is an alpha char
        /// </summary>
        public static bool IsALPHA(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

        /// <summary>
        /// Check if <paramref name="c"/> is a digit
        /// </summary>
        public static bool IsDIGIT(char c) => c >= '0' && c <= '9';

        /// <summary>
        /// Check if <paramref name="c"/> is a hexadecimal digit
        /// </summary>
        public static bool IsHEXDIGIT(char c) => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');

        /// <summary>
        /// Check if <paramref name="c"/> is a bit reference
        /// </summary>
        public static bool IsBIT(char c) => c == '0' || c == '1';

        /// <summary>
        /// Check if <paramref name="c"/> is any char except NULL
        /// </summary>
        public static bool IsCHAR(char c) => c != '\x00';
        #endregion

        #region Decode

        ///// <summary>
        ///// Read a IANA token
        ///// </summary>
        ///// <remarks>
        ///// 1*(ALPHA / DIGIT / "-")
        ///// </remarks>
        //protected virtual string ReadIanaToken(string line, ref int pos)
        //{
        //    int s = pos, l=line.Length;
        //    char c;
        //    while (pos < l && ((c = line[pos]) == '-' || IsALPHA(c) || IsDIGIT(c)))
        //        pos++;
        //    return s == pos ? null : line.Substring(s, pos - s);
        //}

        ///// <summary>
        ///// Decode a content line
        ///// </summary>
        //public virtual ContentLine DecodeContentLine(string line, bool throwErrors = false)
        //{
        //    ContentLine CheckError(string message)
        //    {
        //        if (throwErrors) throw new ContentSyntaxError();
        //        return null;
        //    };

        //    // An empty line is not error
        //    if (string.IsNullOrWhiteSpace(line)) return null;

        //    var content = new ContentLine();
        //    int pos = 0;

        //    // Read the name
        //    string name = ReadIanaToken(line, ref pos);
        //    //if(name==null) 

        //    char[] partSeparators = new char[] { ':', ';' };
        //    int idx = line.IndexOfAny(partSeparators);
        //    if (idx < 0)
        //    {
        //        content.Name = line;
        //        return content;
        //    }
        //    else
        //    {
        //        content.Name = line.Substring(0, idx);
        //    }

        //    // Read the parameters
        //    char[] paramSeparators = new char[] { ':', '=', ';' };
        //    char[] paramListSeparators = new char[] { ':', ',', ';' };
        //    int pos = idx;
        //    while (pos < line.Length && line[pos] == ';')
        //    {
        //        pos++;  // Skip the separator
        //        idx = line.IndexOfAny(paramSeparators, pos);
        //        // End of line ?
        //        if (idx < 0)
        //        {
        //            // Create an empty parameter
        //            if (pos < line.Length)
        //                content[line.Substring(pos)] = string.Empty;
        //            return content;
        //        }
        //        // get the name
        //        string paramName = line.Substring(pos, idx - pos);
        //        string paramValue = string.Empty;
        //        // Whithout value ?
        //        if (line[idx] != '=')
        //        {
        //            // Create an empty parameter
        //            content[paramName] = paramValue;
        //            // And continue
        //            pos = idx;
        //            continue;
        //        }
        //        // Check the value
        //        int sParam = idx + 1;
        //        pos = sParam - 1;
        //        do
        //        {
        //            pos++;
        //            // Encoded ?
        //            if (pos < line.Length && line[pos] == '"')
        //            {
        //                idx = line.IndexOf('"', pos + 1);
        //                if (idx < 0)
        //                {
        //                    // Get the rest of the line as a parameter value
        //                    pos = line.Length;
        //                    break;
        //                }
        //                else
        //                {
        //                    // Extract the parameter value
        //                    pos = idx + 1;
        //                }
        //            }
        //            else
        //            {
        //                // Search a separator
        //                idx = line.IndexOfAny(paramListSeparators, pos);
        //                if (idx < 0)
        //                {
        //                    // Get the rest of the line as a parameter value
        //                    pos = line.Length;
        //                    break;
        //                }
        //                else
        //                {
        //                    // Extract the parameter value
        //                    paramValue = line.Substring(sParam, idx - pos);
        //                    pos = idx;
        //                }
        //            }
        //        } while (pos >= 0 && pos < line.Length && line[pos] == ',');
        //        if (pos >= 0)
        //            paramValue = line.Substring(sParam, pos - sParam);
        //        // Create the parameter
        //        if (!string.IsNullOrEmpty(paramName) || !string.IsNullOrEmpty(paramValue))
        //        {
        //            //paramValue = paramValue.Replace("=3D", "=").Replace("\\;", ";");
        //            content[paramName] = paramValue;
        //        }
        //    }

        //    // Read the value
        //    content.Value = pos < line.Length ? line.Substring(pos + 1) : null;
        //    return content;
        //}

        #endregion

        #region Encode

        ///// <summary>
        ///// Encode a content line
        ///// </summary>
        //public virtual string EncodeContentLine(ContentLine line)
        //{
        //    if (line == null) return null;
        //    StringBuilder builder = new StringBuilder();
        //    if (!string.IsNullOrEmpty(line.Group))
        //    {
        //        builder.Append(line.Group);
        //        if (!string.IsNullOrEmpty(line.Name))
        //            builder.Append('.');
        //    }
        //    builder.Append(line.Name);
        //    foreach (var prm in line.GetParams())
        //    {
        //        builder
        //            .Append(";")
        //            .Append(prm.Name)
        //            .Append("=")
        //            .Append(EncodeTextParameter(prm.Value))
        //            ;
        //    }
        //    builder.Append(":").Append(line.Value);
        //    return builder.ToString();
        //}

        ///// <summary>
        ///// Encode a text value
        ///// </summary>
        //public virtual string EncodeText(string value)
        //{
        //    if (value == null) return null;
        //    return value
        //        .Replace(@"\", @"\\")
        //        .Replace(";", @"\;")
        //        .Replace(",", @"\,")
        //        .Replace(CRLF, @"\n")
        //        .Replace(sCR, @"\n")
        //        .Replace(sLF, @"\n")
        //        ;
        //}

        ///// <summary>
        ///// Encode a text parameter value
        ///// </summary>
        //public virtual string EncodeTextParameter(string value)
        //{
        //    if (value == null) return null;
        //    if (value.IndexOfAny(new char[] { ',', ';', '=' }) >= 0 && !(value.StartsWith("\"") && value.EndsWith("\"")))
        //        return $"\"{EncodeText(value)}\"";
        //    return EncodeText(value);
        //}

        #endregion

    }

}
