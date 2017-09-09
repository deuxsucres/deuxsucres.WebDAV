using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// Response desciption
    /// </summary>
    public class DavResponseDescription : DavNode
    {

        /// <summary>
        /// Create a new node
        /// </summary>
        public DavResponseDescription(Uri rootUri, XElement node) : base(rootUri, node)
        {
        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description => (string)Node;
    }

}
