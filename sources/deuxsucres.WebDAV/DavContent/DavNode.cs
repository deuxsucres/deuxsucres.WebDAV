using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// Base of the DAV node content
    /// </summary>
    public class DavNode
    {

        /// <summary>
        /// Extract the node name of a type of DAV content
        /// </summary>
        protected static XName GetContentNodeName(Type type)
        {
            var cntName = type.GetTypeInfo().GetCustomAttribute<ContentNodeNameAttribute>();
            if (cntName != null) return cntName.NodeName;
            string name = type.Name;
            if (name.StartsWith("Dav", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(3);
            return WebDavConstants.NsDAV.GetName(name.ToLower());
        }

        /// <summary>
        /// Extract the node name of a type of DAV content
        /// </summary>
        public static XName GetContentNodeName<T>() where T : DavNode
        {
            return GetContentNodeName(typeof(T));
        }

        /// <summary>
        /// Create a new generic node
        /// </summary>
        public static DavNode CreateNode(Uri rootUri, XName name)
        {
            var result = new DavNode();
            result.Load(rootUri, new XElement(name ?? throw new ArgumentNullException(nameof(name))), false);
            return result;
        }

        /// <summary>
        /// Create a new empty node
        /// </summary>
        public static T CreateNode<T>(Uri rootUri) where T : DavNode
        {
            var result = (T)Activator.CreateInstance(typeof(T));
            result.Load(rootUri, new XElement(GetContentNodeName<T>()), false);
            return result;
        }

        /// <summary>
        /// Load a generic node from XML element
        /// </summary>
        public static DavNode LoadNode(Uri rootUri, XElement node, bool checkName = true)
        {
            var result = new DavNode();
            result.Load(rootUri, node, false);
            return result;
        }

        /// <summary>
        /// Load a generic node from XML element
        /// </summary>
        public static DavNode LoadNode(Type tNode, Uri rootUri, XElement node, bool checkName = true)
        {
            var result = (DavNode)Activator.CreateInstance(tNode ?? throw new ArgumentNullException(nameof(tNode)));
            result.Load(rootUri, node, false);
            return result;
        }

        /// <summary>
        /// Load a node from XML element
        /// </summary>
        public static T LoadNode<T>(Uri rootUri, XElement node, bool checkName = true) where T : DavNode
        {
            var result = (T)Activator.CreateInstance(typeof(T));
            result.Load(rootUri, node, checkName);
            return result;
        }

        /// <summary>
        /// Load the node
        /// </summary>
        protected virtual void Load(Uri rootUri, XElement node, bool checkName)
        {
            if (checkName) CheckNodeName(node);
            RootUri = rootUri ?? throw new ArgumentNullException(nameof(rootUri));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        /// <summary>
        /// Check a node is the correct name
        /// </summary>
        protected virtual void CheckNodeName(XElement node)
        {
            var cName = GetContentNodeName(GetType());
            if (node?.Name != cName)
                throw new WebDavException(string.Format(Locales.SR.Err_InvalidNodeForContent, node.Name, cName));
        }

        /// <summary>
        /// Create a new node from an element
        /// </summary>
        public T MakeNode<T>(XElement node, Func<DavNode, XElement, T> builder = null) where T : DavNode
        {
            if (node == null) return null;
            if (builder != null)
                return builder(this, node);
            return LoadNode<T>(RootUri, node);
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
        /// Create a new property from registered properties or default property node
        /// </summary>
        public DavProperty MakeProperty(XElement node)
        {
            if (node == null) return null;
            var prop = DavProperties.CreateProperty(RootUri, node);
            return prop ?? MakeNode<DavProperty>(node);
        }

        /// <summary>
        /// Create a list od properties from a list of elements
        /// </summary>
        public IEnumerable<DavProperty> MakeProperties(IEnumerable<XElement> nodes)
        {
            if (nodes == null) return null;
            return nodes
                .Where(n => n != null)
                .Select(n => MakeProperty(n));
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Node.ToString();
        }

        /// <summary>
        /// The root URI
        /// </summary>
        public Uri RootUri { get; private set; }

        /// <summary>
        /// Node
        /// </summary>
        public XElement Node { get; private set; }

        /// <summary>
        /// Name of the node
        /// </summary>
        public XName NodeName => Node.Name;

    }

}
