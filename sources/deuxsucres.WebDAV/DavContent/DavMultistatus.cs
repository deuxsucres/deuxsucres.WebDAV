using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// Create a multistatus node
    /// </summary>
    public class DavMultistatus : DavNode
    {
        /// <summary>
        /// List of responses
        /// </summary>
        protected List<DavResponse> SourceResponses { get; private set; } = new List<DavResponse>();

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            SourceResponses.AddRange(MakeNodes<DavResponse>(WebDavConstants.NsDAV.GetName("response")));
            ResponseDescription = MakeNode<DavResponseDescription>(WebDavConstants.NsDAV.GetName("responsedescription"));
        }

        /// <summary>
        /// Responses
        /// </summary>
        public DavResponse[] Responses => SourceResponses.ToArray();

        /// <summary>
        /// Description
        /// </summary>
        public DavResponseDescription ResponseDescription { get; private set; }

    }

}
