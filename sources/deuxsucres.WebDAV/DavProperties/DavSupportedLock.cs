using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'supportedlock' property node
    /// </summary>
    public class DavSupportedLock : DavProperty
    {

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            LockEntry = MakeNode<DavLockEntry>(Node.Element(WebDavConstants.NsDAV + "lockentry"));
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return LockEntry?.ToString();
        }

        /// <summary>
        /// Lock entry
        /// </summary>
        public DavLockEntry LockEntry { get; private set; }

    }
}
