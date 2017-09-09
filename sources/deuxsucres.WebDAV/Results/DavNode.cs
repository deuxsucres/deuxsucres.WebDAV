using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV.Results
{

    /// <summary>
    /// Base of the DAV node result
    /// </summary>
    public class DavNode
    {
        /// <summary>
        /// Create a new node
        /// </summary>
        public DavNode(Uri rootUri, XElement node)
        {
            RootUri = rootUri ?? throw new ArgumentNullException(nameof(rootUri));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        /// <summary>
        /// Create a new node from an element
        /// </summary>
        public T MakeNode<T>(XElement node, Func<DavNode, XElement, T> builder = null) where T : DavNode
        {
            if (node == null) return null;
            if (builder != null)
                return builder(this, node);
            return (T)Activator.CreateInstance(typeof(T), RootUri, node);
        }

        /// <summary>
        /// Create a new node from an element name
        /// </summary>
        public T MakeNode<T>(XName name, Func<DavNode, XElement, T> builder = null) where T : DavNode
        {
            if (name == null) return null;
            return MakeNode(Node.Element(name), builder);
        }

        /// <summary>
        /// Create a list of nodes from a list of elements
        /// </summary>
        public IEnumerable<T> MakeNodes<T>(IEnumerable<XElement> nodes, Func<DavNode, XElement, T> builder = null) where T : DavNode
        {
            if (nodes == null) return null;
            return nodes
                .Where(n => n != null)
                .Select(n => MakeNode(n, builder));
        }

        /// <summary>
        /// Create a list of nodes from a name
        /// </summary>
        public IEnumerable<T> MakeNodes<T>(XName name, Func<DavNode, XElement, T> builder = null) where T : DavNode
        {
            if (name == null) return null;
            return MakeNodes(Node.Elements(name), builder);
        }

        /// <summary>
        /// The root URI
        /// </summary>
        public Uri RootUri { get; private set; }

        /// <summary>
        /// Node
        /// </summary>
        public XElement Node { get; private set; }
    }

}
