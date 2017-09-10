using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'activelock' node
    /// </summary>
    public class DavActiveLock : DavNode
    {

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            LockScope = MakeNode<DavLockScope>(Node.Element(WebDavConstants.NsDAV + "lockscope"));
            LockType = MakeNode<DavLockType>(Node.Element(WebDavConstants.NsDAV + "locktype"));
            Depth = ParseHelpers.ParseDepth((string)Node.Element(WebDavConstants.NsDAV + "depth")) ?? DepthValue.Infinity;
            Owner = MakeNode<DavOwner>(Node.Element(WebDavConstants.NsDAV + "owner"));
            Timeout = MakeNode<DavTimeout>(Node.Element(WebDavConstants.NsDAV + "timeout"));
            LockToken = MakeNode<DavLockToken>(Node.Element(WebDavConstants.NsDAV + "locktoken"));
            LockRoot = MakeNode<DavLockRoot>(Node.Element(WebDavConstants.NsDAV + "lockroot"));
        }

        /// <summary>
        /// Lock scope
        /// </summary>
        public DavLockScope LockScope { get; private set; }

        /// <summary>
        /// Lock type
        /// </summary>
        public DavLockType LockType { get; private set; }

        /// <summary>
        /// Depth
        /// </summary>
        public DepthValue Depth { get; private set; }

        /// <summary>
        /// Owner
        /// </summary>
        public DavOwner Owner { get; private set; }

        /// <summary>
        /// Timeout
        /// </summary>
        public DavTimeout Timeout { get; private set; }

        /// <summary>
        /// Lock token
        /// </summary>
        public DavLockToken LockToken { get; private set; }

        /// <summary>
        /// Lock root
        /// </summary>
        public DavLockRoot LockRoot { get; private set; }
    }
}
