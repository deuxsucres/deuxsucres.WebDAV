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
        /// <summary>
        /// LF: line feed
        /// </summary>
        public const char LF = '\x0A';

        /// <summary>
        /// CR: carriage return
        /// </summary>
        public const char CR = '\x0D';

        /// <summary>
        /// CRLF: end of line
        /// </summary>
        public const string CRLF = "\x0D\x0A";

        /// <summary>
        /// SP: space char
        /// </summary>
        public const char SP = ' ';

        /// <summary>
        /// HTAB: horizontal tab
        /// </summary>
        public const char HTAB = '\x09';

        /// <summary>
        /// DQUOTE: double quote
        /// </summary>
        public const char DQUOTE = '\x22';

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

    }

}
