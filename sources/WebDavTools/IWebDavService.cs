using deuxsucres.WebDAV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDavTools.Models;

namespace WebDavTools
{
    public interface IWebDavService
    {
        Task<RequestResult<T>> CallRequestAsync<T>(Func<WebDavClient, Task<T>> call);
    }
}
