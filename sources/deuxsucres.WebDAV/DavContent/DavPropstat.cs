using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'propstat' node
    /// </summary>
    public class DavPropstat : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            Prop = MakeNode<DavProp>(Node.Element(WebDavConstants.NsDAV + "prop"));
            Status = MakeNode<DavStatus>(Node.Element(WebDavConstants.NsDAV + "status"));
            Error = MakeNode<DavError>(Node.Element(WebDavConstants.NsDAV + "error"));
            ResponseDescription = MakeNode<DavResponseDescription>(Node.Element(WebDavConstants.NsDAV + "responsedescription"));
        }

        /// <summary>
        /// Properties informations
        /// </summary>
        public DavProp Prop { get; private set; }

        /// <summary>
        /// Status
        /// </summary>
        public DavStatus Status { get; private set; }

        /// <summary>
        /// Error
        /// </summary>
        public DavError Error { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public DavResponseDescription ResponseDescription { get; private set; }
    }
}
