using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'creationdate' property node
    /// </summary>
    public class DavCreationDate : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            if (DateTimeOffset.TryParse((string)Node, out DateTimeOffset dto))
                CreationDate = dto;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return CreationDate?.ToString();
        }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTimeOffset? CreationDate { get; private set; }
    }
}
