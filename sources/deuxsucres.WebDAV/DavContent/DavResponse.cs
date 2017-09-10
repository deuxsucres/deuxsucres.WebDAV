using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Response node
    /// </summary>
    public class DavResponse : DavNode
    {
        /// <summary>
        /// Source of 'propstat' elements
        /// </summary>
        protected List<DavPropstat> SourcePropstats = new List<DavPropstat>();

        /// <summary>
        /// Load the node
        /// </summary>
        protected override void Load(Uri rootUri, XElement node, bool checkName)
        {
            base.Load(rootUri, node, checkName);
            Href = MakeNode<DavHref>(Node.Element(WebDavConstants.NsDAV + "href"));
            var propstats = Node.Elements(WebDavConstants.NsDAV + "propstat").ToList();
            if (propstats.Count > 0)
            {
                SourcePropstats.AddRange(MakeNodes<DavPropstat>(propstats));
            }
            else
            {
                Status = MakeNode<DavStatus>(Node.Element(WebDavConstants.NsDAV + "status"));
            }
            Error = MakeNode<DavError>(Node.Element(WebDavConstants.NsDAV + "error"));
            ResponseDescription = MakeNode<DavResponseDescription>(Node.Element(WebDavConstants.NsDAV + "responsedescription"));
            Location = MakeNode<DavLocation>(Node.Element(WebDavConstants.NsDAV + "location"));
        }

        /// <summary>
        /// List all properties
        /// </summary>
        public IEnumerable<DavProperty> GetProperties()
        {
            return SourcePropstats
                .SelectMany(sp => sp.Prop?.Properties ?? Enumerable.Empty<DavProperty>())
                ;
        }

        /// <summary>
        /// List all properties of a name
        /// </summary>
        public IEnumerable<DavProperty> GetProperties(XName name)
            => GetProperties().Where(p => p.Name == name);

        /// <summary>
        /// List all properties of a type
        /// </summary>
        public IEnumerable<T> GetProperties<T>() where T : DavProperty
            => GetProperties().OfType<T>();

        /// <summary>
        /// Get the first property of a type
        /// </summary>
        public T GetProperty<T>() where T : DavProperty
            => GetProperties<T>().FirstOrDefault();

        /// <summary>
        /// Href
        /// </summary>
        public DavHref Href { get; private set; }

        /// <summary>
        /// Propstats
        /// </summary>
        public DavPropstat[] Propstats => SourcePropstats.ToArray();

        /// <summary>
        /// Status
        /// </summary>
        public DavStatus Status { get; private set; }

        /// <summary>
        /// Error
        /// </summary>
        public DavError Error { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public DavResponseDescription ResponseDescription { get; private set; }

        /// <summary>
        /// Location
        /// </summary>
        public DavLocation Location { get; private set; }
    }
}
