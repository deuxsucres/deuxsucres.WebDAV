﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'lockinfo' node
    /// </summary>
    public class DavLockInfo : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            LockScope = MakeNode<DavLockScope>(Node.Element(WebDavConstants.NsDAV + "lockscope"));
            LockType = MakeNode<DavLockType>(Node.Element(WebDavConstants.NsDAV + "locktype"));
            Owner = MakeNode<DavOwner>(Node.Element(WebDavConstants.NsDAV + "owner"));
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
        /// Owner
        /// </summary>
        public DavOwner Owner { get; private set; }
    }
}
