using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// DAV Href
    /// </summary>
    public class DavHref : DavNode
    {
        /// <summary>
        /// HRef
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// Absolute uri
        /// </summary>
        public Uri Uri { get; private set; }
    }

}
