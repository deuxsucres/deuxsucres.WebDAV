using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'locktype'  node
    /// </summary>
    public class DavLockType : DavNode
    {

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            DavLockTypeType? lockType = Node.Element(WebDavConstants.NsDAV + "write") != null ? (DavLockTypeType?)DavLockTypeType.Write
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
        public DavLockTypeType LockType { get; private set; }
    }

    /// <summary>
    /// Type of lock 
    /// </summary>
    public enum DavLockTypeType
    {
        /// <summary>
        /// Write
        /// </summary>
        Write
    }

}
