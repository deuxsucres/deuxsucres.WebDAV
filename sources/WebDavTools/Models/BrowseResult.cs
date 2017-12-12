using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDavTools.Models
{
    public class BrowseResult
    {
        public BrowseItem RootItem { get; set; }
        public BrowseItem ParentItem { get; set; }
        public List<BrowseItem> Items { get; set; }
    }
}
