using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Base of the node wich update properties like 'remove' and 'set'
    /// </summary>
    public abstract class DavPropertyUpdateNode : DavNode
    {
        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(XElement node, bool checkName)
        {
            base.Load(node, checkName);
            Prop = MakeNode<DavProp>(Node.Element(WebDavConstants.NsDAV + "prop"));
        }

        /// <summary>
        /// Add a property
        /// </summary>
        public DavPropertyUpdateNode AddProperty(DavProperty prop)
        {
            if (prop != null)
            {
                if (Prop == null)
                {
                    Prop = CreateNode<DavProp>();
                    Node.Add(Prop.Node);
                }
                Prop.AddProperty(prop);
            }
            return this;
        }

        /// <summary>
        /// Properties informations
        /// </summary>
        public DavProp Prop { get; private set; }
    }

}
