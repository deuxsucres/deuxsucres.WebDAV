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
            Timeout = (int)node;
        }

        /// <summary>
        /// Timeout
        /// </summary>
        public int Timeout { get; private set; }

    }
}
