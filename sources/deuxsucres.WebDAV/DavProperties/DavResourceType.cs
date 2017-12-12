using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// 'resourcetype' property node
    /// </summary>
    public class DavResourceType : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            IsCollection = Node.Elements(WebDavConstants.NsDAV + "collection").Any();
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return string.Join(", ", Node.Elements().Select(e => e.Name == WebDavConstants.NsDAV + "collection" ? "collection" : e.Name.ToString()));
        }

        /// <summary>
        /// Indicates if the resource is a collection
        /// </summary>
        public bool IsCollection { get; private set; }
    }

}
