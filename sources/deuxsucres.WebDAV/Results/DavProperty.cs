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
            XmlTag = (ns ?? WebDavConstants.NsDAV) + Name;
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// XML tag
        /// </summary>
        public XName XmlTag { get; }

        /// <summary>
        /// Namespace
        /// </summary>
        public XNamespace Namespace => XmlTag.Namespace;
    }
}
