using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// Response desciption
    /// </summary>
    public class DavResponseDescription : DavNode
    {
        /// <summary>
        /// Description
        /// </summary>
        public string Description => (string)Node;
    }

}
