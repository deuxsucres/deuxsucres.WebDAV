using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'lockentry' node
    /// </summary>
    public class DavLockEntry : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            LockScope = MakeNode<DavLockScope>(Node.Element(WebDavConstants.NsDAV + "lockscope"));
            LockType = MakeNode<DavLockType>(Node.Element(WebDavConstants.NsDAV + "locktype"));
        }

        /// <summary>
        /// Lock scope
        /// </summary>
        public DavLockScope LockScope { get; private set; }

        /// <summary>
        /// Lock type
        /// </summary>
        public DavLockType LockType { get; private set; }
    }
}
