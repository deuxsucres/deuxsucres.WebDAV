using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Syntax error
    /// </summary>
    public class ContentSyntaxError : Exception
    {
        /// <summary>
        /// Create a new syntax error
        /// </summary>
        public ContentSyntaxError(string message, int lineNumber = -1, int linePos = -1) : base(message)
        {
            LineNumber = lineNumber;
            LinePosition = linePos;
        }

        /// <summary>
        /// Message
        /// </summary>
        public override string Message => string.Format(Locales.SR.ContentSyntaxErrorMessage, LineNumber, LinePosition, base.Message);

        /// <summary>
        /// Raw error message
        /// </summary>
        public string ErrorMessage => base.Message;

        /// <summary>
        /// Line number
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Position in the line
        /// </summary>
        public int LinePosition { get; private set; }
    }
}
