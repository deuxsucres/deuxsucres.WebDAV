using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebDavTools.Models;

namespace WebDavTools.Views
{
    /// <summary>
    /// Logique d'interaction pour DefaultContentViewer.xaml
    /// </summary>
    public partial class DefaultContentViewer : UserControl, IViewer
    {
        public DefaultContentViewer()
        {
            InitializeComponent();
        }

        Task IViewer.LoadViewerAsync(BrowseItem item, HttpResponseMessage head, IWebDavService webdav)
        {
            headers.LoadHeaders(head);
            return Task.FromResult(true);
        }

    }
}
