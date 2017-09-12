using deuxsucres.WebDAV;
using System;
using System.Net.Http;

namespace deuxsucres.CalDAV
{
    /// <summary>
    /// CalDAV client
    /// </summary>
    public class CalDavClient : WebDavClient
    {
        /// <summary>
        /// Create a new client
        /// </summary>
        public CalDavClient(string uri, string userName = null, string password = null, HttpMessageHandler handler = null) 
            : base(uri, userName, password, handler)
        {
        }
    }
}
