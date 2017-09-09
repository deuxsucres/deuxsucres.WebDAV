using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Error node
    /// </summary>
    public class DavError : DavNode
    {
        /// <summary>
        /// Create a new node
        /// </summary>
        public DavError(Uri rootUri, XElement node) : base(rootUri, node)
        {
        }
    }
}
