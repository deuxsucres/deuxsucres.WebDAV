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
        /// Default constructor
        /// </summary>
        public DavProperty() : base() { }

        /// <summary>
        /// Create a new property
        /// </summary>
        public DavProperty(string name, XNamespace ns = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name.", nameof(name));
            Load(new XElement((ns ?? WebDavConstants.NsDAV) + Name), false);
        }

        /// <summary>
        /// Create a new property
        /// </summary>
        public DavProperty(XName nodeName)
        {
            Load(new XElement(nodeName ?? throw new ArgumentNullException(nameof(nodeName))), false);
        }

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, false);
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
