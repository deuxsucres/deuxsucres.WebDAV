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
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            foreach (var child in Node.Elements())
            {
                if (child.Name == WebDavConstants.NsDAV + "remove")
                    SourceUpdateNodes.Add(DavNode.LoadNode<DavRemove>(node));
                else if (child.Name == WebDavConstants.NsDAV + "set")
                    SourceUpdateNodes.Add(DavNode.LoadNode<DavSet>(node));
            }
        }

        /// <summary>
        /// Add a set property node
        /// </summary>
        public DavPropertyUpdate Set(DavProperty prop)
        {
            if (prop == null) return this;
            var setNode = DavNode.CreateNode<DavSet>();
            setNode.AddProperty(prop);
            SourceUpdateNodes.Add(setNode);
            Node.Add(setNode.Node);
            return this;
        }

        /// <summary>
        /// Add a remove property node
        /// </summary>
        public DavPropertyUpdate Remove(DavProperty prop)
        {
            if (prop == null) return this;
            return Remove(prop.NodeName);
        }

        /// <summary>
        /// Add a remove property node
        /// </summary>
        public DavPropertyUpdate Remove(XName name)
        {
            if (name == null) return this;
            var removeNode = DavNode.CreateNode<DavRemove>();
            removeNode.AddProperty(DavProperties.CreateProperty(name));
            SourceUpdateNodes.Add(removeNode);
            Node.Add(removeNode.Node);
            return this;
        }

        /// <summary>
        /// List of the update nodes
        /// </summary>
        public DavPropertyUpdateNode[] UpdateNodes => SourceUpdateNodes.ToArray();

    }
}
