using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDavTools.Models
{
    public class BrowseItem
    {
        public Uri Uri { get; set; }
        public string Path { get; set; }
        public string DisplayName { get; set; }
        public DateTimeOffset? CreationDate { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string ContentType { get; set; }
        public int? ContentLength { get; set; }
        public bool IsCollection { get; set; }
    }
}
