using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'location' node
    /// </summary>
    public class DavLocation : DavNode
    {
        /// <summary>
        /// HRef
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// Absolute uri
        /// </summary>
        public Uri Uri => new Uri(RootUri, Href ?? string.Empty);
    }
}
