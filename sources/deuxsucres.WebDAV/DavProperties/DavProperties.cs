using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// DAV properties registrar
    /// </summary>
    public static class DavProperties
    {
        static Dictionary<XName, Type> _registered = new Dictionary<XName, Type>();

        /// <summary>
        /// Static constructor for registering standard properties
        /// </summary>
        static DavProperties()
        {
            Register<DavCreationDate>();
            Register<DavDisplayName>();
            Register<DavGetContentLanguage>();
            Register<DavGetContentLength>();
            Register<DavGetContentType>();
            Register<DavGetETag>();
            Register<DavGetLastModified>();
            Register<DavLockDiscovery>();
            Register<DavResourceType>();
            Register<DavSupportedLock>();
        }

        /// <summary>
        /// Register a property type
        /// </summary>
        public static void Register<T>() where T : DavProperty
        {
            _registered[DavNode.GetContentNodeName<T>()] = typeof(T);
        }

        /// <summary>
        /// Create a property from the name registered
        /// </summary>
        public static DavProperty CreateProperty(XName name, bool nullIfNotRegistered = false)
        {
            if (name == null) return null;
            return LoadProperty(new XElement(name), nullIfNotRegistered);
        }

        /// <summary>
        /// Load a property from the name registered
        /// </summary>
        public static DavProperty LoadProperty(XElement node, bool nullIfNotRegistered = false)
        {
            if (node == null) return null;
            if (!_registered.TryGetValue(node.Name, out Type tProp))
                return nullIfNotRegistered ? null : DavNode.LoadNode<DavProperty>(node, false);
            return DavNode.LoadNode(tProp, node, false) as DavProperty;
        }
    }

}
