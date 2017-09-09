using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// DAV Property
    /// </summary>
    public class DavProperty : DavNode
    {
        /// <summary>
        /// Default root uri
        /// </summary>
        public static readonly Uri DefaultRootUri = new Uri("http://localhost");

        /// <summary>
        /// Default constructor
        /// </summary>
        public DavProperty() : base() { }

        /// <summary>
        /// Create a new property
        /// </summary>
        public DavProperty(string name, XNamespace ns = null, Uri rootUri = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name.", nameof(name));
            Load(rootUri ?? DefaultRootUri, new XElement((ns ?? WebDavConstants.NsDAV) + Name), false);
        }

        /// <summary>
        /// Create a new property
        /// </summary>
        public DavProperty(XName nodeName, Uri rootUri = null)
        {
            Load(rootUri ?? DefaultRootUri, new XElement(nodeName ?? throw new ArgumentNullException(nameof(nodeName))), false);
        }

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, false);
        }

        /// <summary>
        /// Implicit cast from a <see cref="XName"/>
        /// </summary>
        public static implicit operator DavProperty(XName name) 
            => new DavProperty(name);

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name => NodeName.LocalName;

        /// <summary>
        /// Namespace
        /// </summary>
        public XNamespace Namespace => NodeName.Namespace;
    }
}
