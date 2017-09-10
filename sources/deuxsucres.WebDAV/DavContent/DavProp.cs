﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// 'prop' node
    /// </summary>
    public class DavProp : DavNode
    {

        /// <summary>
        /// Source of the properties
        /// </summary>
        protected List<DavProperty> SourceProperties { get; private set; } = new List<DavProperty>();

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            SourceProperties.AddRange(MakeProperties(Node.Elements()));
        }

        /// <summary>
        /// Add a property
        /// </summary>
        public DavProp AddProperty(DavProperty prop)
        {
            if (prop != null)
            {
                SourceProperties.Add(prop);
                Node.Add(prop.Node);
            }
            return this;
        }

        /// <summary>
        /// Properties
        /// </summary>
        public DavProperty[] Properties => SourceProperties.ToArray();

    }
}
