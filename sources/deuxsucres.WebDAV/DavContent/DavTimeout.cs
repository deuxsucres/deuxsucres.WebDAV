using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'timeout' node
    /// </summary>
    public class DavTimeout : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            Timeout = ParseHelpers.ParseTimeout((string)node);
        }

        /// <summary>
        /// Timeout
        /// </summary>
        public uint? Timeout { get; private set; }

    }
}
