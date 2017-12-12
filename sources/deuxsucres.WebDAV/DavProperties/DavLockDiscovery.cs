using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'lockdiscovery' property node
    /// </summary>
    public class DavLockDiscovery : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            ActiveLock = MakeNode<DavActiveLock>(Node.Element(WebDavConstants.NsDAV + "activelock"));
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return ActiveLock?.ToString();
        }

        /// <summary>
        /// Active lock
        /// </summary>
        public DavActiveLock ActiveLock { get; private set; }
    }
}
