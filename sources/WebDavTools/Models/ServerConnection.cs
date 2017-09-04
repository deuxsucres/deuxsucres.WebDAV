using System;
using System.Collections.Generic;
using System.Text;

namespace WebDavTools.Models
{
    public class ServerConnection
    {

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string Server => Uri != null ? $"{Uri.Scheme}://{Uri.Authority}" : null;

        public string AbsolutePath => Uri?.AbsolutePath;

        public string UserName { get; set; }

        public string Password { get; set; }

    }
}
