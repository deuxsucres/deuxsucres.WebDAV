using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'lockroot' node
    /// </summary>
    public class DavLockRoot : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            Href = MakeNode<DavHref>(Node.Element(WebDavConstants.NsDAV + "href"));
        }

        /// <summary>
        /// Href
        /// </summary>
        public DavHref Href { get; private set; }
    }
}
