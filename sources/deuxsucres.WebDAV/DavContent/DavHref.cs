﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// DAV Href
    /// </summary>
    public class DavHref : DavNode
    {

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            Href = (string)Node;
        }

        /// <summary>
        /// HRef
        /// </summary>
        public string Href { get; private set; }

        /// <summary>
        /// Absolute uri
        /// </summary>
        public Uri Uri => new Uri(RootUri, Href ?? string.Empty);
    }

}
