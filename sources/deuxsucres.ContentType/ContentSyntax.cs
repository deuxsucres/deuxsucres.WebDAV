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
