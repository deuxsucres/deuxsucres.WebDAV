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
        /// Create a new href
        /// </summary>
        public DavHref(Uri rootUri, XElement node) : base(rootUri, node)
        {
            Href = (string)node;
            Uri = new Uri(RootUri, Href ?? string.Empty);
        }

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
