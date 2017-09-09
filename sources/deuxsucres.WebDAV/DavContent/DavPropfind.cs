using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Content of method PROPFIND
    /// </summary>
    [ContentNodeName("propfind")]
    public class DavPropfind : DavNode
    {

        /// <summary>
        /// Define this content as a 'propname' query
        /// </summary>
        public DavPropfind AsPropname()
        {
            Node.RemoveNodes();
            Node.Add(new XElement(WebDavConstants.NsDAV + "propname"));
            return this;
        }

        /// <summary>
        /// Define this content as a 'allprop' query
        /// </summary>
        public DavPropfind AsAllProp(IEnumerable<DavProperty> includes = null)
        {
            Node.RemoveNodes();
            Node.Add(new XElement(WebDavConstants.NsDAV + "allprop"));
            if (includes != null)
                AddProperties(includes);
            return this;
        }

        /// <summary>
        /// Define this content as a 'prop' query
        /// </summary>
        public DavPropfind AsProp(IEnumerable<DavProperty> properties = null)
        {
            Node.RemoveNodes();
            Node.Add(new XElement(WebDavConstants.NsDAV + "prop"));
            if (properties != null)
                AddProperties(properties);
            return this;
        }

        /// <summary>
        /// Add properties
        /// </summary>
        public DavPropfind AddProperties(IEnumerable<DavProperty> properties)
        {
            if (properties == null) return this;
            if (IsPropname)
                throw new InvalidOperationException(Locales.SR.Err_CantAddPropertyInPropnameContent);
            // If it's a 'allprop' content add the property in the 'include' node
            XElement pnode = null;
            if (Node.Element(WebDavConstants.NsDAV + "allprop") != null)
            {
                pnode = Node.Element(WebDavConstants.NsDAV + "include");
                if (pnode == null)
                {
                    pnode = new XElement(WebDavConstants.NsDAV + "include");
                    Node.Add(pnode);
                }
            }
            else
            {
                pnode = Node.Element(WebDavConstants.NsDAV + "prop");
                if (pnode == null)
                {
                    pnode = new XElement(WebDavConstants.NsDAV + "prop");
                    Node.Add(pnode);
                }
            }
            foreach (var prop in properties.Where(p => p != null))
                pnode.Add(new XElement(prop.NodeName));
            return this;
        }

        /// <summary>
        /// List all properties
        /// </summary>
        public IEnumerable<DavProperty> GetProperties()
        {
            var pnode = Node.Element(WebDavConstants.NsDAV + "prop") ?? Node.Element(WebDavConstants.NsDAV + "include");
            return pnode != null
                ? pnode.Elements().Select(n => new DavProperty(n.Name))
                : Enumerable.Empty<DavProperty>();
        }

        /// <summary>
        /// Indicates if the node is a 'propname' query
        /// </summary>
        public bool IsPropname => Node.Descendants(WebDavConstants.NsDAV + "propname").Any();

    }
}
