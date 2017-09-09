using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// DAV Property
    /// </summary>
    public class DavProperty
    {

        /// <summary>
        /// Create a new property
        /// </summary>
        public DavProperty(string name, XNamespace ns = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name.", nameof(name));
            Name = name;
            NodeName = (ns ?? WebDavConstants.NsDAV) + Name;
        }

        /// <summary>
        /// Create a new property
        /// </summary>
        public DavProperty(XName nodeName)
        {
            NodeName = nodeName ?? throw new ArgumentNullException(nameof(nodeName));
            Name = NodeName.LocalName;
        }

        /// <summary>
        /// Implicit cast from a <see cref="XName"/>
        /// </summary>
        public static implicit operator DavProperty(XName name) => new DavProperty(name);

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Node name
        /// </summary>
        public XName NodeName { get; }

        /// <summary>
        /// Namespace
        /// </summary>
        public XNamespace Namespace => NodeName.Namespace;
    }
}
