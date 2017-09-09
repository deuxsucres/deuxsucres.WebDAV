using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'displayname' property node
    /// </summary>
    public class DavDisplayName : DavProperty
    {

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            DisplayName = (string)node;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; private set; }
    }
}
