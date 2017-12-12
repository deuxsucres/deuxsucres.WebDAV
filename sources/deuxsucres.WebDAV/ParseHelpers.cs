using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Parsing helpers
    /// </summary>
    /// <remarks>
    /// HTTP Basic rule: https://tools.ietf.org/html/rfc2616#section-2.2
    /// </remarks>
    public static class ParseHelpers
    {

        /// <summary>
        /// CR char
        /// </summary>
        public const char CR = (char)13;

        /// <summary>
        /// LF char
        /// </summary>
        public const char LF = (char)10;

        /// <summary>
        /// DEL char
        /// </summary>
        public const char DEL = (char)127;

        /// <summary>
        /// Space char
        /// </summary>
        public const char SP = (char)32;

        /// <summary>
        /// Horizontal tab
        /// </summary>
        public const char HT = (char)9;

        /// <summary>
        /// Double quote
        /// </summary>
        public const char DQUOTE = '"';

        /// <summary>
        /// Determines if <paramref name="c"/> is a char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsCHAR(char c) => c >= 0 && c <= 127;

        /// <summary>
        /// Determines if <paramref name="c"/> is an upper alpha char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsUPALPHA(char c) => c >= 'A' && c <= 'Z';

        /// <summary>
        /// Determines if <paramref name="c"/> is a lower alpha char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsLOALPHA(char c) => c >= 'a' && c <= 'z';

        /// <summary>
        /// Determines if <paramref name="c"/> is an alpha char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsALPHA(char c) => IsLOALPHA(c) || IsUPALPHA(c);

        /// <summary>
        /// Determines if <paramref name="c"/> is a digit char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsDIGIT(char c) => c >= '0' && c <= '9';

        /// <summary>
        /// Determines if <paramref name="c"/> is an hex digit char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsHEX(char c) => (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || (c >= '0' && c <= '9');

        /// <summary>
        /// Determines if <paramref name="c"/> is a control char like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsCTL(char c) => (c >= 0 && c <= 31) || c == DEL;

        /// <summary>
        /// Determines if <paramref name="c"/> is a separator like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsSeparator(char c)
        {
            return c == '(' || c == ')' || c == '<' || c == '>' || c == '@'
                      || c == ',' || c == ';' || c == ':' || c == '\\' || c == DQUOTE
                      || c == '/' || c == '[' || c == ']' || c == '?' || c == '='
                      || c == '{' || c == '}' || c == SP || c == HT;
        }
        /// <summary>
        /// Determines if <paramref name="value"/> is a token like defined in the RFC 2616 section 2.2
        /// </summary>
        public static bool IsToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return value.Cast<char>().All(c => IsCHAR(c) && !IsSeparator(c) && !IsCTL(c));
        }

        /// <summary>
        /// Parse a depth value
        /// </summary>
        public static DepthValue? ParseDepth(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            foreach (DepthValue depth in Enum.GetValues(typeof(DepthValue)))
            {
                if (string.Equals(value, depth.ToHeaderValue(), StringComparison.OrdinalIgnoreCase))
                    return depth;
            }
            return null;
        }

        /// <summary>
        /// Parse a timeout value
        /// </summary>
        public static uint? ParseTimeout(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (string.Equals("Infinite", value, StringComparison.OrdinalIgnoreCase))
                return uint.MaxValue;
            if (!value.StartsWith("Second-", StringComparison.OrdinalIgnoreCase))
                return null;
            if (uint.TryParse(value.Substring(7), out uint r))
                return r;
            return null;
        }

    }
}
