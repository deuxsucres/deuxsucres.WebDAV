using deuxsucres.ContentType.Locales;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Unescape a parameter value
        /// </summary>
        protected virtual string UnescapeParamValue(string value)
        {
            return !CircumflexEscape ? value : value
                    .Replace("^n", Environment.NewLine)
                    .Replace("^^", "^")
                    .Replace("^'", "\"")
                    ;
        }

        /// <summary>
        /// Read a IANA token
        /// </summary>
        /// <remarks>
        /// 1*(ALPHA / DIGIT / "-")
        /// </remarks>
        protected virtual string ReadIanaToken(string line, ref int pos)
        {
            int s = pos, l = line.Length;
            char c;
            while (pos < l && ((c = line[pos]) == '-' || IsALPHA(c) || IsDIGIT(c)))
                pos++;
            return s == pos ? null : line.Substring(s, pos - s);
        }

        /// <summary>
        /// Read a QSAFE-CHAR sequence
        /// </summary>
        /// <remarks>
        /// QSAFE-CHAR   = WSP / %x21 / %x23-7E / NON-ASCII ; Any character except CTLs, DQUOTE
        /// </remarks>
        protected virtual string ReadQSafeChar(string line, ref int pos)
        {
            int s = pos, l = line.Length;
            char c;
            while (pos < l && ((c = line[pos]) != DQUOTE && !IsCTL(c)))
                pos++;
            return s == pos ? string.Empty : line.Substring(s, pos - s);
        }

        /// <summary>
        /// Read a SAFE-CHAR sequence
        /// </summary>
        /// <remarks>
        /// SAFE-CHAR    = WSP / %x21 / %x23-2B / %x2D-39 / %x3C-7E / NON-ASCII ; Any character except CTLs, DQUOTE, ";", ":", ","
        /// </remarks>
        protected virtual string ReadSafeChar(string line, ref int pos)
        {
            int s = pos, l = line.Length;
            char c;
            while (pos < l && ((c = line[pos]) != DQUOTE && c != ';' && c != ':' && c != ',' && !IsCTL(c)))
                pos++;
            return s == pos ? string.Empty : line.Substring(s, pos - s);
        }

        /// <summary>
        /// Skip the char if in <paramref name="chars"/>
        /// </summary>
        protected virtual bool SkipChar(string line, ref int pos, params char[] chars)
        {
            if (pos >= line.Length || chars == null || chars.Length == 0) return false;
            bool r = chars.Contains(line[pos]);
            if (r) pos++;
            return r;
        }

        /// <summary>
        /// Decode a content line
        /// </summary>
        public virtual ContentLine DecodeContentLine(string line, int lineNumber = -1, bool strict = false, bool throwErrors = false)
        {
            // An empty line is not error
            if (string.IsNullOrWhiteSpace(line)) return null;

            ContentLine content = null;
            int pos = 0;
            ContentLine CheckError(string message, params object[] formatArgs)
            {
                if (throwErrors)
                    throw new ContentSyntaxError(formatArgs != null ? string.Format(message, formatArgs) : message, lineNumber, lineNumber >= 0 ? pos : -1);
                return strict ? null : content;
            };
            char CurrentChar() => pos < line.Length ? line[pos] : '\0';

            // Read the name
            string name = ReadIanaToken(line, ref pos);
            if (name == null) return CheckError(SR.DecodeContentLine_InvalidCharForName, CurrentChar());
            content = new ContentLine();
            if (SkipChar(line, ref pos, '.'))
            {
                content.Group = name;
                name = ReadIanaToken(line, ref pos);
                if (name == null) return CheckError(SR.DecodeContentLine_InvalidCharForName, CurrentChar());
            }
            content.Name = name;

            // Read the parameters
            while (SkipChar(line, ref pos, ';'))
            {
                // Read the name
                name = ReadIanaToken(line, ref pos);
                if (name == null) return CheckError(SR.DecodeContentLine_InvalidCharForName, CurrentChar());
                if (!SkipChar(line, ref pos, '=')) return CheckError(SR.DecodeContentLine_ExpectedChar, '=', CurrentChar());

                // Read the values
                char c = CurrentChar();
                do
                {
                    if (c == DQUOTE)
                    {
                        pos++;
                        string value = ReadQSafeChar(line, ref pos);
                        if (!SkipChar(line, ref pos, DQUOTE)) return CheckError(SR.DecodeContentLine_ExpectedChar, DQUOTE, CurrentChar());
                        content.AddParam(name, UnescapeParamValue(value));
                    }
                    else
                    {
                        string value = ReadSafeChar(line, ref pos);
                        content.AddParam(name, UnescapeParamValue(value));
                    }
                } while (SkipChar(line, ref pos, ','));
            }

            // Read the value
            if (SkipChar(line, ref pos, ':'))
            {
                content.Value = line.Substring(pos);
            }
            else
            {
                if (pos < line.Length)
                    return CheckError(SR.DecodeContentLine_ExpectedChar, ':', CurrentChar());
            }
            return content;
        }

        #endregion

        #region Encode

        /// <summary>
        /// Escape a parameter value
        /// </summary>
        protected virtual string EscapeParamValue(string value)
        {
            return !CircumflexEscape ? value : value
                    .Replace(CRLF, "^n")
                    .Replace(sLF, "^n")
                    .Replace(sCR, "^n")
                    .Replace("^", "^^")
                    .Replace("\"", "^'")
                    ;
        }

        /// <summary>
        /// Encode a parameter value
        /// </summary>
        protected virtual string EncodeParamValue(string value)
        {
            //if (value == null) return null;
            if (value.IndexOfAny(new char[] { ',', ';', '=' }) >= 0 && !(value.StartsWith("\"") && value.EndsWith("\"")))
                return $"\"{EscapeParamValue(value)}\"";
            return EscapeParamValue(value);
        }

        /// <summary>
        /// Encode a content line
        /// </summary>
        public virtual string EncodeContentLine(ContentLine line)
        {
            if (line == null) return null;
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(line.Group))
            {
                builder.Append(line.Group);
                if (!string.IsNullOrEmpty(line.Name))
                    builder.Append('.');
            }
            builder.Append(line.Name);
            foreach (var prm in line.GetParams())
            {
                builder
                    .Append(";")
                    .Append(prm.Name)
                    .Append("=")
                    ;
                bool first = true;
                foreach (string pVal in prm.Values)
                {
                    if (!first) builder.Append(",");
                    builder.Append(EncodeParamValue(pVal));
                    first = false;
                }
            }
            builder.Append(":").Append(line.Value);
            return builder.ToString();
        }

        #endregion

        /// <summary>
        /// Enable/disable the circumflex escape as defined by RFC-6868
        /// </summary>
        public bool CircumflexEscape { get; set; } = true;

    }

}
