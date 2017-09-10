using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDavTools.Models
{
    public class RequestHistory
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public string Log { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }
        public string Path { get; internal set; }
        public Uri ServerUri { get; internal set; }
    }
}
