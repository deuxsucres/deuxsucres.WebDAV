using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDavTools.Models
{
    public class RequestResult<T>
    {
        public Uri ServerUri { get; set; }
        public T Result { get; set; }
    }
}
