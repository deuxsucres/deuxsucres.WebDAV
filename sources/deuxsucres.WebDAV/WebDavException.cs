using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// WebDav exception
    /// </summary>
    public class WebDavException : Exception
    {
        /// <summary>
        /// Create a new exception
        /// </summary>
        public WebDavException(string message) : base(message)
        {
        }
    }
}
