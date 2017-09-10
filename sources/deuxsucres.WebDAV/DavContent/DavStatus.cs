using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Status node
    /// </summary>
    public class DavStatus : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            var parts = ((string)node).Split(new char[] { ' ' }, 3);
            Protocol = parts[0];
            StatusCode = int.Parse(parts[1]);
            StatusDescription = parts[2];
        }

        /// <summary>
        /// Protocol
        /// </summary>
        public string Protocol { get; private set; }

        /// <summary>
        /// Status code
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// Status description
        /// </summary>
        public string StatusDescription { get; private set; }

    }
}
