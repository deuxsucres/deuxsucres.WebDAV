using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// 'getcontenttype' property node
    /// </summary>
    public class DavGetContentType : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            ContentType = (string)node;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return ContentType;
        }

        /// <summary>
        /// Content type
        /// </summary>
        public string ContentType { get; private set; }
    }

}
