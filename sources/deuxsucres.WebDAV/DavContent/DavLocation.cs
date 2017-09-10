using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'location' node
    /// </summary>
    public class DavLocation : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            Href = (string)Node;
        }

        /// <summary>
        /// HRef
        /// </summary>
        public string Href { get; private set; }
    }
}
