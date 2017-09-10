using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'getcontentlanguage' property node
    /// </summary>
    public class DavGetContentLanguage : DavProperty
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            Language = (string)node;
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Language;
        }

        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; private set; }
    }
}
