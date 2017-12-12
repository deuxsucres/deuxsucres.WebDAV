using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Define the DAV content node name
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ContentNodeNameAttribute : Attribute
    {
        /// <summary>
        /// Create an attribute
        /// </summary>
        public ContentNodeNameAttribute(string name)
        {
            NodeName = XName.Get(name ?? throw new ArgumentNullException(nameof(name)), WebDavConstants.NsDAV.NamespaceName);
        }
        /// <summary>
        /// Create an attribute
        /// </summary>
        public ContentNodeNameAttribute(string name, string ns)
        {
            NodeName = XName.Get(
                name ?? throw new ArgumentNullException(nameof(name)),
                ns ?? throw new ArgumentNullException(nameof(ns))
                );
        }
        /// <summary>
        /// Name of the node
        /// </summary>
        public XName NodeName { get; private set; }
    }
}
