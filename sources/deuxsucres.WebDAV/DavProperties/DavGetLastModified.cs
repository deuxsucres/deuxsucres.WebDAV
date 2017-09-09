using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'getlastmodified' property node
    /// </summary>
    public class DavGetLastModified : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            if (DateTimeOffset.TryParse((string)Node, out DateTimeOffset dto))
                LastModified = dto;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return LastModified?.ToString();
        }

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTimeOffset? LastModified { get; private set; }
    }
}
