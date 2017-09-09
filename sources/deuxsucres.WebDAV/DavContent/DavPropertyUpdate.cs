using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'propertyupdate' node
    /// </summary>
    public class DavPropertyUpdate : DavNode
    {

        /// <summary>
        /// Source of the update nodes
        /// </summary>
        protected List<DavPropertyUpdateNode> SourceUpdateNodes { get; private set; } = new List<DavPropertyUpdateNode>();

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            foreach (var child in Node.Elements())
            {
                if (child.Name == WebDavConstants.NsDAV + "remove")
                    SourceUpdateNodes.Add(DavNode.LoadNode<DavRemove>(RootUri, node));
                else if (child.Name == WebDavConstants.NsDAV + "set")
                    SourceUpdateNodes.Add(DavNode.LoadNode<DavSet>(RootUri, node));
            }
        }

        /// <summary>
        /// List of the update nodes
        /// </summary>
        public DavPropertyUpdateNode[] UpdateNodes => SourceUpdateNodes.ToArray();

    }
}
