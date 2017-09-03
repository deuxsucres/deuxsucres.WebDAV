using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Arguments for events about a web request
    /// </summary>
    public class WebRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new event arguments
        /// </summary>
        public WebRequestEventArgs(HttpRequestMessage request)
        {
            Request = request;
        }
        /// <summary>
        /// The request concerned by the event
        /// </summary>
        public HttpRequestMessage Request { get; private set; }
    }

    /// <summary>
    /// Arguments for event about a web response
    /// </summary>
    public class WebResponseEventArgs : EventArgs
    {

        /// <summary>
        /// Create a new events arguments
        /// </summary>
        public WebResponseEventArgs(HttpResponseMessage response)
        {
            Response = response;
        }
        /// <summary>
        /// Response
        /// </summary>
        public HttpResponseMessage Response { get; private set; }
    }
}
