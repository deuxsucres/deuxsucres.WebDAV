using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Exception for syntax error
    /// </summary>
    public class CalSyntaxError : Exception
    {
        /// <summary>
        /// Create a new exception with a message
        /// </summary>
        public CalSyntaxError(string message) : base(message) { }

        /// <summary>
        /// Create a new exception with a message and an inner exception
        /// </summary>
        public CalSyntaxError(string message, Exception innerException) : base(message, innerException) { }
    }
}
