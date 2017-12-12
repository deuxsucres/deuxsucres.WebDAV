using deuxsucres.WebDAV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using WebDavTools.Models;

namespace WebDavTools
{
    public interface IViewer
    {
        Task LoadViewerAsync(BrowseItem item, HttpResponseMessage head, IWebDavService webdav);
    }
}
