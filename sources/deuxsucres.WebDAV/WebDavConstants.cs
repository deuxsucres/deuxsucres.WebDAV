using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Constants for WebDAV
    /// </summary>
    public static class WebDavConstants
    {

        /// <summary>
        /// The DAV XML namespace
        /// </summary>
        public readonly static XNamespace NsDAV = XNamespace.Get("DAV:");

        /// <summary>
        /// DAV header
        /// </summary>
        public const string DavHeader = "DAV";

        /// <summary>
        /// Allow header
        /// </summary>
        public const string AllowHeader = "Allow";

        #region DAV methods

        /// <summary>
        /// PROPFIND method
        /// </summary>
        public readonly static HttpMethod PropFind = new HttpMethod("PROPFIND");

        /// <summary>
        /// PROPPATCH method
        /// </summary>
        public readonly static HttpMethod PropPatch = new HttpMethod("PROPPATCH");

        /// <summary>
        /// MKCOL method
        /// </summary>
        public readonly static HttpMethod MkCol = new HttpMethod("MKCOL");

        /// <summary>
        /// COPY method
        /// </summary>
        public readonly static HttpMethod Copy = new HttpMethod("COPY");

        /// <summary>
        /// MOVE method
        /// </summary>
        public readonly static HttpMethod Move = new HttpMethod("MOVE");

        /// <summary>
        /// LOCK method
        /// </summary>
        public readonly static HttpMethod Lock = new HttpMethod("LOCK");

        /// <summary>
        /// UNLOCK method
        /// </summary>
        public readonly static HttpMethod Unlock = new HttpMethod("UNLOCK");

        /// <summary>
        /// OPTIONS method
        /// </summary>
        public readonly static HttpMethod Options = new HttpMethod("OPTIONS");

        #endregion
    }
}
