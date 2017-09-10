using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'getcontentlength' property node
    /// </summary>
    public class DavGetContentLength : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            if (int.TryParse((string)Node, out int ival))
                ContentLength = ival;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return ContentLength?.ToString();
        }

        /// <summary>
        /// Content length
        /// </summary>
        public int? ContentLength { get; private set; }
    }
}
