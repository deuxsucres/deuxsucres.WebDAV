using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'getetag' property name
    /// </summary>
    public class DavGetETag : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            ETag = (string)node;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return ETag;
        }

        /// <summary>
        /// ETag
        /// </summary>
        public string ETag { get; private set; }
    }
}
