using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// 'lockscope' node
    /// </summary>
    public class DavLockScope : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            DavLockScopeType? lockType = Node.Element(WebDavConstants.NsDAV + "exclusive") != null ? (DavLockScopeType?)DavLockScopeType.Exclusive
                : Node.Element(WebDavConstants.NsDAV + "shared") != null ? (DavLockScopeType?)DavLockScopeType.Shared 
                : null;
            if (!lockType.HasValue)
                if (Node.HasElements)
                    throw new InvalidOperationException(string.Format(Locales.SR.Err_InvalidLockType, Node.Elements().First().Name));
                else
                    throw new InvalidOperationException(Locales.SR.Err_LockTypeUndefined);
            LockType = lockType.Value;
        }

        /// <summary>
        /// Lock type
        /// </summary>
        public DavLockScopeType LockType { get; private set; }
    }

    /// <summary>
    /// Type of lock scope
    /// </summary>
    public enum DavLockScopeType
    {
        /// <summary>
        /// Exclusive lock
        /// </summary>
        Exclusive,
        /// <summary>
        /// Shared lock
        /// </summary>
        Shared
    }

}
